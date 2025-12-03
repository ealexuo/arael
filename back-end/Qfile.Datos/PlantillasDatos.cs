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

namespace Qfile.Datos
{
    public class PlantillasDatos : IPlantillasDatos
    {
        private readonly IConnectionProvider connectionProvider;

        public PlantillasDatos(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }
        public async Task<List<PlantillaModelo>> ObtenerPlantillasAsync(int idEntidad, int idProceso)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                const string sqlQuery = @"
                SELECT 
                    pp.id_entidad AS IdEntidad, 
                    pp.id_proceso AS IdProceso, 
                    pp.id_plantilla AS IdPlantilla, 
                    pp.nombre AS Nombre, 
                    pp.descripcion AS Descripcion, 
                    pp.Version, 
                    pp.Version_Propuesta AS VersionPropuesta, 
                    pp.Activa, 
                    pp.id_plantilla AS IdPlantilla2, 
                    pps.id_entidad AS IdEntidad, 
                    pps.id_proceso AS IdProceso, 
                    pps.id_plantilla AS IdPlantilla, 
                    pps.ID_SECCION AS IdSeccion,
                    pps.nombre, 
                    pps.descripcion, 
                    pps.orden, 
                    pps.activa, 
                    pps.ID_SECCION AS IdSeccion2, 
                    ppsc.id_entidad AS IdEntidad, 
                    ppsc.id_proceso AS IdProceso,
                    ppsc.id_plantilla AS IdPlantilla, 
                    ppsc.ID_SECCION AS IdSeccion, 
                    ppsc.id_campo AS IdCampo, 
                    ppsc.nombre, 
                    ppsc.descripcion, 
                    ppsc.orden, 
                    ppsc.longitud,
                    ppsc.obligatorio, 
                    ppsc.id_tipo_campo AS IdTipoCampo, 
                    ppsc.NO_COLUMNAS AS noColumnas, 
                    ppsc.activo, 
                    ISNULL(ppsc.id_campo_padre, 0) AS IdCampoPadre,
                    ppscp.Nombre AS NombreCampoPadre
                FROM 
                    proceso_plantilla pp
                    LEFT JOIN proceso_plantilla_seccion_PROPUESTA pps ON pps.id_entidad = pp.id_entidad
                        AND pps.id_proceso = pp.id_proceso
                        AND pps.id_plantilla = pp.id_plantilla
                    LEFT JOIN proceso_plantilla_seccion_campo_PROPUESTA ppsc ON ppsc.id_entidad = pps.id_entidad
                        AND ppsc.id_proceso = pps.id_proceso
                        AND ppsc.id_plantilla = pps.id_plantilla
                        AND ppsc.id_seccion = pps.id_seccion
                     LEFT JOIN proceso_plantilla_seccion_campo_PROPUESTA ppscp ON ppscp.id_entidad = pps.id_entidad
                        AND ppscp.id_proceso = pps.id_proceso
                        AND ppscp.id_plantilla = pps.id_plantilla
                        AND ppscp.id_seccion = pps.id_seccion
                        AND ppscp.id_campo = ppsc.id_campo_padre
                WHERE 
                    pp.id_entidad = @identidad 
                    AND pp.id_proceso = @idproceso
                ORDER BY 
                    pp.id_plantilla, 
                    pps.orden, 
                    ppsc.orden
                ";

                var plantillaDictionary = new Dictionary<int, PlantillaModelo>();
                var seccionDictionary = new Dictionary<int, SeccionModelo>();
                int i = 0;

                var result = await connection.QueryAsync<PlantillaModelo, SeccionModelo, CampoModelo, PlantillaModelo>(sqlQuery, (p, s, c) =>
                {
                    PlantillaModelo plantilla;

                    if (!plantillaDictionary.TryGetValue(p.IdPlantilla, out plantilla))
                    {
                        plantilla = p;
                        plantilla.ListaSecciones = new List<SeccionModelo>();
                        plantillaDictionary.Add(plantilla.IdPlantilla, plantilla);

                        seccionDictionary = new Dictionary<int, SeccionModelo>();
                    }

                    SeccionModelo seccion;

                    if (!seccionDictionary.TryGetValue(s.IdSeccion, out seccion))
                    {
                        seccion = s;
                        seccion.ListaCampos = new List<CampoModelo>();
                        seccionDictionary.Add(seccion.IdSeccion, seccion);
                    }

                    if (c != null && c.IdCampo != 0)
                    {
                        seccion.ListaCampos.AsList().Add(c);
                    }

                    if (s != null && s.IdSeccion != 0 && !ExisteSeccion(s.IdSeccion, plantilla.ListaSecciones))
                    {
                        plantilla.ListaSecciones.AsList().Add(s);
                    }

                    return plantilla;

                }, new { idEntidad, idProceso }, splitOn: "IdPlantilla2,IdSeccion2");

