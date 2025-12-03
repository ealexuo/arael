using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using Dapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Data;
using System.ComponentModel.Design;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using Qfile.Core.Constantes;

namespace Qfile.Datos
{
    public class ProcesoDatos : IProcesoDatos
    {
        private readonly IConnectionProvider connectionProvider;

        public ProcesoDatos(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }
        public async Task<List<ProcesoModelo>> ObtenerTodosAsync(int idEntidad)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"
                SELECT pr.id_proceso idProceso, pr.id_entidad idEntidad, pr.nombre, pr.descripcion, pr.color, tipo_proceso idTipoProceso,
                    IIF(tipo_proceso = 0, 'Proceso Estandar', 'Proceso Personalizado') AS tipoproceso,
                    ua.id_unidad_administrativa idUnidadAdministrativa,
                    ua.nombre unidadadministrativa, 
                    ua.siglas siglasua,
                    pr.activo estado
                FROM proceso pr, ad_unidades_administrativas ua
                WHERE ua.id_unidad_administrativa = pr.id_unidad_administrativa
                AND ua.id_entidad = pr.id_entidad
                AND pr.id_entidad = @identidad
                ORDER BY pr.id_proceso";

                var result = await connection.QueryAsync<ProcesoModelo>(sqlQuery, new { idEntidad });
                return result.ToList();
            }
        }
        public async Task<int> CrearProcesoAsync(ProcesoModelo proceso, int idUsuarioRegistro, DateTime fechaRegistro)
        {
            const string insertarProcesoSQL = @"                
            INSERT INTO PROCESO
            (ID_ENTIDAD, ID_PROCESO, ID_UNIDAD_ADMINISTRATIVA, NOMBRE, DESCRIPCION, TIPO_PROCESO, ACTIVO, COLOR, FECHA_REGISTRO, USUARIO_REGISTRO)
            OUTPUT INSERTED.ID_PROCESO
            VALUES
            (@IdEntidad, (select ISNULL(MAX(id_proceso), 0) +1 from proceso where id_entidad = @IdEntidad), @IdUnidadAdministrativa, @Nombre, @Descripcion, @IdTipoProceso, @Activo, @Color, @FechaRegistro, @UsuarioRegistro)
            ";

            const string insertarPlantillaSQL = @"                
                ---------  Creación de la Plantilla -----------
                INSERT INTO PROCESO_PLANTILLA
                   (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, NOMBRE, VERSION, VERSION_PROPUESTA, ACTIVA, FECHA_REGISTRO, USUARIO_REGISTRO)
                 VALUES
                   (@IdEntidad, @IdProceso, 1, 'Plantilla Base', 0, 1, 1, @FechaRegistro, @UsuarioRegistro)

                ---------  Creación de la Sección Propuesta -----------
                INSERT INTO PROCESO_PLANTILLA_SECCION_PROPUESTA
                   (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, NOMBRE, ORDEN, ACTIVA,  DESCRIPCION)
                VALUES
                   (@IdEntidad, @IdProceso, 1, 1, 'Datos Generales', 1, 1,  'Sección Base') 

                ---------  Creación de la Sección -----------
                INSERT INTO PROCESO_PLANTILLA_SECCION
                   (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, NOMBRE, ORDEN, ACTIVA,  DESCRIPCION)
                VALUES
                   (@IdEntidad, @IdProceso, 1, 1, 'Datos Generales', 1, 1,  'Sección Base')";

            //const string insertarPlantillaSQL = @"                
            //    INSERT INTO PROCESO_PLANTILLA
            //       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, NOMBRE, VERSION, VERSION_PROPUESTA, ACTIVA, FECHA_REGISTRO, USUARIO_REGISTRO)
            //     VALUES
            //       (@IdEntidad, @IdProceso, 1, 'Plantilla Base', 0, 1, 1, @FechaRegistro, @UsuarioRegistro)";

            //const string insertarSeccionSQL = @"                
            //    INSERT INTO PROCESO_PLANTILLA_SECCION_PROPUESTA
            //       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, NOMBRE, ORDEN, ACTIVA,  DESCRIPCION)
            //     VALUES
            //       (@IdEntidad, @IdProceso, 1, 1, 'Datos Generales', 1, 1,  'Sección Base')";
            
            const string  plantillaStandard = @"
                ---------  Creación de los Campos Propuesta -----------
                 INSERT INTO PROCESO_PLANTILLA_SECCION_CAMPO_PROPUESTA
                   (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                    ORDEN, NOMBRE, DESCRIPCION, LONGITUD, OBLIGATORIO, 
                    NO_COLUMNAS, ID_TIPO_CAMPO, ACTIVO, ID_CAMPO_PADRE)
                 Values
                   (@IdEntidad, @IdProceso, 1, 1, 1, 
                    1, 'Número Original del Documento', 'Número que identifica al documento', 30, 0, 
                    4, 1, 1, NULL)
                 INSERT INTO PROCESO_PLANTILLA_SECCION_CAMPO_PROPUESTA
                   (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                    ORDEN, NOMBRE, DESCRIPCION, LONGITUD, OBLIGATORIO, 
                    NO_COLUMNAS, ID_TIPO_CAMPO, ACTIVO, ID_CAMPO_PADRE)
                 Values
                   (@IdEntidad, @IdProceso, 1, 1, 2, 
                    2, 'Tipo de Documento', 'Listado de diferentes tipos de documento que pueden presentarse', 10, 0, 
                    3, 6, 1, NULL)
                 INSERT INTO PROCESO_PLANTILLA_SECCION_CAMPO_PROPUESTA
                   (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                    ORDEN, NOMBRE, DESCRIPCION, LONGITUD, OBLIGATORIO, 
                    NO_COLUMNAS, ID_TIPO_CAMPO, ACTIVO, ID_CAMPO_PADRE)
                 Values
                   (@IdEntidad, @IdProceso, 1, 1, 3, 
                    3, 'Número de Folios', 'Cantidad de folios que contiene el documento al registrarse al sistema', 10, 0, 
                    3, 3, 1, NULL)
                 INSERT INTO PROCESO_PLANTILLA_SECCION_CAMPO_PROPUESTA
                   (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                    ORDEN, NOMBRE, DESCRIPCION, LONGITUD, OBLIGATORIO, 
                    NO_COLUMNAS, ID_TIPO_CAMPO, ACTIVO, ID_CAMPO_PADRE)
                 Values
                   (@IdEntidad, @IdProceso, 1, 1, 4, 
                    4, 'Fecha del Documento', 'Fecha de creación registrada en el documento', 10, 0, 
                    3, 4, 1, NULL)
                 INSERT INTO PROCESO_PLANTILLA_SECCION_CAMPO_PROPUESTA
                   (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                    ORDEN, NOMBRE, DESCRIPCION, LONGITUD, OBLIGATORIO, 
                    NO_COLUMNAS, ID_TIPO_CAMPO, ACTIVO, ID_CAMPO_PADRE)
                 Values
                   (@IdEntidad, @IdProceso, 1, 1, 5, 
                    5, 'Fecha Límite de Atención', 'Fecha límite en la que debe ser atendido el expediente', 10, 0, 
                    3, 4, 1, NULL)

                ---------  Creación de la Lista de Valores Propuesta -----------
                     INSERT INTO PROCESO_LISTA_VALORES_PROPUESTA
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        1, NULL, NULL, 0, 'Acta', 
                        1)
                     INSERT INTO PROCESO_LISTA_VALORES_PROPUESTA
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        2, NULL, NULL, 0, 'Acuerdo', 
                        2)
                     INSERT INTO PROCESO_LISTA_VALORES_PROPUESTA
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        3, NULL, NULL, 0, 'Denuncia', 
                        3)
                     INSERT INTO PROCESO_LISTA_VALORES_PROPUESTA
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        4, NULL, NULL, 0, 'Fax', 
                        4)
                     INSERT INTO PROCESO_LISTA_VALORES_PROPUESTA
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        5, NULL, NULL, 0, 'Oficio', 
                        5)
                     INSERT INTO PROCESO_LISTA_VALORES_PROPUESTA
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        6, NULL, NULL, 0, 'Providencia', 
                        6)
                     INSERT INTO PROCESO_LISTA_VALORES_PROPUESTA
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        7, NULL, NULL, 0, 'Memorándum', 
                        7)
                     INSERT INTO PROCESO_LISTA_VALORES_PROPUESTA
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        9, NULL, NULL, 0, 'Circular', 
                        8)
                     INSERT INTO PROCESO_LISTA_VALORES_PROPUESTA
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        10, NULL, NULL, 0, 'Otro', 
                        9)					

                ---------  Creación de los Campos  -----------
			      INSERT INTO PROCESO_PLANTILLA_SECCION_CAMPO
                   (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                    ORDEN, NOMBRE, DESCRIPCION, LONGITUD, OBLIGATORIO, 
                    NO_COLUMNAS, ID_TIPO_CAMPO, ACTIVO, ID_CAMPO_PADRE)
                 Values
                   (@IdEntidad, @IdProceso, 1, 1, 1, 
                    1, 'Número Original del Documento', 'Número que identifica al documento', 30, 0, 
                    4, 1, 0, NULL)
                 INSERT INTO PROCESO_PLANTILLA_SECCION_CAMPO
                   (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                    ORDEN, NOMBRE, DESCRIPCION, LONGITUD, OBLIGATORIO, 
                    NO_COLUMNAS, ID_TIPO_CAMPO, ACTIVO, ID_CAMPO_PADRE)
                 Values
                   (@IdEntidad, @IdProceso, 1, 1, 2, 
                    2, 'Tipo de Documento', 'Listado de diferentes tipos de documento que pueden presentarse', 10, 0, 
                    3, 6, 0, NULL)
                 INSERT INTO PROCESO_PLANTILLA_SECCION_CAMPO
                   (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                    ORDEN, NOMBRE, DESCRIPCION, LONGITUD, OBLIGATORIO, 
                    NO_COLUMNAS, ID_TIPO_CAMPO, ACTIVO, ID_CAMPO_PADRE)
                 Values
                   (@IdEntidad, @IdProceso, 1, 1, 3, 
                    3, 'Número de Folios', 'Cantidad de folios que contiene el documento al registrarse al sistema', 10, 0, 
                    3, 3, 0, NULL)
                 INSERT INTO PROCESO_PLANTILLA_SECCION_CAMPO
                   (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                    ORDEN, NOMBRE, DESCRIPCION, LONGITUD, OBLIGATORIO, 
                    NO_COLUMNAS, ID_TIPO_CAMPO, ACTIVO, ID_CAMPO_PADRE)
                 Values
                   (@IdEntidad, @IdProceso, 1, 1, 4, 
                    4, 'Fecha del Documento', 'Fecha de creación registrada en el documento', 10, 0, 
                    3, 4, 0, NULL)
                 INSERT INTO PROCESO_PLANTILLA_SECCION_CAMPO
                   (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                    ORDEN, NOMBRE, DESCRIPCION, LONGITUD, OBLIGATORIO, 
                    NO_COLUMNAS, ID_TIPO_CAMPO, ACTIVO, ID_CAMPO_PADRE)
                 Values
                   (@IdEntidad, @IdProceso, 1, 1, 5, 
                    5, 'Fecha Límite de Atención', 'Fecha límite en la que debe ser atendido el expediente', 10, 0, 
                    3, 4, 0, NULL)

                ---------  Creación de la Lista de Valores -----------
                     INSERT INTO PROCESO_LISTA_VALORES
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        1, NULL, NULL, 0, 'Acta', 
                        1)
                     INSERT INTO PROCESO_LISTA_VALORES
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        2, NULL, NULL, 0, 'Acuerdo', 
                        2)
                     INSERT INTO PROCESO_LISTA_VALORES
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        3, NULL, NULL, 0, 'Denuncia', 
                        3)
                     INSERT INTO PROCESO_LISTA_VALORES
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        4, NULL, NULL, 0, 'Fax', 
                        4)
                     INSERT INTO PROCESO_LISTA_VALORES
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        5, NULL, NULL, 0, 'Oficio', 
                        5)
                     INSERT INTO PROCESO_LISTA_VALORES
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        6, NULL, NULL, 0, 'Providencia', 
                        6)
                     INSERT INTO PROCESO_LISTA_VALORES
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        7, NULL, NULL, 0, 'Memorándum', 
                        7)
                     INSERT INTO PROCESO_LISTA_VALORES
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        9, NULL, NULL, 0, 'Circular', 
                        8)
                     INSERT INTO PROCESO_LISTA_VALORES
                       (ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, 
                        ID_VALOR, ID_CAMPO_PADRE, ID_VALOR_PADRE, PREDETERMINADO, NOMBRE, 
                        ORDEN)
                     Values
                       (@IdEntidad, @IdProceso, 1, 1, 2, 
                        10, NULL, NULL, 0, 'Otro', 
                        9)";
            
            using (var connection = await connectionProvider.OpenAsync())
            {

                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@IdProceso", proceso.IdProceso);
                    param.Add("@IdEntidad", proceso.IdEntidad);
                    param.Add("@IdUnidadAdministrativa", proceso.IdUnidadAdministrativa);
                    param.Add("@Nombre", proceso.Nombre);
                    param.Add("@Descripcion", proceso.Descripcion);
                    param.Add("@IdTipoProceso", proceso.IdTipoProceso);
                    param.Add("@Activo", 0);
                    param.Add("@Color", proceso.Color);
                    param.Add("@FechaRegistro", fechaRegistro);
                    param.Add("@UsuarioRegistro", idUsuarioRegistro);

                    proceso.IdProceso = connection.QuerySingle<int>(insertarProcesoSQL, param, trx);

                    await connection.ExecuteAsync(insertarPlantillaSQL, new { proceso.IdEntidad, proceso.IdProceso, FechaRegistro = fechaRegistro, UsuarioRegistro = idUsuarioRegistro }, trx);

                    //await connection.ExecuteAsync(insertarSeccionSQL, new { proceso.IdEntidad, proceso.IdProceso }, trx);

                    if (proceso.IdTipoProceso == TipoProceso.Standard)
                    {
                        await connection.ExecuteAsync(plantillaStandard, new { proceso.IdEntidad, proceso.IdProceso }, trx);
                    }

                    trx.Commit();
                }
            }

            return proceso.IdProceso;

        }
        public async Task<int> ActualizarProcesoAsync(ProcesoModelo proceso)
        {
            const string actualizarSQL = @"                
                UPDATE PROCESO SET
                    ID_UNIDAD_ADMINISTRATIVA = @IdUnidadAdministrativa, 
                    NOMBRE = @Nombre,
                    DESCRIPCION = @Descripcion, 
                    TIPO_PROCESO = @IdTipoProceso,
                    ACTIVO = @Activo,
                    COLOR =  @Color
                WHERE
                    ID_ENTIDAD = @IdEntidad AND
                    ID_PROCESO = @IdProceso";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var param = new DynamicParameters();
                param.Add("@IdProceso", proceso.IdProceso);
                param.Add("@IdEntidad", proceso.IdEntidad);
                param.Add("@IdUnidadAdministrativa", proceso.IdUnidadAdministrativa);
                param.Add("@Nombre", proceso.Nombre);
                param.Add("@Descripcion", proceso.Descripcion);
                param.Add("@IdTipoProceso", proceso.IdTipoProceso);
                param.Add("@Activo", Convert.ToInt32(proceso.Estado));
                param.Add("@Color", proceso.Color);

                var response = await connection.ExecuteAsync(actualizarSQL, param);

                return response;
            }
        }
        public async Task<int> EliminarProcesoAsync(int idEntidad, int idProceso)
        {
            const string eliminarProcesoSQL = @"                
                DELETE PROCESO WHERE ID_ENTIDAD = @IdEntidad AND ID_PROCESO = @IdProceso";

            using (var connection = await connectionProvider.OpenAsync())
            {

                var response = await connection.ExecuteAsync(eliminarProcesoSQL, new { idEntidad, idProceso });

                return response;
            }

        }
        public async Task<ProcesoModelo> ObtenerProcesoAsync(int idEntidad, int idProceso)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                //TODO: incluir datos para pagineo 
                string sqlQuery = @"
                SELECT 
                    ex.id_entidad,
                    ex.id_proceso,
                    CASE
                        WHEN pf.id_tipo_fase IN (1, 2) THEN 1
                        WHEN pf.id_tipo_fase = 3 THEN 2
                    END AS tipo,
                    COUNT(1) AS cantidad

                    INTO #Totales
                FROM 
                    exp_expediente ex
                    INNER JOIN proceso_fase pf ON pf.id_fase = ex.id_fase_actual
                        AND pf.id_proceso = ex.id_proceso
                        AND pf.id_entidad = ex.id_entidad
                WHERE 
                    ex.id_entidad = @idEntidad
                    AND ex.id_proceso = @idProceso
                GROUP BY 
                    ex.id_entidad, ex.id_proceso, 
                    CASE
                        WHEN pf.id_tipo_fase IN (1, 2) THEN 1
                        WHEN pf.id_tipo_fase = 3 THEN 2
                    END


                SELECT 
                    pr.id_proceso AS idProceso,
                    pr.nombre,
                    pr.descripcion,
                    pr.color,
                    pr.tipo_proceso AS idTipoProceso,
                    CASE pr.tipo_proceso
                        WHEN 0 THEN 'Proceso Estandar'
                        WHEN 1 THEN 'Proceso Personalizado'
                    END AS tipoproceso,
                    ua.id_unidad_administrativa AS idUnidadAdministrativa,
                    ua.nombre AS unidadadministrativa,
                    ua.siglas AS siglasua,
                    pr.activo AS estado,
                    t1.cantidad AS expedientesActivos,
                    t2.cantidad AS expedientesFinalizados,
                    (ISNULL(t1.cantidad, 0) + ISNULL(t2.cantidad, 0)) AS totalExpedientes
                FROM 
                    proceso pr
                    INNER JOIN ad_unidades_administrativas ua ON ua.id_unidad_administrativa = pr.id_unidad_administrativa
                        AND ua.id_entidad = pr.id_entidad
                    LEFT JOIN #Totales t1 ON t1.id_entidad = pr.id_entidad
                        AND t1.id_proceso = pr.id_proceso
                        AND t1.tipo = 1
                    LEFT JOIN #Totales t2 ON t2.id_entidad = pr.id_entidad
                        AND t2.id_proceso = pr.id_proceso
                        AND t2.tipo = 2
                WHERE 
                    pr.id_entidad = @idEntidad
                    AND pr.id_proceso = @idProceso
                ORDER BY 
                    pr.id_proceso
                ";

                var result = await connection.QueryAsync<ProcesoModelo>(sqlQuery, new { idEntidad, idProceso });
                return result.FirstOrDefault();
            }
        }
        public async Task<bool> ProcesoCompletoAsync(ProcesoModelo proceso)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"
                SELECT COUNT(1)
                FROM proceso_plantilla_seccion ps
                JOIN proceso_plantilla_seccion_campo psc ON psc.id_entidad = ps.id_entidad
                                                         AND psc.id_proceso = ps.id_proceso
                JOIN proceso_fase pf ON pf.id_entidad = psc.id_entidad
                                     AND pf.id_proceso = psc.id_proceso
                WHERE pf.activa = 1
                  AND pf.id_tipo_fase = 1
                  AND ps.id_entidad = @IdEntidad
                  AND ps.id_proceso = @IdProceso
                  AND ps.activa = 1
                  AND psc.activo = 1
                ";

                var result = await connection.ExecuteScalarAsync<int>(sqlQuery, new { proceso.IdEntidad, proceso.IdProceso });

                return result == 0 ? false : true;
            }
        }
        public async Task<List<ProcesoModelo>> ObtenerProcesosActivosAsync(int idEntidad, int idUsuario)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"SELECT DISTINCT pf.id_proceso AS idProceso,
                pf.id_entidad AS idEntidad,
                pr.nombre,
                pr.color
                FROM proceso_fase pf
                JOIN proceso_transicion_usuarios ptu ON ptu.id_fase_origen = pf.id_fase
                    AND ptu.id_proceso = pf.id_proceso
                    AND ptu.id_entidad = pf.id_entidad
                JOIN proceso pr ON pr.id_proceso = ptu.id_proceso
                    AND pr.id_entidad = ptu.id_entidad
                WHERE ptu.id_usuario = @idUsuario
                  AND pf.id_entidad = @idEntidad
                  AND pf.id_tipo_fase = 1
                  AND pr.activo = 1
                ";

                var result = await connection.QueryAsync<ProcesoModelo>(sqlQuery, new { idEntidad, idUsuario });
                return result.ToList();
            }
        }

    }
}