                return result.Distinct().ToList();
            }
        }
        private bool ExisteSeccion(int idSeccion, List<SeccionModelo> secciones)
        {

            foreach (var item in secciones)
            {
                if (item.IdSeccion == idSeccion)
                    return true;
            }
            return false;
        }
        public async Task<int> CrearPlantillaAsync(PlantillaModelo plantilla, int idUsuarioRegistro, DateTime fechaRegistro)
        {

            const string insertarSeccionSQL = @"                
                INSERT INTO PROCESO_PLANTILLA
                (
                    ID_ENTIDAD, 
                    ID_PROCESO, 
                    ID_PLANTILLA, 
                    NOMBRE, 
                    DESCRIPCION, 
                    VERSION, 
                    VERSION_PROPUESTA, 
                    ACTIVA, 
                    FECHA_REGISTRO, 
                    USUARIO_REGISTRO
                )
                OUTPUT INSERTED.ID_PLANTILLA
                VALUES(
                    @IdEntidad, 
                    @IdProceso, 
                    (select ISNULL(MAX(id_plantilla), 0) + 1 from proceso_plantilla where id_entidad = @IdEntidad and id_proceso = @IdProceso), 
                    @Nombre, 
                    @Descripcion, 
                    @Version, 
                    @VersionPropuesta, 
                    @Activa, 
                    @FechaRegistro, 
                    @UsuarioRegistro
                )";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@IdEntidad", plantilla.IdEntidad);
                    param.Add("@IdProceso", plantilla.IdProceso);
                    param.Add("@Nombre", plantilla.Nombre);
                    param.Add("@Descripcion", plantilla.Descripcion);
                    param.Add("@Version", 0);
                    param.Add("@VersionPropuesta", 1);
                    param.Add("@Activa", plantilla.Activa);
                    param.Add("@FechaRegistro", fechaRegistro);
                    param.Add("@UsuarioRegistro", idUsuarioRegistro);

                    plantilla.IdPlantilla = connection.QuerySingle<int>(insertarSeccionSQL, param, trx);

                    trx.Commit();
                }                
            }

            return plantilla.IdPlantilla;
        }
        public async Task<int> ActualizarPlantillaAsync(PlantillaModelo plantilla)
        {
            const string actualizarPlantillaSQL = @"                
                UPDATE PROCESO_PLANTILLA SET
                    NOMBRE = @Nombre,
                    DESCRIPCION = @Descripcion, 
                    ACTIVA = @Activa
                WHERE
                    ID_ENTIDAD = @IdEntidad
                    AND ID_PROCESO = @IdProceso
                    AND ID_PLANTILLA = @IdPlantilla";

            const string inactivarPlantillasSQL = @"                
                UPDATE PROCESO_PLANTILLA SET
                    ACTIVA = 0
                WHERE
                    ID_ENTIDAD = @IdEntidad
                    AND ID_PROCESO = @IdProceso
                    AND ACTIVA = 1";

            int response = 0;

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@IdEntidad", Convert.ToInt32(plantilla.IdEntidad));
                    param.Add("@IdProceso", Convert.ToInt32(plantilla.IdProceso));
                    param.Add("@IdPlantilla", Convert.ToInt32(plantilla.IdPlantilla));
                    param.Add("@Nombre", plantilla.Nombre);
                    param.Add("@Descripcion", plantilla.Descripcion);
                    param.Add("@Activa", Convert.ToInt32(plantilla.Activa));

                    if (plantilla.Activa)
                        response = await connection.ExecuteAsync(inactivarPlantillasSQL, new { plantilla.IdEntidad, plantilla.IdProceso }, trx);

                    response = await connection.ExecuteAsync(actualizarPlantillaSQL, param, trx);

                    trx.Commit();
                }

                return response;
            }

        }
        public async Task<int> EliminarPlantillaAsync(int idEntidad, int idProceso, int idPlantilla)
        {
            const string eliminarPlantillaSQL = @"                
                DELETE PROCESO_PLANTILLA 
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla";

            using (var connection = await connectionProvider.OpenAsync())
            {

                var response = await connection.ExecuteAsync(eliminarPlantillaSQL, new { idEntidad, idProceso, idPlantilla });

                return response;
            }

        }
        public async Task<int> CrearSeccionAsync(SeccionModelo seccion, DateTime fechaRegistro, int usuarioRegistro)
        {
            const string insertarSeccionSQL = @"                
                INSERT INTO PROCESO_PLANTILLA_SECCION_PROPUESTA
                (
                    ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, NOMBRE, DESCRIPCION, ORDEN, ACTIVA
                )
                OUTPUT INSERTED.ID_SECCION
                VALUES(
                    @IdEntidad, @IdProceso, @IdPlantilla, 
                    (select ISNULL(MAX(id_seccion), 0) + 1 
                    from proceso_plantilla_seccion_PROPUESTA 
                    where id_entidad = @IdEntidad
                    and id_proceso = @IdProceso
                    and id_plantilla = @IdPlantilla), 
                    @Nombre, @Descripcion, @Orden, @Activa
                )";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@IdEntidad", seccion.IdEntidad);
                    param.Add("@IdProceso", seccion.IdProceso);
                    param.Add("@IdPlantilla", seccion.IdPlantilla);
                    param.Add("@IdSeccion", seccion.IdSeccion);
                    param.Add("@Nombre", seccion.Nombre);
                    param.Add("@Descripcion", seccion.Descripcion);
                    param.Add("@Orden", seccion.Orden);
                    param.Add("@Activa", 0);

                    var p = await ObtenerVersionesPlantilla(seccion.IdEntidad, seccion.IdProceso, seccion.IdPlantilla);

                    if (p.Version == p.VersionPropuesta)
                    {
                        decimal versionPropuesta = ObtenerVersion(p.Version);

                        await CrearPlantillaPropuesta(seccion.IdEntidad, seccion.IdProceso, seccion.IdPlantilla, versionPropuesta, fechaRegistro, usuarioRegistro, connection, trx);
                    }

                    await connection.ExecuteAsync(insertarSeccionSQL, param, trx);

                    seccion.IdSeccion = param.Get<int>("IdSeccion");

                    trx.Commit();
                }
            }

            return seccion.IdSeccion;
        }
        public async Task<int> ActualizarSeccionAsync(SeccionModelo seccion, DateTime fechaRegistro, int usuarioRegistro)
        {
            int response;

            const string actualizarSeccionSQL = @"                
                UPDATE PROCESO_PLANTILLA_SECCION_PROPUESTA SET
                    NOMBRE = @Nombre,
                    DESCRIPCION = @Descripcion, 
                    ORDEN = @Orden,
                    ACTIVA = @Activa
                WHERE
                    ID_ENTIDAD = @IdEntidad
                    AND ID_PROCESO = @IdProceso
                    AND ID_PLANTILLA = @IdPlantilla
                    AND ID_SECCION = @IdSeccion";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@IdEntidad", Convert.ToInt32(seccion.IdEntidad));
                    param.Add("@IdProceso", Convert.ToInt32(seccion.IdProceso));
                    param.Add("@IdPlantilla", Convert.ToInt32(seccion.IdPlantilla));
                    param.Add("@IdSeccion", Convert.ToInt32(seccion.IdSeccion));
                    param.Add("@Nombre", seccion.Nombre);
                    param.Add("@Descripcion", seccion.Descripcion);
                    param.Add("@Orden", Convert.ToInt32(seccion.Orden));
                    param.Add("@Activa", Convert.ToInt32(seccion.Activa));

                    var p = await ObtenerVersionesPlantilla(seccion.IdEntidad, seccion.IdProceso, seccion.IdPlantilla);

                    if (p.Version == p.VersionPropuesta)
                    {
                        decimal versionPropuesta = ObtenerVersion(p.Version);

                        await CrearPlantillaPropuesta(seccion.IdEntidad, seccion.IdProceso, seccion.IdPlantilla, versionPropuesta, fechaRegistro, usuarioRegistro, connection, trx);
                    }

                    response = await connection.ExecuteAsync(actualizarSeccionSQL, param, trx);

                    trx.Commit();
                }

                return response;
            }

        }
        public async Task<int> EliminarSeccionAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, DateTime fechaRegistro, int usuarioRegistro)
        {
            const string eliminarSeccionSQL = @"                
                DELETE PROCESO_PLANTILLA_SECCION_PROPUESTA
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla 
                AND ID_SECCION = @IdSeccion";

            int response;

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var p = await ObtenerVersionesPlantilla(idEntidad, idProceso, idPlantilla);

                    if (p.Version == p.VersionPropuesta)
                    {
                        decimal versionPropuesta = ObtenerVersion(p.Version);

                        await CrearPlantillaPropuesta(idEntidad, idProceso, idPlantilla, versionPropuesta, fechaRegistro, usuarioRegistro, connection, trx);
                    }

                    response = await connection.ExecuteAsync(eliminarSeccionSQL, new { idEntidad, idProceso, idPlantilla, idSeccion }, trx);

                    trx.Commit();
                }
            }

            return response;
        }
        public async Task<int> EliminarSeccionesAsync(int idEntidad, int idProceso, int idPlantilla, DateTime fechaRegistro, int usuarioRegistro)
        {
            const string eliminarSeccionSQL = @"                
                DELETE PROCESO_PLANTILLA_SECCION_PROPUESTA
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla";

            int response;

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var p = await ObtenerVersionesPlantilla(idEntidad, idProceso, idPlantilla);

                    if (p.Version == p.VersionPropuesta)
                    {
                        decimal versionPropuesta = ObtenerVersion(p.Version);

                        await CrearPlantillaPropuesta(idEntidad, idProceso, idPlantilla, versionPropuesta, fechaRegistro, usuarioRegistro, connection, trx);
                    }

                    response = await connection.ExecuteAsync(eliminarSeccionSQL, new { idEntidad, idProceso, idPlantilla }, trx);

                    trx.Commit();
                }
            }

            return response;
        }

        public async Task<bool> SeccionTieneCampos(int idEntidad, int idProceso, int idPlantilla, int idSeccion)
        {
            const string camposSeccionSql = @"                
                SELECT COUNT(*) 
                FROM PROCESO_PLANTILLA_SECCION_CAMPO_PROPUESTA
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla 
                AND ID_SECCION = @IdSeccion";

            int response;

            using (var connection = await connectionProvider.OpenAsync())
            {
                response = await connection.ExecuteScalarAsync<int>(camposSeccionSql, new { idEntidad, idProceso, idPlantilla, idSeccion });
            }

            return response > 0;
        }

        public async Task<bool> ExistenSeccionesConCampos(int idEntidad, int idProceso, int idPlantilla)
        {
            const string camposSeccionSql = @"                
                SELECT COUNT(*) 
                FROM PROCESO_PLANTILLA_SECCION_CAMPO_PROPUESTA
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla";

            int response;

            using (var connection = await connectionProvider.OpenAsync())
            {
                response = await connection.ExecuteScalarAsync<int>(camposSeccionSql, new { idEntidad, idProceso, idPlantilla });
            }

            return response > 0;
        }


        public async Task<List<TipoCampoModelo>> ObtenerTiposCampoAsync()
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                const string sqlQuery = @"SELECT id_tipo_campo idTipoCampo, nombre from CA_TIPO_CAMPO";

                var result = await connection.QueryAsync<TipoCampoModelo>(sqlQuery);

                return result.ToList();
            }
        }
        public async Task<int> CrearCampoAsync(CampoModelo campo, DateTime fechaRegistro, int usuarioRegistro)
        {
            const string insertarCampoSQL = @"                
                INSERT INTO PROCESO_PLANTILLA_SECCION_CAMPO_PROPUESTA
                (
                    ID_ENTIDAD, ID_PROCESO, ID_PLANTILLA, ID_SECCION, ID_CAMPO, ORDEN, NOMBRE, DESCRIPCION, LONGITUD, OBLIGATORIO, NO_COLUMNAS, ID_TIPO_CAMPO, ACTIVO, ID_CAMPO_PADRE
                )
                OUTPUT INSERTED.ID_CAMPO
                VALUES(
                    @IdEntidad, @IdProceso, @IdPlantilla, @IdSeccion,
                    (select ISNULL(MAX(id_campo), 0) + 1 
                    from proceso_plantilla_seccion_campo_PROPUESTA
                    where id_entidad = @IdEntidad
                    and id_proceso = @IdProceso
                    and id_plantilla = @IdPlantilla
                    and id_seccion = @IdSeccion), 
                    @Orden, @Nombre, @Descripcion, @Longitud,
                    @Obligatorio, @NoColumnas,
                    @IdTipoCampo, @Activo,
                    IIF(@IdCampoPadre = 0, null, @IdCampoPadre)
                )";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@IdEntidad", campo.IdEntidad);
                    param.Add("@IdProceso", campo.IdProceso);
                    param.Add("@IdPlantilla", campo.IdPlantilla);
                    param.Add("@IdSeccion", campo.IdSeccion);
                    param.Add("@IdCampo", campo.IdCampo);
                    param.Add("@Orden", campo.Orden);
                    param.Add("@Nombre", campo.Nombre);
                    param.Add("@Descripcion", campo.Descripcion);
                    param.Add("@Longitud", campo.Longitud);
                    param.Add("@Obligatorio", campo.Obligatorio);
                    param.Add("@NoColumnas", campo.NoColumnas);
                    param.Add("@IdTipoCampo", campo.IdTipoCampo);
                    param.Add("@Activo", campo.Activo);
                    param.Add("@IdCampoPadre", campo.IdCampoPadre);

                    var p = await ObtenerVersionesPlantilla(campo.IdEntidad, campo.IdProceso, campo.IdPlantilla);

                    if (p.Version == p.VersionPropuesta)
                    {
                        decimal versionPropuesta = ObtenerVersion(p.Version);

                        await CrearPlantillaPropuesta(campo.IdEntidad, campo.IdProceso, campo.IdPlantilla, versionPropuesta, fechaRegistro, usuarioRegistro, connection, trx);
                    }

                    await connection.ExecuteAsync(insertarCampoSQL, param, trx);

                    trx.Commit();

                    campo.IdCampo = param.Get<int>("IdCampo");
                }
            }

            return campo.IdCampo;
        }
        public async Task<int> ActualizarCampoAsync(CampoModelo campo, DateTime fechaRegistro, int usuarioRegistro)
        {
            const string actualizarCampoSQL = @"                
                UPDATE PROCESO_PLANTILLA_SECCION_CAMPO_PROPUESTA SET
                    ORDEN = @Orden,
                    NOMBRE = @Nombre,
                    DESCRIPCION = @Descripcion, 
                    LONGITUD = @Longitud,
                    OBLIGATORIO = @Obligatorio,
                    NO_COLUMNAS = @NoColumnas,
                    ID_TIPO_CAMPO = @IdTipoCampo,
                    ACTIVO = @Activo,
                    ID_CAMPO_PADRE = IIF(@IdCampoPadre = 0, null, @IdCampoPadre)
                WHERE
                    ID_ENTIDAD = @IdEntidad
                    AND ID_PROCESO = @IdProceso
                    AND ID_PLANTILLA = @IdPlantilla
                    AND ID_SECCION = @IdSeccion
                    AND ID_CAMPO = @IdCampo";

            int response;

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@IdEntidad", campo.IdEntidad);
                    param.Add("@IdProceso", campo.IdProceso);
                    param.Add("@IdPlantilla", campo.IdPlantilla);
                    param.Add("@IdSeccion", campo.IdSeccion);
                    param.Add("@IdCampo", campo.IdCampo);
                    param.Add("@Orden", campo.Orden);
                    param.Add("@Nombre", campo.Nombre);
                    param.Add("@Descripcion", campo.Descripcion);
                    param.Add("@Longitud", campo.Longitud);
                    param.Add("@Obligatorio", campo.Obligatorio ? 1 : 0);
                    param.Add("@NoColumnas", campo.NoColumnas);
                    param.Add("@IdTipoCampo", campo.IdTipoCampo);
                    param.Add("@Activo", campo.Activo ? 1 : 0);
                    param.Add("@IdCampoPadre", campo.IdCampoPadre);

                    var p = await ObtenerVersionesPlantilla(campo.IdEntidad, campo.IdProceso, campo.IdPlantilla);

                    if (p.Version == p.VersionPropuesta)
                    {
                        decimal versionPropuesta = ObtenerVersion(p.Version);

                        await CrearPlantillaPropuesta(campo.IdEntidad, campo.IdProceso, campo.IdPlantilla, versionPropuesta, fechaRegistro, usuarioRegistro, connection, trx);
                    }

                    response = await connection.ExecuteAsync(actualizarCampoSQL, param, trx);

                    trx.Commit();

                }
                return response;
            }

        }
        public async Task<int> EliminarCampoAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, DateTime fechaRegistro, int usuarioRegistro)
        {
            const string eliminarCampoSQL = @"                
                DELETE PROCESO_PLANTILLA_SECCION_CAMPO_PROPUESTA 
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla 
                AND ID_SECCION = @IdSeccion
                AND ID_CAMPO = @IdCampo";

            const string selctCampoSQL = @"                
                SELECT id_entidad idEntidad, id_proceso idProceso, id_Plantilla idPlantilla, id_seccion idSeccion, id_campo idCampo, orden, nombre
                FROM PROCESO_PLANTILLA_SECCION_CAMPO_PROPUESTA 
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla 
                AND ID_SECCION = @IdSeccion
                order by orden";

            const string updateCampoSQL = @"                
                UPDATE PROCESO_PLANTILLA_SECCION_CAMPO_PROPUESTA SET ORDEN = @Orden
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla 
                AND ID_SECCION = @IdSeccion
                AND ID_CAMPO = @IdCampo";

            int response;

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var p = await ObtenerVersionesPlantilla(idEntidad, idProceso, idPlantilla);

                    if (p.Version == p.VersionPropuesta)
                    {
                        decimal versionPropuesta = ObtenerVersion(p.Version);

                        await CrearPlantillaPropuesta(idEntidad, idProceso, idPlantilla, versionPropuesta, fechaRegistro, usuarioRegistro, connection, trx);
                    }

                  response = await connection.ExecuteAsync(eliminarCampoSQL, new { idEntidad, idProceso, idPlantilla, idSeccion, idCampo }, trx);

                    var campos = await connection.QueryAsync<CampoModelo>(selctCampoSQL, new { idEntidad, idProceso, idPlantilla, idSeccion }, trx);

                    int i = 1;
                    foreach (var campo in campos.ToList())
                    {
                        if (campo.Orden != i)
                        {
                            var r = await connection.ExecuteAsync(updateCampoSQL, new { idEntidad, idProceso, idPlantilla, idSeccion, campo.IdCampo, Orden = i }, trx);
                        }
                        i++;
                    }

                    trx.Commit();
                }

            }
            return response;
        }
        public async Task<int> EliminarCamposAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, DateTime fechaRegistro, int usuarioRegistro)
        {
            const string eliminarCampoSQL = @"                
                DELETE PROCESO_PLANTILLA_SECCION_CAMPO_PROPUESTA
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla
                AND ID_SECCION = @IdSeccion";

            int response;

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var p = await ObtenerVersionesPlantilla(idEntidad, idProceso, idPlantilla);

                    if (p.Version == p.VersionPropuesta)
                    {
                        decimal versionPropuesta = ObtenerVersion(p.Version);

                        await CrearPlantillaPropuesta(idEntidad, idProceso, idPlantilla, versionPropuesta, fechaRegistro, usuarioRegistro, connection, trx);
                    }

                    response = await connection.ExecuteAsync(eliminarCampoSQL, new { idEntidad, idProceso, idPlantilla, idSeccion }, trx);

                    trx.Commit();
                }
            }
            return response;
        }

        public async Task<bool> CampoTieneValores(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo)
        {
            const string valoresCampoSql = @"                
                SELECT COUNT(*) 
                FROM PROCESO_LISTA_VALORES_PROPUESTA
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla 
                AND ID_SECCION = @IdSeccion
                AND ID_CAMPO = @IdCampo";

            int response;

            using (var connection = await connectionProvider.OpenAsync())
            {
                response = await connection.ExecuteScalarAsync<int>(valoresCampoSql, new { idEntidad, idProceso, idPlantilla, idSeccion, idCampo });
            }

            return response > 0;
        }

        public async Task<bool> ExistenCamposConValores(int idEntidad, int idProceso, int idPlantilla, int idSeccion)
        {
            const string valoresCampoSql = @"                
                SELECT COUNT(*)
                FROM PROCESO_LISTA_VALORES_PROPUESTA
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla 
                AND ID_SECCION = @IdSeccion";

            int response;

            using (var connection = await connectionProvider.OpenAsync())
            {
                response = await connection.ExecuteScalarAsync<int>(valoresCampoSql, new { idEntidad, idProceso, idPlantilla, idSeccion });
            }

            return response > 0;
        }

        public async Task<List<PlantillaModelo>> ObtenerPlantillaActualAsync(int idEntidad, int idProceso, int idPlantilla)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                const string sqlQuery = @"
                SELECT 
                    pp.id_entidad AS IdEntidad, 
                    pp.id_proceso AS IdProceso, 
                    pp.id_plantilla AS IdPlantilla, 
                    pp.nombre AS Nombre, 
                    pp.descripcion AS Descripcion, 
                    pp.Version, 
                    pp.Version_Propuesta AS VersionPropuesta, 
                    pp.Activa, 
                    pp.id_plantilla AS IdPlantilla2, 
                    pps.id_entidad AS IdEntidad, 
                    pps.id_proceso AS IdProceso, 
                    pps.id_plantilla AS IdPlantilla, 
                    pps.ID_SECCION AS IdSeccion,
                    pps.nombre, 
                    pps.descripcion, 
                    pps.orden, 
                    pps.activa, 
                    pps.ID_SECCION AS IdSeccion2, 
                    ppsc.id_entidad AS IdEntidad, 
                    ppsc.id_proceso AS IdProceso,
                    ppsc.id_plantilla AS IdPlantilla, 
                    ppsc.ID_SECCION AS IdSeccion, 
                    ppsc.id_campo AS IdCampo, 
                    ppsc.nombre, 
                    ppsc.descripcion, 
                    ppsc.orden, 
                    ppsc.longitud,
                    ppsc.obligatorio, 
                    ppsc.id_tipo_campo AS IdTipoCampo, 
                    ppsc.NO_COLUMNAS AS noColumnas, 
                    ppsc.activo, 
                    ISNULL(ppsc.id_campo_padre, 0) AS IdCampoPadre
                FROM 
                    proceso_plantilla pp
                    LEFT JOIN proceso_plantilla_seccion pps ON pps.id_entidad = pp.id_entidad
                        AND pps.id_proceso = pp.id_proceso
                        AND pps.id_plantilla = pp.id_plantilla
                    LEFT JOIN proceso_plantilla_seccion_campo ppsc ON ppsc.id_entidad = pps.id_entidad
                        AND ppsc.id_proceso = pps.id_proceso
                        AND ppsc.id_plantilla = pps.id_plantilla
                        AND ppsc.id_seccion = pps.id_seccion
                WHERE 
                    pp.id_entidad = @identidad 
                    AND pp.id_proceso = @idproceso
                    AND pp.id_plantilla = @idplantilla
                ORDER BY 
                    pp.id_plantilla, 
                    pps.orden, 
                    ppsc.orden
                ";

                var plantillaDictionary = new Dictionary<int, PlantillaModelo>();
                var seccionDictionary = new Dictionary<int, SeccionModelo>();
                int i = 0;

                var result = await connection.QueryAsync<PlantillaModelo, SeccionModelo, CampoModelo, PlantillaModelo>(sqlQuery, (p, s, c) =>
                {
                    PlantillaModelo plantilla;

                    if (!plantillaDictionary.TryGetValue(idPlantilla, out plantilla))
                    {
                        plantilla = p;
                        plantilla.ListaSecciones = new List<SeccionModelo>();
                        plantillaDictionary.Add(plantilla.IdPlantilla, plantilla);

                        seccionDictionary = new Dictionary<int, SeccionModelo>();
                    }

                    SeccionModelo seccion;

                    if (!seccionDictionary.TryGetValue(s.IdSeccion, out seccion))
                    {
                        seccion = s;
                        seccion.ListaCampos = new List<CampoModelo>();
                        seccionDictionary.Add(seccion.IdSeccion, seccion);
                    }

                    if (c != null && c.IdCampo != 0)
                    {
                        seccion.ListaCampos.AsList().Add(c);
                    }

                    if (s != null && s.IdSeccion != 0 && !ExisteSeccion(s.IdSeccion, plantilla.ListaSecciones))
                    {
                        plantilla.ListaSecciones.AsList().Add(s);
                    }

                    return plantilla;

                }, new { idEntidad, idProceso, idPlantilla }, splitOn: "IdPlantilla2,IdSeccion2");

                return result.Distinct().ToList();
            }
        }
        private async Task<PlantillaModelo> ObtenerVersionesPlantilla(int idEntidad, int idProceso, int idPlantilla)
        {

            using (var connection = await connectionProvider.OpenAsync())
            {
                const string sqlQuery = @"SELECT version, version_propuesta versionPropuesta
                                          FROM proceso_plantilla aa
                                         WHERE id_entidad = @identidad
                                           AND id_proceso = @idproceso
                                           AND id_plantilla = @idplantilla";

                var result = await connection.QueryAsync<PlantillaModelo>(sqlQuery, new { idEntidad, idProceso, idPlantilla });

                return result.FirstOrDefault();
            }
        }
        private async Task<int> CrearPlantillaPropuesta(int idEntidad, int idProceso, int idPlantilla, decimal version,
            DateTime fechaRegistro, int usuarioRegistro, IDbConnection connection, IDbTransaction trx)
        {
            int result = 0;

            const string sqlHistorico = @"Insert into historico_plantillas
                                    (id_entidad, id_proceso, id_plantilla, version, fecha_publicacion, fecha_registro, usuario_registro) 
                                    values
                                    (@idEntidad, @idProceso, @idPlantilla, @version, null, @fechaRegistro, @usuarioRegistro )";

            const string sqlUpdate = @"Update proceso_plantilla set version_propuesta = @version
                                    where id_entidad = @idEntidad
                                    and id_proceso = @idProceso
                                    and id_plantilla = @idPlantilla";

            await connection.ExecuteAsync(sqlHistorico, new { idEntidad, idProceso, idPlantilla, version, fechaRegistro, usuarioRegistro }, trx);

            await connection.ExecuteAsync(sqlUpdate, new { idEntidad, idProceso, idPlantilla, version }, trx);


            return result;
        }
        private decimal ObtenerVersion(decimal versionActual)
        {
            return (versionActual - decimal.Truncate(versionActual)) == Convert.ToDecimal(0.9) ? decimal.Truncate(versionActual) + 1 : versionActual + Convert.ToDecimal(0.1);
        }
        public async Task<int> PublicarVersionPropuestaAsync(HistoricoPlantillasModelo modelo)
        {
            int response = 0;

            const string deleteValoreListaSQL = @"delete proceso_lista_valores where id_entidad = @IdEntidad and id_proceso = @IdProceso and id_plantilla = @IdPlantilla";

            const string deleteCamposSQL = @"delete proceso_plantilla_seccion_campo where id_entidad = @IdEntidad and id_proceso = @IdProceso and id_plantilla = @IdPlantilla";

            const string deleteSeccionesSQL = @"delete proceso_plantilla_seccion where id_entidad = @IdEntidad and id_proceso = @IdProceso and id_plantilla = @IdPlantilla";

            const string insertValoresListaSQL = @"insert into proceso_lista_valores (id_entidad, id_proceso, id_plantilla, id_seccion, id_campo, id_campo_padre, id_valor_padre, id_valor, orden, nombre, predeterminado)
                                                    select id_entidad, id_proceso, id_plantilla, id_seccion, id_campo, id_campo_padre, id_valor_padre, id_valor, orden, nombre, predeterminado
                                                    from proceso_lista_valores_propuesta
                                                    where id_entidad = @IdEntidad and id_proceso = @IdProceso and id_plantilla = @IdPlantilla ";

            const string insertCamposSQL = @"insert into proceso_plantilla_seccion_campo (id_entidad, id_proceso, id_plantilla, id_seccion, id_campo, orden, nombre, descripcion, longitud, obligatorio, no_columnas, id_tipo_campo, activo, id_campo_padre)   
                                               select id_entidad, id_proceso, id_plantilla, id_seccion, id_campo, orden, nombre, descripcion, longitud, obligatorio, no_columnas, id_tipo_campo, activo, id_campo_padre
                                               from proceso_plantilla_seccion_campo_propuesta
                                               where id_entidad = @IdEntidad
                                               and id_proceso = @IdProceso
                                               and id_plantilla = @IdPlantilla";

            const string insertSeccionesSQL = @"insert into proceso_plantilla_seccion (id_entidad, id_proceso, id_plantilla, id_seccion, orden, nombre, descripcion, activa)   
                                               select id_entidad, id_proceso, id_plantilla, id_seccion, orden, nombre, descripcion, activa
                                               from proceso_plantilla_seccion_propuesta
                                               where id_entidad = @IdEntidad
                                               and id_proceso = @IdProceso
                                               and id_plantilla = @IdPlantilla";

            const string actualizarSQL = @"                
                UPDATE HISTORICO_PLANTILLAS SET
                    FECHA_PUBLICACION = @FechaPublicacion
                WHERE
                    ID_ENTIDAD = @IdEntidad AND
                    ID_PROCESO = @IdProceso AND
                    ID_PLANTILLA = @IdPlantilla";

            const string actualizaVersionSQL = @"update proceso_plantilla set version = version_propuesta 
                                                where id_entidad = @IdEntidad 
                                                and id_proceso = @IdProceso 
                                                and id_plantilla = @IdPlantilla";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@IdProceso", modelo.IdProceso);
                    param.Add("@IdEntidad", modelo.IdEntidad);
                    param.Add("@IdPlantilla", modelo.IdPlantilla);
                    param.Add("@FechaPublicacion", modelo.fechaPublicacion);

                    await connection.ExecuteAsync(deleteValoreListaSQL, new { modelo.IdEntidad, modelo.IdProceso, modelo.IdPlantilla }, trx);

                    await connection.ExecuteAsync(deleteCamposSQL, new { modelo.IdEntidad, modelo.IdProceso, modelo.IdPlantilla }, trx);

                    await connection.ExecuteAsync(deleteSeccionesSQL, new { modelo.IdEntidad, modelo.IdProceso, modelo.IdPlantilla }, trx);

                    await connection.ExecuteAsync(insertSeccionesSQL, new { modelo.IdEntidad, modelo.IdProceso, modelo.IdPlantilla }, trx);

                    await connection.ExecuteAsync(insertCamposSQL, new { modelo.IdEntidad, modelo.IdProceso, modelo.IdPlantilla }, trx);

                    await connection.ExecuteAsync(insertValoresListaSQL, new { modelo.IdEntidad, modelo.IdProceso, modelo.IdPlantilla }, trx);

                    await connection.ExecuteAsync(actualizaVersionSQL, new { modelo.IdEntidad, modelo.IdProceso, modelo.IdPlantilla }, trx);

                    await connection.ExecuteAsync(actualizarSQL, param, trx);

                    trx.Commit();

                    response = 1;
                }
                return response;
            }

        }
        public async Task<int> CambiarOrdenAsync(SeccionModelo[] secciones, int idEntidad, int usuarioRegistro, DateTime fechaRegistro)
        {
            int response = 0;

            const string actualizaOrdenSQL = @"update proceso_plantilla_seccion_propuesta set orden = @Orden 
                                                where id_entidad = @IdEntidad 
                                                and id_proceso = @IdProceso 
                                                and id_plantilla = @IdPlantilla
                                                and id_seccion = @IdSeccion";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var p = await ObtenerVersionesPlantilla(idEntidad, secciones[0].IdProceso, secciones[0].IdPlantilla);

                    if (p.Version == p.VersionPropuesta)
                    {
                        decimal versionPropuesta = ObtenerVersion(p.Version);

                        await CrearPlantillaPropuesta(idEntidad, secciones[0].IdProceso, secciones[0].IdPlantilla, versionPropuesta, fechaRegistro, usuarioRegistro, connection, trx);
                    }

                    foreach (var seccion in secciones)
                    {
                        await connection.ExecuteAsync(actualizaOrdenSQL, new { idEntidad, seccion.IdProceso, seccion.IdPlantilla, seccion.IdSeccion, seccion.Orden }, trx);
                    }

                    trx.Commit();
                }
            }

            return response;
        }
        public async Task<int> CambiarOrdenCamposAsync(CampoModelo[] campos, int idEntidad, int idUsuario, DateTime fechaRegistro)
        {
            int response = 0;

            const string actualizaOrdenSQL = @"update proceso_plantilla_seccion_campo_propuesta set orden = @Orden 
                                                where id_entidad = @IdEntidad 
                                                and id_proceso = @IdProceso 
                                                and id_plantilla = @IdPlantilla
                                                and id_seccion = @IdSeccion
                                                and id_campo = @IdCampo";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var p = await ObtenerVersionesPlantilla(idEntidad, campos[0].IdProceso, campos[0].IdPlantilla);

                    if (p.Version == p.VersionPropuesta)
                    {
                        decimal versionPropuesta = ObtenerVersion(p.Version);

                        await CrearPlantillaPropuesta(idEntidad, campos[0].IdProceso, campos[0].IdPlantilla, versionPropuesta, fechaRegistro, idUsuario, connection, trx);
                    }

                    foreach (var campo in campos)
                    {
                        await connection.ExecuteAsync(actualizaOrdenSQL, new { idEntidad, campo.IdProceso, campo.IdPlantilla, campo.IdSeccion, campo.IdCampo, campo.Orden }, trx);
                    }

                    trx.Commit();
                }
            }

            return response;
        }

        public async Task<bool> ExisteComoCampoPadreAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo) {
            const string existeComoCampoPadre = @"
                SELECT COUNT(*) FROM 
                PROCESO_PLANTILLA_SECCION_CAMPO_PROPUESTA
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla 
                AND ID_SECCION = @IdSeccion
                AND ID_CAMPO_PADRE = @IdCampo
            ";

            int response;

            using (var connection = await connectionProvider.OpenAsync())
            {
                response = await connection.ExecuteScalarAsync<int>(existeComoCampoPadre, new { idEntidad, idProceso, idPlantilla, idSeccion, idCampo });
            }

            return response > 0;
        }

        public async Task<bool> ExisteComoValorPadreAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idValor)
        {
            const string existeComoValorPadre = @"
                SELECT * 
                FROM PROCESO_LISTA_VALORES_PROPUESTA
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla 
                AND ID_SECCION = @IdSeccion
                AND ID_CAMPO_PADRE = @IdCampo
                AND ID_VALOR_PADRE = @IdValor
            ";

            int response;

            using (var connection = await connectionProvider.OpenAsync())
            {
                response = await connection.ExecuteScalarAsync<int>(existeComoValorPadre, new { idEntidad, idProceso, idPlantilla, idSeccion, idCampo, idValor });
            }

            return response > 0;
        }

        public async Task<List<CampoModelo>> ObtenerListasAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo) {

            using (var connection = await connectionProvider.OpenAsync())
            {
                const string sqlQuery = @"
                WITH RecursiveCTE AS (
                    SELECT 
                        id_campo
                    FROM 
                        proceso_plantilla_seccion_campo_propuesta
                    WHERE 
                        id_entidad = @idEntidad
                        AND id_plantilla = @idPlantilla
                        AND id_proceso = @idProceso
                        AND id_seccion = @idSeccion
                        AND id_campo_padre = @IdCampo
                    UNION ALL
                    SELECT 
                        p.id_campo
                    FROM 
                        proceso_plantilla_seccion_campo_propuesta p
                    INNER JOIN 
                        RecursiveCTE r ON p.id_campo_padre = r.id_campo
                )
                SELECT 
                    id_Entidad AS IdEntidad, 
                    id_proceso AS IdProceso, 
                    id_plantilla AS IdPlantilla, 
                    id_seccion AS IdSeccion, 
                    id_campo AS IdCampo, 
                    nombre, 
                    descripcion
                FROM 
                    proceso_plantilla_seccion_campo_propuesta
                WHERE 
                    id_entidad = @idEntidad
                    AND id_proceso = @idProceso
                    AND id_plantilla = @idPlantilla
                    AND id_seccion = @idSeccion
                    AND id_tipo_campo = 6
                    AND id_campo <> @IdCampo
                    AND id_campo NOT IN (SELECT id_campo FROM RecursiveCTE)
                ORDER BY 
                    id_Entidad, id_Proceso, id_Plantilla, id_Seccion, id_Campo";

                var result = await connection.QueryAsync<CampoModelo>(sqlQuery, new { idEntidad, idProceso, idPlantilla, idSeccion, idCampo});

                return result.ToList();
            }
        }
        public async Task<bool> ValidaAsigancionListaPadreAsync(CampoModelo campo)
        {

            using (var connection = await connectionProvider.OpenAsync())
            {
                const string sqlQuery = @"
                WITH RecursiveCTE AS (
                    SELECT 
                        id_campo
                    FROM 
                        proceso_plantilla_seccion_campo_propuesta
                    WHERE 
                        id_entidad = @IdEntidad
                        AND id_plantilla = @IdPlantilla
                        AND id_proceso = @IdProceso
                        AND id_seccion = @IdSeccion
                        AND id_Campo_padre = @IdCampo

                    UNION ALL

                    SELECT 
                        p.id_campo
                    FROM 
                        proceso_plantilla_seccion_campo_propuesta p
                    INNER JOIN 
                        RecursiveCTE r ON p.id_campo_padre = r.id_campo
                    WHERE
                        p.id_entidad = @IdEntidad
                        AND p.id_plantilla = @IdPlantilla
                        AND p.id_proceso = @IdProceso
                        AND p.id_seccion = @IdSeccion
                )

                SELECT COUNT(*)
                FROM RecursiveCTE
                WHERE id_campo = @IdCampoPadre
                ";

                var result = await connection.ExecuteScalarAsync<int>(sqlQuery, new { campo.IdEntidad, campo.IdProceso, campo.IdPlantilla, campo.IdSeccion, campo.IdCampo, campo.IdCampoPadre });

                return result == 0;
            }
        }
        public async Task<List<ValorListaModelo>> ObtenerValoresListaAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idCampoPadre, int idValorPadre)
        {

            using (var connection = await connectionProvider.OpenAsync())
            {
                const string sqlQuery = @"select id_entidad IdEntidad, id_proceso IdProceso, id_plantilla IdPlantilla, id_seccion IdSeccion, id_campo IdCampo, id_valor IdValor, orden, 
                                            id_campo_padre IdCampoPadre, id_valor_padre IdValorPadre, nombre, predeterminado
                                            from proceso_lista_valores_propuesta
                                            where id_entidad = @IdEntidad
                                            and id_proceso = @IdProceso
                                            and id_plantilla = @IdPlantilla
                                            and id_seccion = @IdSeccion
                                            and id_campo = @IdCampo
                                            and IIF(@IdCampoPadre = 0, 0, ISNULL(id_campo_padre, 0)) = @IdCampoPadre
                                            and IIF(@IdCampoPadre = 0, 0, ISNULL(id_valor_padre, 0)) = @IdValorPadre
                                            order by orden";

                var result = await connection.QueryAsync<ValorListaModelo>(sqlQuery, new { idEntidad, idProceso, idPlantilla, idSeccion, idCampo, idCampoPadre,  idValorPadre });

                return result.ToList();
            }
        }

        public async Task<List<ValorListaModelo>> ObtenerValoresListaPlantillaActualAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idCampoPadre, int idValorPadre)
        {

            using (var connection = await connectionProvider.OpenAsync())
            {
                const string sqlQuery = @"SELECT 
                                            id_entidad AS IdEntidad, 
                                            id_proceso AS IdProceso, 
                                            id_plantilla AS IdPlantilla, 
                                            id_seccion AS IdSeccion, 
                                            id_campo AS IdCampo, 
                                            id_valor AS IdValor, 
                                            orden, 
                                            id_campo_padre AS IdCampoPadre, 
                                            id_valor_padre AS IdValorPadre, 
                                            nombre, 
                                            predeterminado
                                        FROM proceso_lista_valores
                                        WHERE 
                                            id_entidad = @idEntidad
                                            AND id_proceso = @idProceso
                                            AND id_plantilla = @idPlantilla
                                            AND id_seccion = @idSeccion
                                            AND id_campo = @idCampo
                                            AND (
                                                CASE 
                                                    WHEN @idCampoPadre = 0 THEN 0 
                                                    ELSE ISNULL(id_campo_padre, 0) 
                                                END
                                            ) = @idCampoPadre
                                            AND (
                                                CASE 
                                                    WHEN @idCampoPadre = 0 THEN 0 
                                                    ELSE ISNULL(id_valor_padre, 0) 
                                                END
                                            ) = @idValorPadre
                                        ORDER BY orden;";

                var result = await connection.QueryAsync<ValorListaModelo>(sqlQuery, new { idEntidad, idProceso, idPlantilla, idSeccion, idCampo, idCampoPadre, idValorPadre });

                return result.ToList();
            }
        }

        public async Task<int> AgregarValorListaAsync(ValorListaModelo valor, DateTime fechaRegistro, int usuarioRegistro)
        {
            const string insertSQL = @"INSERT INTO PROCESO_LISTA_VALORES_PROPUESTA
                (id_entidad, id_proceso, id_plantilla, id_seccion, id_campo, id_valor, orden, id_campo_padre, id_valor_padre, nombre, predeterminado)
                OUTPUT INSERTED.ID_VALOR
                VALUES 
                (@IdEntidad, @IdProceso, @IdPlantilla, @IdSeccion, @IdCampo, 
                (select ISNULL(max(id_Valor),0) + 1
                    from proceso_lista_valores_propuesta 
                    where id_entidad = @IdEntidad
                    and id_proceso = @IdProceso
                    and id_plantilla = @IdPlantilla
                    and id_seccion = @IdSeccion
                    and id_campo = @IdCampo), @Orden, IIF(@IdCampoPadre = 0, null, @IdCampoPadre), IIF(@IdValorPadre = 0, NULL, @IdValorPadre), @Nombre, @Predeterminado)
                ";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@IdEntidad", valor.IdEntidad);
                    param.Add("@IdProceso", valor.IdProceso);
                    param.Add("@IdPlantilla", valor.IdPlantilla);
                    param.Add("@IdSeccion", valor.IdSeccion);
                    param.Add("@IdCampo", valor.IdCampo);
                    param.Add("@IdCampoPadre", valor.IdCampoPadre);
                    param.Add("@IdValorPadre", valor.IdValorPadre);
                    param.Add("@Nombre", valor.Nombre);
                    param.Add("@Predeterminado", 0);
                    param.Add("@IdValor", 0);
                    param.Add("@Orden", valor.Orden);

                    var p = await ObtenerVersionesPlantilla(valor.IdEntidad, valor.IdProceso, valor.IdPlantilla);

                    if (p.Version == p.VersionPropuesta)
                    {
                        decimal versionPropuesta = ObtenerVersion(p.Version);

                        await CrearPlantillaPropuesta(valor.IdEntidad, valor.IdProceso, valor.IdPlantilla, versionPropuesta, fechaRegistro, usuarioRegistro, connection, trx);
                    }

                    await connection.ExecuteAsync(insertSQL, param, trx);

                    trx.Commit();

                    valor.IdValor = param.Get<int>("IdValor");
                }
            }

            return valor.IdValor;
        }
        public async Task<int> EliminarValorListaAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idValor, int usuarioRegistro, DateTime fechaRegistro)
        {
            const string eliminarCampoSQL = @"                
                DELETE PROCESO_LISTA_VALORES_PROPUESTA 
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla 
                AND ID_SECCION = @IdSeccion
                AND ID_CAMPO = @IdCampo
                AND ID_VALOR = @IdValor";

            const string selectValoresSQL = @"                
                SELECT id_entidad idEntidad, id_proceso idProceso, id_Plantilla idPlantilla, id_seccion idSeccion, id_campo idCampo, id_valor IdValor, orden
                FROM PROCESO_LISTA_VALORES_PROPUESTA
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla 
                AND ID_SECCION = @IdSeccion
                AND ID_CAMPO = @IdCampo
                order by orden";

            const string updateValoresSQL = @"                
                UPDATE PROCESO_LISTA_VALORES_PROPUESTA SET ORDEN = @Orden
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla 
                AND ID_SECCION = @IdSeccion
                AND ID_CAMPO = @IdCampo
                AND ID_VALOR = @IdValor";

            int response;

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var p = await ObtenerVersionesPlantilla(idEntidad, idProceso, idPlantilla);

                    if (p.Version == p.VersionPropuesta)
                    {
                        decimal versionPropuesta = ObtenerVersion(p.Version);

                        await CrearPlantillaPropuesta(idEntidad, idProceso, idPlantilla, versionPropuesta, fechaRegistro, usuarioRegistro, connection, trx);
                    }

                    response = await connection.ExecuteAsync(eliminarCampoSQL, new { idEntidad, idProceso, idPlantilla, idSeccion, idCampo, idValor }, trx);

                    var valores = await connection.QueryAsync<ValorListaModelo>(selectValoresSQL, new { idEntidad, idProceso, idPlantilla, idSeccion, idCampo }, trx);

                    int i = 1;
                    foreach (var valor in valores.ToList())
                    {
                        if (valor.Orden != i)
                        {
                            var r = await connection.ExecuteAsync(updateValoresSQL, new { idEntidad, idProceso, idPlantilla, idSeccion, idCampo, valor.IdValor, Orden = i }, trx);
                        }
                        i++;
                    }

                    trx.Commit();
                }

            }
            return response;
        }
        public async Task<int> EliminarValoresListaAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int usuarioRegistro, DateTime fechaRegistro)
        {
            const string eliminarCampoSQL = @"                
                DELETE PROCESO_LISTA_VALORES_PROPUESTA 
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_PLANTILLA = @IdPlantilla 
                AND ID_SECCION = @IdSeccion
                AND ID_CAMPO = @IdCampo";

            int response;

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var p = await ObtenerVersionesPlantilla(idEntidad, idProceso, idPlantilla);

                    if (p.Version == p.VersionPropuesta)
                    {
                        decimal versionPropuesta = ObtenerVersion(p.Version);

                        await CrearPlantillaPropuesta(idEntidad, idProceso, idPlantilla, versionPropuesta, fechaRegistro, usuarioRegistro, connection, trx);
                    }

                    response = await connection.ExecuteAsync(eliminarCampoSQL, new { idEntidad, idProceso, idPlantilla, idSeccion, idCampo }, trx);

                    trx.Commit();
                }

            }
            return response;
        }
        public async Task<int> CambiarOrdenValoresAsync(ValorListaModelo[] valores, int idEntidad, int idUsuario, DateTime fechaRegistro)
        {
            int response = 0;

            const string actualizaOrdenSQL = @"update proceso_lista_valores_propuesta set orden = @Orden 
                                                where id_entidad = @IdEntidad 
                                                and id_proceso = @IdProceso 
                                                and id_plantilla = @IdPlantilla
                                                and id_seccion = @IdSeccion
                                                and id_campo = @IdCampo
                                                and id_valor = @IdValor";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var p = await ObtenerVersionesPlantilla(idEntidad, valores[0].IdProceso, valores[0].IdPlantilla);

                    if (p.Version == p.VersionPropuesta)
                    {
                        decimal versionPropuesta = ObtenerVersion(p.Version);

                        await CrearPlantillaPropuesta(idEntidad, valores[0].IdProceso, valores[0].IdPlantilla, versionPropuesta, fechaRegistro, idUsuario, connection, trx);
                    }

                    foreach (var valor in valores)
                    {
                        await connection.ExecuteAsync(actualizaOrdenSQL, new { idEntidad, valor.IdProceso, valor.IdPlantilla, valor.IdSeccion, valor.IdCampo, valor.IdValor, valor.Orden }, trx);
                    }

                    trx.Commit();
                }
            }

            return response;
        }
        public async Task<int> PredeterminarValorListaAsync(ValorListaModelo valor, DateTime fechaRegistro, int usuarioRegistro)
        {
            const string updateSQL = @"UPDATE PROCESO_LISTA_VALORES_PROPUESTA SET PREDETERMINADO=0
                                        WHERE ID_ENTIDAD = @IdEntidad 
                                        AND ID_PROCESO = @IdProceso 
                                        AND ID_PLANTILLA = @IdPlantilla 
                                        AND ID_SECCION = @IdSeccion
                                        AND ID_CAMPO = @IdCampo";

            const string updateSQL2 = @"UPDATE PROCESO_LISTA_VALORES_PROPUESTA SET PREDETERMINADO= @Predeterminar
                                        WHERE ID_ENTIDAD = @IdEntidad 
                                        AND ID_PROCESO = @IdProceso 
                                        AND ID_PLANTILLA = @IdPlantilla 
                                        AND ID_SECCION = @IdSeccion
                                        AND ID_CAMPO = @IdCampo
                                        AND ID_VALOR = @IdValor";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@IdEntidad", valor.IdEntidad);
                    param.Add("@IdProceso", valor.IdProceso);
                    param.Add("@IdPlantilla", valor.IdPlantilla);
                    param.Add("@IdSeccion", valor.IdSeccion);
                    param.Add("@IdCampo", valor.IdCampo);
                    param.Add("@IdCampoPadre", valor.IdCampoPadre);
                    param.Add("@IdValorPadre", valor.IdValorPadre);
                    param.Add("@Nombre", valor.Nombre);
                    param.Add("@Predeterminar", valor.Predeterminado);
                    param.Add("@IdValor", valor.IdValor);

                    var p = await ObtenerVersionesPlantilla(valor.IdEntidad, valor.IdProceso, valor.IdPlantilla);

                    if (p.Version == p.VersionPropuesta)
                    {
                        decimal versionPropuesta = ObtenerVersion(p.Version);

                        await CrearPlantillaPropuesta(valor.IdEntidad, valor.IdProceso, valor.IdPlantilla, versionPropuesta, fechaRegistro, usuarioRegistro, connection, trx);
                    }

                    await connection.ExecuteAsync(updateSQL, param, trx);
                    await connection.ExecuteAsync(updateSQL2, param, trx);

                    trx.Commit();

                }
            }

            return valor.IdValor;
        }
        public async Task<int> RevertirCambiosPlantillaAsync(HistoricoPlantillasModelo modelo)
        {
            int response = 0;

            const string deleteValoreListaSQL = @"delete proceso_lista_valores_propuesta where id_entidad = @IdEntidad and id_proceso = @IdProceso and id_plantilla = @IdPlantilla";

            const string deleteCamposSQL = @"delete proceso_plantilla_seccion_campo_propuesta where id_entidad = @IdEntidad and id_proceso = @IdProceso and id_plantilla = @IdPlantilla";

            const string deleteSeccionesSQL = @"delete proceso_plantilla_seccion_propuesta where id_entidad = @IdEntidad and id_proceso = @IdProceso and id_plantilla = @IdPlantilla";

            const string insertValoresListaSQL = @"insert into proceso_lista_valores_propuesta (id_entidad, id_proceso, id_plantilla, id_seccion, id_campo, id_campo_padre, id_valor_padre, id_valor, orden, nombre, predeterminado)
                                                    select id_entidad, id_proceso, id_plantilla, id_seccion, id_campo, id_campo_padre, id_valor_padre, id_valor, orden, nombre, predeterminado
                                                    from proceso_lista_valores
                                                    where id_entidad = @IdEntidad and id_proceso = @IdProceso and id_plantilla = @IdPlantilla ";

            const string insertCamposSQL = @"insert into proceso_plantilla_seccion_campo_propuesta (id_entidad, id_proceso, id_plantilla, id_seccion, id_campo, orden, nombre, descripcion, longitud, obligatorio, no_columnas, id_tipo_campo, activo, id_campo_padre)   
                                               select id_entidad, id_proceso, id_plantilla, id_seccion, id_campo, orden, nombre, descripcion, longitud, obligatorio, no_columnas, id_tipo_campo, activo, id_campo_padre
                                               from proceso_plantilla_seccion_campo
                                               where id_entidad = @IdEntidad
                                               and id_proceso = @IdProceso
                                               and id_plantilla = @IdPlantilla";

            const string insertSeccionesSQL = @"insert into proceso_plantilla_seccion_propuesta (id_entidad, id_proceso, id_plantilla, id_seccion, orden, nombre, descripcion, activa)   
                                               select id_entidad, id_proceso, id_plantilla, id_seccion, orden, nombre, descripcion, activa
                                               from proceso_plantilla_seccion
                                               where id_entidad = @IdEntidad
                                               and id_proceso = @IdProceso
                                               and id_plantilla = @IdPlantilla";

            const string deleteHistoricoSQL = @"DELETE HISTORICO_PLANTILLAS
                                                WHERE
                                                    ID_ENTIDAD = @IdEntidad AND
                                                    ID_PROCESO = @IdProceso AND
                                                    ID_PLANTILLA = @IdPlantilla";

            const string actualizaVersionSQL = @"update proceso_plantilla set version_propuesta = version
                                                where id_entidad = @IdEntidad 
                                                and id_proceso = @IdProceso 
                                                and id_plantilla = @IdPlantilla";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@IdProceso", modelo.IdProceso);
                    param.Add("@IdEntidad", modelo.IdEntidad);
                    param.Add("@IdPlantilla", modelo.IdPlantilla);
                    param.Add("@FechaPublicacion", modelo.fechaPublicacion);

                    await connection.ExecuteAsync(deleteValoreListaSQL, new { modelo.IdEntidad, modelo.IdProceso, modelo.IdPlantilla }, trx);

                    await connection.ExecuteAsync(deleteCamposSQL, new { modelo.IdEntidad, modelo.IdProceso, modelo.IdPlantilla }, trx);

                    await connection.ExecuteAsync(deleteSeccionesSQL, new { modelo.IdEntidad, modelo.IdProceso, modelo.IdPlantilla }, trx);

                    await connection.ExecuteAsync(insertSeccionesSQL, new { modelo.IdEntidad, modelo.IdProceso, modelo.IdPlantilla }, trx);

                    await connection.ExecuteAsync(insertCamposSQL, new { modelo.IdEntidad, modelo.IdProceso, modelo.IdPlantilla }, trx);

                    await connection.ExecuteAsync(insertValoresListaSQL, new { modelo.IdEntidad, modelo.IdProceso, modelo.IdPlantilla }, trx);

                    await connection.ExecuteAsync(actualizaVersionSQL, new { modelo.IdEntidad, modelo.IdProceso, modelo.IdPlantilla }, trx);

                    await connection.ExecuteAsync(deleteHistoricoSQL, param, trx);

                    trx.Commit();

                    response = 1;
                }
                return response;
            }

        }

    }
}