using Dapper;
using Qfile.Core.Constantes;
using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using Qfile.Core.Servicios.Documentos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Qfile.Datos
{
    public class ExpedienteDatos : IExpedienteDatos
    {
        private readonly IConnectionProvider connectionProvider;
        private readonly IDocumentoServicio _documentosServicio;

        public ExpedienteDatos(IConnectionProvider connectionProvider, IDocumentoServicio documentosServicio)
        {
            this.connectionProvider = connectionProvider;
            _documentosServicio = documentosServicio;
        }

        public async Task<List<ExpedienteListaModelo>> ObtenerExpedientesAsync(int idEntidad, int idUsuario, int pagina, int cantidad, string buscarTexto)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"
                SELECT * FROM (
                    SELECT 
                        ex.id_expediente AS idexpediente,
                        ex.id_proceso AS idproceso,
                        ex.descripcion AS descripcion,
                        ori.nombre AS nombreorigen,
                        ex.emisor AS emisor,
                        ex.fecha_asignacion AS fechaasignacion,
                        ex.fecha_traslado AS fechatraslado,
                        pr.nombre AS nombreproceso,
                        pf.id_fase AS idfaseactual,
                        pf.id_tipo_fase AS idtipofase,
                        pf.nombre AS faseactualproceso,
                        pr.color AS colorproceso,
                        pr.activo AS activoproceso,
                        COUNT(*) OVER () cantidadtotal,
                        ex.id_tipo_operacion ultimaidtipooperacion,
                        ea.id_tipo_operacion idtipooperacion,
                        ex.id_usuario_asignado idUsuarioAsignado,
                        @idUsuario idUsuarioConsulta,
                        tp.TIEMPO_PROMEDIO_TOTAL AS TiempoPromedioTotal, 
                        CASE 
                            WHEN pr.tipo_proceso = 1 THEN ROUND((((DATEDIFF(MINUTE, ex.FECHA_REGISTRO, GETDATE())) / tp.TIEMPO_PROMEDIO_TOTAL) * 100), 2)
                            WHEN pr.tipo_proceso = 0 AND (
                                    SELECT ed.VALOR
                                    FROM exp_expedientes_datos ed
                                    WHERE ed.id_expediente = ex.id_expediente                                        
                                    AND ed.id_campo = 5
                                ) IS NULL THEN -1
                            WHEN pr.tipo_proceso = 0 AND (
                                    SELECT ed.VALOR
                                    FROM exp_expedientes_datos ed
                                    WHERE ed.id_expediente = ex.id_expediente                                        
                                    AND ed.id_campo = 5
                                ) IS NOT NULL THEN
                                ROUND(
                                    (
                                        (
                                            (
                                                (DATEDIFF(MINUTE, ex.FECHA_REGISTRO, GETDATE())) / 
                                                (
                                                    (
                                                        SELECT DATEDIFF(MINUTE, ex.FECHA_REGISTRO, CAST(ed.valor AS DATETIME))
                                                        FROM exp_expedientes_datos ed
                                                        WHERE ed.id_expediente = ex.id_expediente
                                                        AND ed.id_campo = 5
                                                    )
                                                ) * 1400
                                            )
                                        ) * 100
                                    ), 2
                                )                                                                    
                        END AS PorcentajeTiempoTranscurrido,
                        CASE
                            WHEN ea.fecha_limite_atencion IS NULL THEN -1
                            WHEN ea.fecha_limite_atencion IS NOT NULL THEN
                                ROUND((((DATEDIFF(MINUTE, ex.FECHA_REGISTRO, GETDATE())) / (DATEDIFF(MINUTE, ex.FECHA_REGISTRO, ea.fecha_limite_atencion)) * 1400) * 100), 2)
                        END AS PorcentajeTiempoInterno
                    FROM exp_expediente ex
                    JOIN ca_origenes ori ON ori.id_entidad = ex.id_entidad 
                        AND ori.id_origen = ex.id_origen
                    JOIN proceso_fase pf ON pf.id_entidad = ex.id_entidad 
                        AND pf.id_proceso = ex.id_proceso
                        AND pf.id_fase = ex.id_fase_actual
                        AND pf.id_tipo_fase <> 3
                    JOIN proceso pr ON pr.id_proceso = ex.id_proceso 
                        AND pr.id_entidad = ex.id_entidad
                    JOIN proceso_fase_usuarios pfu ON pfu.id_fase = pf.id_fase
                        AND pfu.id_proceso = pf.id_proceso
                        AND pfu.id_entidad = pf.id_entidad
                        AND pfu.ID_USUARIO = @idUsuario
                    JOIN (
                            SELECT 
                                id_entidad, 
                                id_proceso, 
                                SUM(
                                    CASE 
                                    WHEN id_unidad_medida = 1 THEN tiempo_promedio
                                    WHEN id_unidad_medida = 2 THEN tiempo_promedio * 60
                                    WHEN id_unidad_medida = 3 THEN tiempo_promedio * 24 * 60
                                    END
                                ) AS tiempo_promedio_total
                            FROM proceso_fase
                            GROUP BY id_entidad, id_proceso
                        ) tp ON tp.id_entidad = ex.id_entidad
                        AND tp.id_proceso = ex.id_proceso
                    JOIN exp_expediente_asignaciones ea ON ea.id_expediente = ex.id_expediente
                        AND ea.id_proceso = ex.id_proceso
                        AND ea.id_entidad = ex.id_entidad
                        AND ea.id_fase = ex.id_fase_actual
                        AND ea.fecha_traslado = ex.fecha_traslado
                        AND ea.fecha_asignacion = ex.fecha_asignacion
                    WHERE ex.id_entidad = @identidad
                    AND (ex.id_usuario_asignado = @idusuario OR ex.id_usuario_asignado IS NULL)

                    UNION ALL

                    SELECT 
                        ex.id_expediente AS idexpediente,
                        ex.id_proceso AS idproceso,
                        ex.descripcion AS descripcion,
                        ori.nombre AS nombreorigen,
                        ex.emisor AS emisor,
                        ea.fecha_asignacion AS fechaasignacion,
                        ea.fecha_traslado AS fechatraslado,
                        pr.nombre AS nombreproceso,
                        pf.id_fase AS idfaseactual,
                        pf.id_tipo_fase AS idtipofase,
                        pf.nombre AS faseactualproceso,
                        pr.color AS colorproceso,
                        pr.activo AS activoproceso,
                        COUNT(*) OVER() cantidadtotal,
                        ex.id_tipo_operacion ultimaidtipooperacion,
                        ea.id_tipo_operacion idtipooperacion,
                        ex.id_usuario_asignado idUsuarioAsignado,
                        @idUsuario idUsuarioConsulta,
                        tp.TIEMPO_PROMEDIO_TOTAL AS TiempoPromedioTotal, 
                        CASE
                            WHEN pr.tipo_proceso = 1 THEN
                                ROUND((((DATEDIFF(MINUTE, ex.FECHA_REGISTRO, GETDATE())) / tp.TIEMPO_PROMEDIO_TOTAL) * 100), 2)
                            WHEN pr.tipo_proceso = 0 AND (SELECT ed.VALOR
                                                            FROM exp_expedientes_datos ed
                                                            WHERE ed.id_expediente = ex.id_expediente
                                                            AND ed.id_campo = 5) IS NULL THEN
                                -1
                            WHEN pr.tipo_proceso = 0 AND (SELECT ed.VALOR
                                                            FROM exp_expedientes_datos ed
                                                            WHERE ed.id_expediente = ex.id_expediente
                                                            AND ed.id_campo = 5) IS NOT NULL THEN
                                ROUND(((((DATEDIFF(MINUTE, ex.FECHA_REGISTRO, GETDATE())) / 
                                        ((SELECT DATEDIFF(MINUTE, ex.FECHA_REGISTRO, 
                                                CAST(ed.valor AS DATETIME))
                                        FROM exp_expedientes_datos ed
                                        WHERE ed.id_expediente = ex.id_expediente
                                            AND ed.id_campo = 5)) * 1400)) * 100), 2)                                                                    
                        END AS PorcentajeTiempoTranscurrido,
                        CASE
                            WHEN ea.fecha_limite_atencion IS NULL THEN
                                -1
                            WHEN ea.fecha_limite_atencion IS NOT NULL THEN
                                ROUND((((DATEDIFF(MINUTE, ex.FECHA_REGISTRO, GETDATE())) / 
                                        (DATEDIFF(MINUTE, ex.FECHA_REGISTRO, ea.fecha_limite_atencion)) * 1400) * 100), 2)
                        END AS PorcentajeTiempoInterno
                    FROM exp_expediente ex
                    JOIN ca_origenes ori ON ori.id_entidad = ex.id_entidad 
                        AND ori.id_origen = ex.id_origen
                    JOIN (
                        SELECT MAX(ea2.fecha_asignacion) fecha_asignacion, ea2.id_expediente
                        FROM exp_expediente_asignaciones ea2
                        JOIN proceso_fase pf ON pf.id_fase = ea2.id_fase AND pf.id_tipo_fase <> 3
                        GROUP BY ea2.id_expediente
                        ) ua ON ex.id_expediente = ua.id_expediente 
                    JOIN exp_expediente_asignaciones ea ON ea.id_expediente = ex.id_expediente
                        AND ea.id_usuario_asignado = @idUsuario
                        AND ea.id_Tipo_operacion = ex.id_Tipo_operacion
                        AND ea.id_proceso = ex.id_proceso
                        AND ea.id_entidad = ex.id_entidad
                        AND ea.fecha_Asignacion = ua.fecha_asignacion
                        AND ea.id_expediente = ua.id_expediente
                    JOIN proceso_fase pf ON pf.id_entidad = ea.id_entidad 
                        AND pf.id_proceso = ea.id_proceso
                        AND pf.id_fase = ea.id_fase
                        AND pf.id_tipo_fase <> 3
                    JOIN proceso pr ON pr.id_entidad = ex.id_entidad 
                        AND pr.id_proceso = ex.id_proceso                                                       
                    JOIN (
                        SELECT 
                            id_entidad, 
                            id_proceso, 
                            SUM(CASE 
                                WHEN id_unidad_medida = 1 THEN tiempo_promedio
                                WHEN id_unidad_medida = 2 THEN tiempo_promedio * 60
                                WHEN id_unidad_medida = 3 THEN tiempo_promedio * 24 * 60
                                END
                            ) AS tiempo_promedio_total
                        FROM proceso_fase
                        GROUP BY id_entidad, id_proceso
                        ) tp ON tp.id_entidad = ex.id_entidad
                        AND tp.id_proceso = ex.id_proceso
                    WHERE ex.id_tipo_operacion = 2
                    AND ex.id_entidad = @identidad
                ) q1
                ORDER BY CAST(q1.idexpediente AS INT)
                OFFSET (@Pagina-1)*@Cantidad ROWS
                FETCH NEXT @Cantidad ROWS ONLY
                ";

                var result = await connection.QueryAsync<ExpedienteListaModelo>(sqlQuery, new
                {
                    Pagina = pagina,
                    Cantidad = cantidad,
                    BuscarTexto = buscarTexto,
                    idUsuario = idUsuario,
                    idEntidad = idEntidad
                });

                return result.ToList();
            }
        }
        
        public async Task<int> GetIdExpediente(System.Data.IDbConnection connection, int Ejercicio, System.Data.IDbTransaction trx) 
        {
            const string GetSolicitudIdSql = @"SELECT CONCAT(CORRELATIVO + 1, EJERCICIO) ID
                                                FROM CORRELATIVO_EXPEDIENTE
                                                WHERE EJERCICIO = @Ejercicio";

            const string InsertSolicitudIdSql = @"INSERT INTO CORRELATIVO_EXPEDIENTE
                                                    (EJERCICIO, CORRELATIVO)
                                                    VALUES( @Ejercicio, 1)";

            const string UpdateSolicitudIdSql = @"UPDATE CORRELATIVO_EXPEDIENTE
                                                    SET CORRELATIVO = CORRELATIVO + 1
                                                    WHERE EJERCICIO = @Ejercicio";
                        
            var result = await connection.ExecuteScalarAsync<int>(GetSolicitudIdSql, new { Ejercicio }, trx);

            if (result == 0) {
                await connection.ExecuteAsync(InsertSolicitudIdSql, new { Ejercicio }, trx);
                result = Convert.ToInt32("1" + Ejercicio.ToString());
            }
            else
                await connection.ExecuteAsync(UpdateSolicitudIdSql, new { Ejercicio }, trx);

            return result;
            
        }

        public async Task<int> CrearExpedienteAsync(ExpedienteModelo expediente, int idUsuarioRegistro, DateTime fechaRegistro)
        {
            const string obtenerFaseInicialDelProcesoSQL = "SELECT id_fase idFaseInicial, nombre faseInicial FROM PROCESO_FASE WHERE ID_PROCESO = @idProceso AND ID_ENTIDAD = @idEntidad and id_tipo_fase = 1";

            const string insertarExpedienteSQL = @"                
                INSERT INTO EXP_EXPEDIENTE
                (ID_EXPEDIENTE, EMISOR, DESCRIPCION, ID_ORIGEN, ID_ENTIDAD, ID_PROCESO, FECHA_ASIGNACION, ID_USUARIO_ASIGNADO, FECHA_REGISTRO, ID_USUARIO_REGISTRO, ID_FASE_ACTUAL)
                OUTPUT INSERTED.ID_EXPEDIENTE
                VALUES(@IdExpediente, @Emisor, @Descripcion, @IdOrigen, @IdEntidad, @IdProceso, @FechaAsignacion, @IdUsuarioAsignado, @FechaRegistro, @IdUsuarioRegistro, @IdFaseActual)
                ";

            const string insertarExpedienteDatosSQL = @"
            INSERT INTO EXP_EXPEDIENTES_DATOS (
                ID_EXPEDIENTE, 
                ID_PLANTILLA, 
                ID_SECCION, 
                ID_CAMPO, 
                ORDEN, 
                NOMBRE_CAMPO, 
                NOMBRE_SECCION,
                CAMPO_ACTIVO,	
                SECCION_ACTIVA,
                DESCRIPCION, 
                LONGITUD, 
                OBLIGATORIO, 
                NO_COLUMNAS, 
                ID_CAMPO_PADRE, 
                VALOR, 
                ID_TIPO_CAMPO
            )
            SELECT 
                @IdExpediente AS ID_EXPEDIENTE, 
                ppsc.ID_PLANTILLA, 
                ppsc.ID_SECCION,
                ppsc.ID_CAMPO,
                ppsc.ORDEN, 
                ppsc.NOMBRE AS NOMBRE_CAMPO, 
                pps.NOMBRE AS NOMBRE_SECCION, 
                ppsc.ACTIVO AS CAMPO_ACTIVO, 
                pps.ACTIVA AS SECCION_ACTIVA,
                ppsc.DESCRIPCION, 
                ppsc.LONGITUD, 
                ppsc.OBLIGATORIO, 
                ppsc.NO_COLUMNAS, 
                ppsc.ID_CAMPO_PADRE, 
                '' AS VALOR, 
                ppsc.ID_TIPO_CAMPO 
            FROM 
                PROCESO_PLANTILLA_SECCION_CAMPO ppsc
            JOIN 
                PROCESO_PLANTILLA_SECCION pps 
                ON ppsc.ID_ENTIDAD = pps.ID_ENTIDAD 
                AND ppsc.ID_PROCESO = pps.ID_PROCESO 
                AND ppsc.ID_PLANTILLA = pps.ID_PLANTILLA 
                AND ppsc.ID_SECCION = pps.ID_SECCION 
            JOIN 
                PROCESO_PLANTILLA pp 
                ON pps.ID_ENTIDAD = pp.ID_ENTIDAD 
                AND pps.ID_PROCESO = pp.ID_PROCESO 
                AND pps.ID_PLANTILLA = pp.ID_PLANTILLA
            WHERE 
                pp.ID_ENTIDAD = @IdEntidad 
                AND pp.ID_PROCESO = @IdProceso 
                AND pp.ACTIVA = 1
            ";

            const string insertarRequisitosGestionSQL = @"INSERT INTO exp_expediente_requisitos_gestion
                    (id_expediente, id_requisito, requisito, obligatorio, presentado,
                     observacion, fecha_registro, id_entidad, id_usuario_registro)
           SELECT @IdExpediente AS id_expediente, id_requisito, requisito,
                  obligatorio, 0, NULL, @FechaRegistro, id_entidad,
                  @IdUsuarioRegistro id_usuario_registro
             FROM proceso_requisitos
             where id_proceso = @IdProceso";


            using (var connection = await connectionProvider.OpenAsync())
            {
                TrasladoModelo traslado;

                using (var trx = connection.BeginTransaction())
                {
                    // fase inicial del proceso

                    var resp = await connection.QueryAsync<FaseIniciaModelo>(obtenerFaseInicialDelProcesoSQL, new
                    {
                        idProceso = expediente.IdProceso,
                        idEntidad = expediente.IdEntidad
                    }, trx);
                    var faseInicial = resp.FirstOrDefault();

                    // correlativo expediente                   

                    int idExpediente = await GetIdExpediente(connection, fechaRegistro.Year, trx);

                    // expediente
                    var paramExpediente = new DynamicParameters();
                    paramExpediente.Add("@IdExpediente", idExpediente);
                    paramExpediente.Add("@Emisor", expediente.Emisor);
                    paramExpediente.Add("@Descripcion", expediente.Descripcion);
                    paramExpediente.Add("@IdOrigen", expediente.IdOrigen);
                    paramExpediente.Add("@IdEntidad", expediente.IdEntidad);
                    paramExpediente.Add("@IdProceso", expediente.IdProceso);
                    paramExpediente.Add("@FechaAsignacion", fechaRegistro);
                    paramExpediente.Add("@IdUsuarioAsignado", idUsuarioRegistro);
                    paramExpediente.Add("@FechaRegistro", fechaRegistro);
                    paramExpediente.Add("@IdUsuarioRegistro", idUsuarioRegistro);
                    paramExpediente.Add("@IdFaseActual", faseInicial.IdFaseInicial);

                    await connection.ExecuteAsync(insertarExpedienteSQL, paramExpediente, trx);

                    expediente.IdExpediente = paramExpediente.Get<int>("IdExpediente");

                    // expediente datos
                    var paramExpedienteDatos = new DynamicParameters();
                    paramExpedienteDatos.Add("@IdExpediente", idExpediente);
                    paramExpedienteDatos.Add("@IdEntidad", expediente.IdEntidad);
                    paramExpedienteDatos.Add("@IdProceso", expediente.IdProceso);

                    await connection.ExecuteAsync(insertarExpedienteDatosSQL, paramExpedienteDatos, trx);

                    var paramRequisitosExpediente = new DynamicParameters();
                    paramRequisitosExpediente.Add("@IdExpediente", idExpediente);
                    paramRequisitosExpediente.Add("@IdUsuarioRegistro", expediente.IdUsuarioRegistro);
                    paramRequisitosExpediente.Add("@IdProceso", expediente.IdProceso);
                    paramRequisitosExpediente.Add("@FechaRegistro", fechaRegistro);

                    await connection.ExecuteAsync(insertarRequisitosGestionSQL, paramRequisitosExpediente, trx);


                    traslado = new TrasladoModelo
                    {
                        IdEntidad = expediente.IdEntidad,
                        IdProceso = expediente.IdProceso,
                        IdExpediente = expediente.IdExpediente,
                        IdFaseDestino = faseInicial.IdFaseInicial,
                        FaseDestino = faseInicial.FaseInicial,
                        FechaTraslado = fechaRegistro,
                        FechaAsignacion = fechaRegistro,
                        IdUsuarioRegistro = idUsuarioRegistro,
                        IdUsuarioAsignado = idUsuarioRegistro,
                        IdTipoOperacion = TipoOperacion.TrasladoConAsignacionObligatoria
                    };

                    trx.Commit();
                }

                var result = TrasladarExpediente(traslado); // TODO: agregar esta acción a la transacción.
            }

            return expediente.IdExpediente;
        }

        public async Task<ExpedienteModelo> ObtenerExpedienteAsync(int idExpediente)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"SELECT 
	                                    ID_EXPEDIENTE AS IdExpediente, 
	                                    EMISOR AS Emisor, 
	                                    DESCRIPCION AS Descripcion, 
	                                    ID_ORIGEN AS IdOrigen, 
	                                    ID_ENTIDAD AS IdEntidad, 
	                                    ID_PROCESO AS IdProceso, 
                                        FECHA_TRASLADO AS FechaTraslado,
	                                    FECHA_ASIGNACION AS FechaAsignacion, 
	                                    ID_USUARIO_ASIGNADO AS IdUsuarioAsignado, 
	                                    ID_FASE_ACTUAL AS IdFaseActual
                                    FROM EXP_EXPEDIENTE
                                    WHERE ID_EXPEDIENTE = @idExpediente";

                var result = await connection.QueryAsync<ExpedienteModelo>(sqlQuery, new { idExpediente });
                return result.FirstOrDefault();
            }
        }

        public async Task<List<ExpedienteSeccionDatosModelo>> ObtenerExpedienteDatosAsync(int idExpediente)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"SELECT ID_EXPEDIENTE AS IdExpediente, ID_PLANTILLA AS IdPlantilla, ID_SECCION AS IdSeccion, NOMBRE_SECCION Nombre, SECCION_ACTIVA Activa, ID_SECCION AS IdSeccion2,
                                        ID_CAMPO AS IdCampo, ORDEN AS Orden, NOMBRE_CAMPO AS Nombre, DESCRIPCION AS Descripcion, LONGITUD AS Longitud, OBLIGATORIO AS Obligatorio, 
                                        NO_COLUMNAS AS NoColumnas, ID_CAMPO_PADRE AS IdCampoPadre, VALOR AS Valor, ID_TIPO_CAMPO AS IdTipoCampo, CAMPO_ACTIVO Activo
                                    FROM EXP_EXPEDIENTES_DATOS
                                    WHERE ID_EXPEDIENTE = @idExpediente";

                var seccionDictionary = new Dictionary<int, ExpedienteSeccionDatosModelo>();

                var result = await connection.QueryAsync<ExpedienteSeccionDatosModelo, ExpedienteCampoDatosModelo, ExpedienteSeccionDatosModelo>(sqlQuery, (s, c) =>
                {
                    ExpedienteSeccionDatosModelo seccion;

                    if (!seccionDictionary.TryGetValue(s.IdSeccion, out seccion))
                    {
                        seccion = s;
                        seccion.ListaCampos = new List<ExpedienteCampoDatosModelo>();
                        seccionDictionary.Add(seccion.IdSeccion, seccion);
                    }

                    if (c != null && c.IdCampo != 0)
                    {
                        seccion.ListaCampos.AsList().Add(c);
                    }

                    return seccion;

                }, new { idExpediente }, splitOn: "IdSeccion2");

                return result.Distinct().ToList();
            }
        }

        public async Task<int> GuardarExpedienteDatosAsync(List<ExpedienteSeccionDatosModelo> datosExpediente)
        {
            int result = 0;
            string actualizarValorCampoSql = @"UPDATE EXP_EXPEDIENTES_DATOS
                                SET VALOR = :Valor
                                WHERE 
	                                ID_EXPEDIENTE = :IdExpediente AND 
	                                ID_PLANTILLA = :IdPlantilla AND 
	                                ID_SECCION = :IdSeccion AND 
	                                ID_CAMPO = :IdCampo";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    foreach (var seccion in datosExpediente)
                    {
                        foreach (var campo in seccion.ListaCampos)
                        {
                            // expediente
                            var paramCampo = new DynamicParameters();
                            paramCampo.Add("@IdExpediente", seccion.IdExpediente);
                            paramCampo.Add("@IdPlantilla", seccion.IdPlantilla);
                            paramCampo.Add("@IdSeccion", seccion.IdSeccion);
                            paramCampo.Add("@IdCampo", campo.IdCampo);
                            paramCampo.Add("@Valor", campo.Valor);

                            result += await connection.ExecuteAsync(actualizarValorCampoSql, paramCampo, trx);
                        }
                    }

                    trx.Commit();
                }

            }

            return result;
        }

        public async Task<int> ActualizarExpedienteAsync(ExpedienteModelo expediente)
        {
            const string actualizarExpedienteSQL = @"                
                UPDATE EXP_EXPEDIENTE
	                SET EMISOR= @Emisor, DESCRIPCION= @Descripcion, ID_ORIGEN= @IdOrigen 
                WHERE ID_EXPEDIENTE= @IdExpediente AND ID_PROCESO= @IdProceso";

            using (var connection = await connectionProvider.OpenAsync())
            {

                using (var trx = connection.BeginTransaction())
                {
                    // expediente
                    var paramExpediente = new DynamicParameters();
                    paramExpediente.Add("@IdExpediente", expediente.IdExpediente);
                    paramExpediente.Add("@Emisor", expediente.Emisor);
                    paramExpediente.Add("@Descripcion", expediente.Descripcion);
                    paramExpediente.Add("@IdOrigen", expediente.IdOrigen);
                    paramExpediente.Add("@IdProceso", expediente.IdProceso);

                    await connection.ExecuteAsync(actualizarExpedienteSQL, paramExpediente, trx);

                    trx.Commit();
                }
            }

            return expediente.IdExpediente;
        }

        public async Task<List<ExpedienteRequisitosModelo>> ListaRequisitosGestion(int idExpediente)
        {

            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"select rg.id_Expediente IdExpediente, rg.id_requisito IdRequisito, rg.Requisito, Obligatorio, rg.Observacion, presentado,
                                    rg.Fecha_Registro FechaRegistro, rg.id_entidad IdEntidad, rg.id_usuario_registro IdUsuarioRegistro, ent.nombre_comercial entidad,
                                    ent.direccion
                                    from EXP_EXPEDIENTE_REQUISITOS_GESTION rg, ad_entidades ent
                                    where rg.id_expediente = @idExpediente
                                    and ent.id_entidad = rg.id_entidad";

                var result = await connection.QueryAsync<ExpedienteRequisitosModelo>(sqlQuery, new
                {
                    idExpediente = idExpediente
                });

                return result.ToList();
            }
        }

        public async Task<int> GuardarRequisitoAsync(ExpedienteRequisitosModelo requisito)
        {
            const string actualizarExpedienteSQL = @"                
                UPDATE exp_expediente_requisitos_gestion
	                SET PRESENTADO= :Presentado 
                WHERE ID_EXPEDIENTE= :IdExpediente AND ID_REQUISITO= :IdRequisito";

            using (var connection = await connectionProvider.OpenAsync())
            {

                using (var trx = connection.BeginTransaction())
                {
                    // expediente
                    var paramExpediente = new DynamicParameters();
                    paramExpediente.Add("@IdExpediente", requisito.IdExpediente);
                    paramExpediente.Add("@IdRequisito", requisito.idRequisito);
                    paramExpediente.Add("@Presentado", requisito.Presentado);

                    await connection.ExecuteAsync(actualizarExpedienteSQL, paramExpediente, trx);

                    trx.Commit();
                }
            }

            return requisito.idRequisito;
        }

        public async Task<List<ExpedienteTraslados>> ObtenerTraslados(int idExpediente)
        {

            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"WITH lista_traslados AS (
                                        SELECT 
                                            fecha_traslado,
                                            ROW_NUMBER() OVER (ORDER BY fecha_traslado ASC) AS correlativo
                                        FROM exp_expediente_traslados
                                        WHERE id_expediente = @idExpediente
                                    )
                                    SELECT 
                                        et.id_entidad AS IdEntidad,
                                        et.id_proceso AS IdProceso,
                                        et.id_expediente AS IdExpediente,
                                        et.id_fase_origen AS IdFaseOrigen,
                                        et.nombre_fase_origen AS FaseOrigen,
                                        et.id_fase_destino AS IdFaseDestino,
                                        et.nombre_fase_destino AS FaseDestino,
                                        et.fecha_traslado AS fechaTraslado,
                                        et.observacion AS ObservacionTraslado,
                                        et.id_usuario_registro AS idUsuarioTraslado,
                                        us.PRIMER_NOMBRE + ' ' + us.PRIMER_APELLIDO AS usuarioTraslado,
                                        lt2.fecha_traslado AS FechaFin,
                                        lt.fecha_traslado AS FechaInicio
                                    FROM 
                                        exp_expediente_traslados et
                                        INNER JOIN ad_usuarios us ON us.id_entidad = et.id_entidad AND us.id_usuario = et.id_usuario_registro
                                        INNER JOIN lista_traslados lt ON lt.fecha_traslado = et.fecha_traslado
                                        LEFT JOIN lista_traslados lt2 ON lt2.correlativo = lt.correlativo + 1
                                    WHERE 
                                        et.id_expediente = @idExpediente
                                    ORDER BY 
                                        et.fecha_traslado;";

                var result = await connection.QueryAsync<ExpedienteTraslados>(sqlQuery, new
                {
                    idExpediente = idExpediente
                });

                return result.ToList();
            }
        }

        public async Task<List<ExpedienteAsignaciones>> ObtenerAsignaciones(int idEntidad, int idProceso, int idExpediente, DateTime fechaTraslado)
        {

            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"WITH lista_asignaciones AS (
                                        SELECT 
                                            fecha_asignacion,
                                            ROW_NUMBER() OVER (ORDER BY fecha_asignacion ASC) AS correlativo
                                        FROM exp_expediente_asignaciones
                                        WHERE id_expediente = @idExpediente
                                            AND id_proceso = @idProceso
                                            AND id_entidad = @idEntidad
                                            AND fecha_traslado = @fechaTraslado
                                    ),
                                    lista_traslados AS (
                                        SELECT 
                                            fecha_traslado,
                                            ROW_NUMBER() OVER (ORDER BY fecha_traslado ASC) AS correlativo
                                        FROM exp_expediente_traslados
                                        WHERE id_expediente = @idExpediente
                                            AND id_proceso = @idProceso
                                            AND id_entidad = @idEntidad
                                    )

                                    SELECT 
                                        ea.id_expediente AS idexpediente,
                                        ea.id_proceso AS idproceso,
                                        ea.id_entidad AS identidad,
                                        ea.id_fase AS idfase,
                                        ea.fecha_traslado AS fechatraslado,
                                        ea.fecha_asignacion AS fechaasignacion,
                                        ea.observacion,
                                        ea.id_usuario_registro AS idusuarioregistro,
                                        us1.primer_nombre + ' ' + us1.primer_apellido AS usuarioregistro,
                                        ea.id_usuario_asignado AS idusuarioasignado,
                                        us2.primer_nombre + ' ' + us2.primer_apellido AS usuarioasignado,
                                        ea.fecha_limite_atencion AS FechaLimiteAtencion,
                                        ea.id_tipo_operacion AS idtipooperacion,
                                        cto.tipo_operacion AS TipoOperacion,
                                        la.fecha_asignacion AS fechaInicio,
                                        ISNULL(la2.fecha_asignacion, lt2.fecha_traslado) AS fechaFin
                                    FROM 
                                        exp_expediente_asignaciones ea
                                        INNER JOIN ad_usuarios us1 
                                            ON us1.id_entidad = ea.id_entidad AND us1.id_usuario = ea.id_usuario_registro
                                        LEFT JOIN ad_usuarios us2 
                                            ON us2.id_entidad = ea.id_entidad AND us2.id_usuario = ea.id_usuario_asignado
                                        INNER JOIN ca_tipo_operacion cto 
                                            ON cto.id_tipo_operacion = ea.id_tipo_operacion
                                        INNER JOIN lista_asignaciones la 
                                            ON la.fecha_asignacion = ea.fecha_asignacion
                                        LEFT JOIN lista_asignaciones la2 
                                            ON la2.correlativo = la.correlativo + 1
                                        INNER JOIN lista_traslados lt 
                                            ON lt.fecha_traslado = ea.fecha_traslado
                                        LEFT JOIN lista_traslados lt2 
                                            ON lt2.correlativo = lt.correlativo + 1
                                    WHERE 
                                        ea.id_expediente = @idExpediente
                                        AND ea.id_proceso = @idProceso
                                        AND ea.id_entidad = @idEntidad
                                        AND ea.fecha_traslado = @fechaTraslado;";

                var result = await connection.QueryAsync<ExpedienteAsignaciones>(sqlQuery, new
                {
                    idExpediente = idExpediente,
                    idEntidad = idEntidad,
                    idProceso = idProceso,
                    fechaTraslado = fechaTraslado
                });

                return result.ToList();
            }
        }

        public async Task<List<ExpedienteAnotacionesModelo>> ObtenerAnotaciones(int idEntidad, int idProceso, int idExpediente, DateTime fechaTraslado, DateTime? fechaAsignacion)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"SELECT ea.id_entidad IdEntidad, ea.id_proceso IdProceso, ea.id_expediente IdExpediente, ea.id_fase IdFase, ea.fecha_traslado FechaTraslado, 
                                    ea.Fecha_asignacion FechaAsignacion, ea.id_anotacion IdAnotacion, ea.anotacion,
                                    ea.id_usuario IdUsuarioRegistro,us.primer_nombre || ' ' || primer_apellido UsuarioRegistro,  ea.fecha_registro FechaRegistro,
                                    ea.id_tipo_acceso IdPrivacidad, ta.nombre Privacidad
                                    FROM EXP_EXPEDIENTE_ANOTACIONES ea, ad_usuarios us, ca_tipo_acceso ta
                                    where ta.id_tipo_acceso = ea.id_tipo_acceso 
                                    and us.id_usuario = ea.id_usuario
                                    and us.ID_ENTIDAD = ea.id_entidad
                                    and ea.id_entidad = :idEntidad
                                    and ea.id_proceso = :idProceso
                                    and ea.id_expediente = :idExpediente
                                    and ea.fecha_traslado = :fechaTraslado
                                    and ea.fecha_asignacion = nvl(:fechaAsignacion, ea.fecha_asignacion)
                                    order by ea.fecha_traslado, ea.fecha_asignacion";

                var result = await connection.QueryAsync<ExpedienteAnotacionesModelo>(sqlQuery, new
                {
                    idExpediente = idExpediente,
                    idEntidad = idEntidad,
                    idProceso = idProceso,
                    fechaTraslado = fechaTraslado,
                    fechaAsignacion = fechaAsignacion
                });

                return result.ToList();
            }
        }

        public async Task<ExpedienteAsignacionInternaModelo> ObtenerAsignacionInterna(int idEntidad, int idProceso, int idExpediente)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"
                WITH asignacion AS (
                    SELECT MAX(fecha_asignacion) AS fecha_asignacion
                    FROM exp_expediente_asignaciones ea
                    WHERE ea.id_entidad = @idEntidad
                    AND ea.id_proceso = @idProceso
                    AND ea.id_expediente = @idExpediente
                )
                SELECT 
                    ea.id_entidad AS idEntidad, 
                    ea.id_proceso AS idProceso, 
                    ea.id_expediente AS idExpediente, 
                    ea.fecha_asignacion AS fechaAsignacion, 
                    ea.fecha_limite_atencion AS fechaLimiteAtencion, 
                    ea.observacion, 
                    ea.id_usuario_registro AS idUsuarioRegistro, 
                    us.PRIMER_NOMBRE + ' ' + us.primer_apellido AS usuarioRegistro 
                FROM 
                    exp_expediente_asignaciones ea
                JOIN 
                    asignacion asi 
                    ON ea.fecha_asignacion = asi.fecha_asignacion
                JOIN 
                    ad_usuarios us 
                    ON us.id_entidad = ea.id_entidad 
                    AND us.id_usuario = ea.id_usuario_registro
                WHERE 
                    ea.id_entidad = @idEntidad
                    AND ea.id_proceso = @idProceso
                    AND ea.id_expediente = @idExpediente
                ";

                var result = await connection.QueryAsync<ExpedienteAsignacionInternaModelo>(sqlQuery, new
                {
                    idExpediente = idExpediente,
                    idEntidad = idEntidad,
                    idProceso = idProceso
                });

                return result.FirstOrDefault();
            }
        }

        public async Task<List<FaseTrasladoModelo>> ObtenerFasesUsuariosTraslado(int idExpediente)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"
                WITH usuarios AS (
                    SELECT 
                        ptu.id_fase_destino, 
                        ptu.id_entidad, 
                        ptu.id_proceso,
                        pfu.id_usuario,
                        us.primer_nombre + ' ' + us.primer_apellido AS usuario
                    FROM 
                        proceso_transicion_usuarios ptu
                    JOIN 
                        exp_expediente ex ON ptu.id_entidad = ex.id_entidad
                        AND ptu.id_proceso = ex.id_proceso
                        AND ptu.id_fase_origen = ex.id_fase_actual
                        AND ptu.id_usuario = ex.id_usuario_asignado
                    JOIN 
                        proceso_fase_usuarios pfu ON pfu.id_entidad = ptu.id_entidad
                        AND pfu.id_proceso = ptu.id_proceso
                        AND pfu.id_fase = ptu.id_fase_destino
                    JOIN 
                        ad_usuarios us ON us.id_entidad = pfu.id_entidad
                        AND us.id_usuario = pfu.id_usuario
                    WHERE 
                        us.activo = 1
                        AND pfu.recepcion_traslado = 1
                        AND ex.id_expediente = @idexpediente
                        AND NOT EXISTS (
                            SELECT 1
                            FROM historico_inhabilitaciones hi
                            WHERE hi.id_entidad = us.id_entidad
                            AND hi.id_usuario = us.id_usuario
                            AND CAST(GETDATE() AS DATE) BETWEEN hi.fecha_inicio AND hi.fecha_fin
                        )
                )
                SELECT 
                    ptu.id_fase_destino AS idfase, 
                    ptu.id_entidad AS identidad,
                    ptu.id_proceso AS idproceso, 
                    pf.nombre AS fase,
                    pf.asignacion_obligatoria AS asignacionobligatoria,
                    pf.id_tipo_fase AS idtipofase, 
                    tf.nombre AS tipofase,
                    pf.acuse_recibo_obligatorio AS acuserecibido, 
                    ptu.id_fase_destino AS idfase,
                    usa.id_usuario AS idUsuario, 
                    usa.usuario
                FROM 
                    proceso_transiciones pt
                JOIN 
                    proceso_transicion_usuarios ptu ON pt.id_fase_destino = ptu.id_fase_destino
                    AND pt.id_fase_origen = ptu.id_fase_origen
                    AND pt.id_proceso = ptu.id_proceso
                    AND pt.id_entidad = ptu.id_entidad
                JOIN 
                    exp_expediente ex ON ptu.id_entidad = ex.id_entidad
                    AND ptu.id_proceso = ex.id_proceso
                    AND ptu.id_fase_origen = ex.id_fase_actual
                    AND ptu.id_usuario = ex.id_usuario_asignado
                JOIN 
                    proceso_fase pf ON pf.id_fase = ptu.id_fase_destino
                    AND pf.id_proceso = ptu.id_proceso
                    AND pf.id_entidad = ptu.id_entidad
                JOIN 
                    ca_tipo_fase tf ON tf.id_tipo_fase = pf.id_tipo_fase
                LEFT JOIN 
                    usuarios usa ON usa.id_fase_destino = ptu.id_fase_destino
                    AND usa.id_proceso = pf.id_proceso
                    AND usa.id_entidad = pf.id_entidad
                WHERE 
                    pt.activa = 1
                    AND ex.id_expediente = @idexpediente

                ";

                var fasesDictionary = new Dictionary<int, FaseTrasladoModelo>();
                var usuariosDictionary = new Dictionary<int, UsuarioFase>();

                var result = await connection.QueryAsync<FaseTrasladoModelo, UsuarioFase, FaseTrasladoModelo>(sqlQuery, (fase, usuarios) =>
                {
                    FaseTrasladoModelo fasesTraslado;

                    if (!fasesDictionary.TryGetValue(fase.IdFase, out fasesTraslado))
                    {
                        fasesTraslado = fase;
                        fasesTraslado.ListaUsuarios = new List<UsuarioFase>();
                        fasesDictionary.Add(fasesTraslado.IdFase, fasesTraslado);
                    }

                    fasesTraslado.ListaUsuarios.AsList().Add(usuarios);

                    return fasesTraslado;
                },
                   new { idExpediente }, splitOn: "IdFase");

                return result.Distinct().ToList();
            }
        }

        public async Task<bool> TrasladarExpediente(TrasladoModelo expediente)
        {

            if (await PuedeOperarse(expediente.IdEntidad, expediente.IdExpediente, expediente.IdUsuarioRegistro))
            {

                string insertarTraslado = @"Insert into EXP_EXPEDIENTE_TRASLADOS
                                           (ID_EXPEDIENTE, ID_PROCESO, ID_ENTIDAD, ID_FASE_ORIGEN, NOMBRE_FASE_ORIGEN, ID_FASE_DESTINO, NOMBRE_FASE_DESTINO, FECHA_TRASLADO, OBSERVACION, ID_USUARIO_REGISTRO) Values
                                           (@idExpediente, @idProceso, @idEntidad, IIF(@idFaseOrigen = 0,null,@idFaseOrigen), @faseOrigen, @idFaseDestino, @faseDestino, @fechaTraslado, @observacion, @idUsuarioRegistro)";

                string insertarAsignacion = @"Insert into EXP_EXPEDIENTE_ASIGNACIONES
                                           (ID_EXPEDIENTE, ID_PROCESO, ID_ENTIDAD, ID_FASE, FECHA_TRASLADO, FECHA_ASIGNACION, OBSERVACION, ID_USUARIO_REGISTRO, FECHA_LIMITE_ATENCION, 
                                            ID_TIPO_OPERACION, ID_USUARIO_ASIGNADO) Values
                                           (@idExpediente, @idProceso, @idEntidad, @idFaseDestino, @fechaTraslado, @fechaAsignacion, @observacion, @idUsuarioRegistro, IIF(@fechaLimiteAtencion = null, null,@fechaLimiteAtencion), 
                                            @idTipoOperacion, @idUsuarioAsignado)";

                string actualizarExpediente = @"update exp_expediente set id_fase_actual = @idFaseDestino, fecha_traslado = @fechaTraslado, fecha_asignacion = @fechaAsignacion, id_usuario_asignado = @idUsuarioAsignado, id_tipo_operacion = @idTipoOperacion where id_expediente = @idExpediente";

                string actualizarExpedienteTO = @"update exp_expediente set id_tipo_operacion = @idTipoOperacion where id_expediente = @idExpediente";

                using (var connection = await connectionProvider.OpenAsync())
                {
                    using (var trx = connection.BeginTransaction())
                    {
                        var param = new DynamicParameters();
                        param.Add("@idProceso", expediente.IdProceso);
                        param.Add("@idEntidad", expediente.IdEntidad);
                        param.Add("@idExpediente", expediente.IdExpediente);
                        param.Add("@idFaseOrigen", expediente.IdFaseOrigen);
                        param.Add("@faseOrigen", expediente.FaseOrigen);
                        param.Add("@idFaseDestino", expediente.IdFaseDestino);
                        param.Add("@faseDestino", expediente.FaseDestino);
                        param.Add("@fechaTraslado", expediente.FechaTraslado);
                        param.Add("@observacion", expediente.Observacion);
                        param.Add("@idUsuarioRegistro", expediente.IdUsuarioRegistro);

                        await connection.ExecuteAsync(insertarTraslado, param, trx);

                        param.Add("@fechaAsignacion", expediente.FechaTraslado);
                        param.Add("@fechaLimiteAtencion", (expediente.FechaLimiteAtencion < DateTime.Today) ? (DateTime?)null : expediente.FechaLimiteAtencion);
                        param.Add("@idTipoOperacion", expediente.IdTipoOperacion);
                        param.Add("@idUsuarioAsignado", expediente.IdUsuarioAsignado == -1 ? (int?)null : expediente.IdUsuarioAsignado);

                        await connection.ExecuteAsync(insertarAsignacion, param, trx);

                        if (expediente.IdTipoOperacion != TipoOperacion.TrasladoPendienteDeRecibir)
                            await connection.ExecuteAsync(actualizarExpediente, param, trx);
                        else
                            await connection.ExecuteAsync(actualizarExpedienteTO, param, trx);

                        trx.Commit();
                    }

                }
                return true;
            }
            else
            {
                return false;
            }


        }

        public async Task<bool> TrasladarMasivamenteExpedientes(TrasladoModelo[] expedientes)
        {

            string insertarTraslado = @"Insert into EXP_EXPEDIENTE_TRASLADOS
                                       (ID_EXPEDIENTE, ID_PROCESO, ID_ENTIDAD, ID_FASE_ORIGEN, NOMBRE_FASE_ORIGEN, ID_FASE_DESTINO, NOMBRE_FASE_DESTINO, FECHA_TRASLADO, OBSERVACION, ID_USUARIO_REGISTRO) Values
                                       (:idExpediente, :idProceso, :idEntidad, decode(:idFaseOrigen,0,null,:idFaseOrigen), :faseOrigen, :idFaseDestino, :faseDestino, :fechaTraslado, :observacion, :idUsuarioRegistro)";

            string insertarAsignacion = @"Insert into EXP_EXPEDIENTE_ASIGNACIONES
                                       (ID_EXPEDIENTE, ID_PROCESO, ID_ENTIDAD, ID_FASE, FECHA_TRASLADO, FECHA_ASIGNACION, OBSERVACION, ID_USUARIO_REGISTRO, FECHA_LIMITE_ATENCION, 
                                        ID_TIPO_OPERACION, ID_USUARIO_ASIGNADO) Values
                                       (:idExpediente, :idProceso, :idEntidad, :idFaseDestino, :fechaTraslado, :fechaAsignacion, :observacion, :idUsuarioRegistro, DECODE(:fechaLimiteAtencion, null, null,trunc(:fechaLimiteAtencion)), 
                                        :idTipoOperacion, :idUsuarioAsignado)";

            string actualizarExpediente = @"update exp_expediente set id_fase_actual = :idFaseDestino, fecha_traslado = :fechaTraslado, fecha_asignacion = :fechaAsignacion, id_usuario_asignado = :idUsuarioAsignado, id_tipo_operacion = :idTipoOperacion where id_expediente = :idExpediente";

            string actualizarExpedienteTO = @"update exp_expediente set id_tipo_operacion = :idTipoOperacion where id_expediente = :idExpediente";


            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {

                    foreach (var expediente in expedientes)
                    {
                        var param = new DynamicParameters();
                        param.Add("@idProceso", expediente.IdProceso);
                        param.Add("@idEntidad", expediente.IdEntidad);
                        param.Add("@idExpediente", expediente.IdExpediente);
                        param.Add("@idFaseOrigen", expediente.IdFaseOrigen);
                        param.Add("@faseOrigen", expediente.FaseOrigen);
                        param.Add("@idFaseDestino", expediente.IdFaseDestino);
                        param.Add("@faseDestino", expediente.FaseDestino);
                        param.Add("@fechaTraslado", expediente.FechaTraslado);
                        param.Add("@observacion", expediente.Observacion);
                        param.Add("@idUsuarioRegistro", expediente.IdUsuarioRegistro);

                        await connection.ExecuteAsync(insertarTraslado, param, trx);

                        param.Add("@fechaAsignacion", expediente.FechaTraslado);
                        param.Add("@fechaLimiteAtencion", (expediente.FechaLimiteAtencion < DateTime.Today) ? (DateTime?)null : expediente.FechaLimiteAtencion);
                        param.Add("@idTipoOperacion", expediente.IdTipoOperacion);
                        param.Add("@idUsuarioAsignado", expediente.IdUsuarioAsignado == -1 ? (int?)null : expediente.IdUsuarioAsignado);

                        await connection.ExecuteAsync(insertarAsignacion, param, trx);

                        if (expediente.IdTipoOperacion != TipoOperacion.TrasladoPendienteDeRecibir)
                            await connection.ExecuteAsync(actualizarExpediente, param, trx);
                        else
                            await connection.ExecuteAsync(actualizarExpedienteTO, param, trx);
                    }

                    trx.Commit();
                }

            }

            return true;
        }

        public async Task<List<UsuarioFase>> ObtenerUsuariosAsignacionIntera(int idEntidad, int idProceso, int idFase, int idUsuario)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"select us.id_usuario as idUsuario, us.primer_nombre || ' ' || us.primer_apellido as usuario
                                    from proceso_fase_usuarios pfu, ad_usuarios us
                                    where us.activo = 1 
                                    and us.id_usuario = pfu.id_usuario
                                    and us.ID_ENTIDAD = pfu.id_entidad 
                                    and pfu.id_entidad = :idEntidad
                                    and pfu.id_proceso = :idProceso
                                    and pfu.id_fase = :idFase
                                    and not exists ( select id_entidad, id_usuario
                                                        from historico_inhabilitaciones hi
                                                        where hi.id_entidad = :idEntidad
                                                        and hi.id_usuario = us.id_usuario
                                                        and trunc(sysdate) between trunc(hi.fecha_inicio) and trunc(hi.fecha_fin))
                                    and pfu.id_usuario <> :idUsuarioConsulta";

                var result = await connection.QueryAsync<UsuarioFase>(sqlQuery, new
                {
                    idEntidad = idEntidad,
                    idProceso = idProceso,
                    idFase = idFase,
                    idUsuarioConsulta = idUsuario
                });

                return result.ToList();
            }
        }

        public async Task<bool> AsignarExpediente(AsignacionModelo expediente)
        {
            if (await PuedeOperarse(expediente.IdEntidad, expediente.IdExpediente, expediente.IdUsuarioRegistro))
            {
                string insertarAsignacion = @"Insert into EXP_EXPEDIENTE_ASIGNACIONES
                                       (ID_EXPEDIENTE, ID_PROCESO, ID_ENTIDAD, ID_FASE, FECHA_TRASLADO, FECHA_ASIGNACION, OBSERVACION, ID_USUARIO_REGISTRO, FECHA_LIMITE_ATENCION, 
                                        ID_TIPO_OPERACION, ID_USUARIO_ASIGNADO) Values
                                       (:idExpediente, :idProceso, :idEntidad, :idFaseDestino, :fechaTraslado, :fechaAsignacion, :observacion, :idUsuarioRegistro, DECODE(:fechaLimiteAtencion, null, null,trunc(:fechaLimiteAtencion)), 
                                        :idTipoOperacion, :idUsuarioAsignado)";

                string actualizarExpediente = @"update exp_expediente set fecha_asignacion = :fechaAsignacion, id_usuario_asignado = :idUsuarioAsignado, id_tipo_operacion = :idTipoOperacion where id_expediente = :idExpediente";

                using (var connection = await connectionProvider.OpenAsync())
                {
                    using (var trx = connection.BeginTransaction())
                    {
                        var param = new DynamicParameters();
                        param.Add("@idEntidad", expediente.IdEntidad);
                        param.Add("@idProceso", expediente.IdProceso);
                        param.Add("@idExpediente", expediente.IdExpediente);
                        param.Add("@idFaseDestino", expediente.IdFaseDestino);
                        param.Add("@fechaTraslado", expediente.FechaTraslado);
                        param.Add("@fechaAsignacion", expediente.FechaAsignacion);
                        param.Add("@observacion", expediente.Observacion);
                        param.Add("@idUsuarioRegistro", expediente.IdUsuarioRegistro);
                        param.Add("@fechaLimiteAtencion", (expediente.FechaLimiteAtencion < DateTime.Today) ? (DateTime?)null : expediente.FechaLimiteAtencion);
                        param.Add("@idTipoOperacion", expediente.IdTipoOperacion);
                        param.Add("@idUsuarioAsignado", expediente.IdUsuarioAsignado);

                        await connection.ExecuteAsync(insertarAsignacion, param, trx);

                        if (expediente.IdTipoOperacion != TipoOperacion.TrasladoPendienteDeRecibir)
                        {
                            await connection.ExecuteAsync(actualizarExpediente, param, trx);
                        }



                        trx.Commit();
                    }

                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> AsignarMasivamenteExpedientes(AsignacionModelo[] expedientes)
        {
            string insertarAsignacion = @"Insert into EXP_EXPEDIENTE_ASIGNACIONES
                                       (ID_EXPEDIENTE, ID_PROCESO, ID_ENTIDAD, ID_FASE, FECHA_TRASLADO, FECHA_ASIGNACION, OBSERVACION, ID_USUARIO_REGISTRO, FECHA_LIMITE_ATENCION, 
                                        ID_TIPO_OPERACION, ID_USUARIO_ASIGNADO) Values
                                       (:idExpediente, :idProceso, :idEntidad, :idFaseDestino, :fechaTraslado, :fechaAsignacion, :observacion, :idUsuarioRegistro, DECODE(:fechaLimiteAtencion, null, null,trunc(:fechaLimiteAtencion)), 
                                        :idTipoOperacion, :idUsuarioAsignado)";

            string actualizarExpediente = @"update exp_expediente set fecha_asignacion = :fechaAsignacion, id_usuario_asignado = :idUsuarioAsignado, id_tipo_operacion = :idTipoOperacion where id_expediente = :idExpediente";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    foreach (var expediente in expedientes)
                    {
                        var param = new DynamicParameters();
                        param.Add("@idEntidad", expediente.IdEntidad);
                        param.Add("@idProceso", expediente.IdProceso);
                        param.Add("@idExpediente", expediente.IdExpediente);
                        param.Add("@idFaseDestino", expediente.IdFaseDestino);
                        param.Add("@fechaTraslado", expediente.FechaTraslado);
                        param.Add("@fechaAsignacion", expediente.FechaAsignacion);
                        param.Add("@observacion", expediente.Observacion);
                        param.Add("@idUsuarioRegistro", expediente.IdUsuarioRegistro);
                        param.Add("@fechaLimiteAtencion", (expediente.FechaLimiteAtencion < DateTime.Today) ? (DateTime?)null : expediente.FechaLimiteAtencion);
                        param.Add("@idTipoOperacion", expediente.IdTipoOperacion);
                        param.Add("@idUsuarioAsignado", expediente.IdUsuarioAsignado);

                        await connection.ExecuteAsync(insertarAsignacion, param, trx);

                        if (expediente.IdTipoOperacion != TipoOperacion.TrasladoPendienteDeRecibir)
                        {
                            await connection.ExecuteAsync(actualizarExpediente, param, trx);
                        }

                    }

                    trx.Commit();
                }

            }
            return true;

        }

        public async Task<List<ExpedienteAnotacionesModelo>> ObtenerAnotacionesEnFasePorUsuarioAsync(ExpedienteModelo expediente)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"
                SELECT ea.id_entidad AS IdEntidad,
                        ea.id_proceso AS IdProceso,
                        ea.id_expediente AS IdExpediente,
                        ea.id_fase AS IdFase,
                        ea.fecha_traslado AS FechaTraslado,
                        ea.fecha_asignacion AS FechaAsignacion,
                        ea.id_anotacion AS IdAnotacion,
                        ea.fecha_registro AS FechaRegistro,
                        ea.anotacion,
                        ea.id_tipo_acceso AS IdPrivacidad,
                        ta.NOMBRE AS Privacidad
                FROM exp_Expediente_Anotaciones ea
                JOIN ca_tipo_acceso ta ON ta.ID_TIPO_ACCESO = ea.id_tipo_acceso
                WHERE ea.id_entidad = @idEntidad
                    AND ea.id_proceso = @idProceso
                    AND ea.id_expediente = @idExpediente
                    AND ea.id_fase = @idFase
                    AND ea.fecha_traslado = @fechaTraslado
                    AND ea.fecha_asignacion = @fechaAsignacion
                    AND ea.id_usuario = @idUsuario
                ORDER BY ea.fecha_registro
                ";

                var result = await connection.QueryAsync<ExpedienteAnotacionesModelo>(sqlQuery, new
                {
                    idExpediente = expediente.IdExpediente,
                    idEntidad = expediente.IdEntidad,
                    idProceso = expediente.IdProceso,
                    idFase = expediente.IdFaseActual,
                    fechaTraslado = expediente.FechaTraslado,
                    fechaAsignacion = expediente.FechaAsignacion,
                    idUsuario = expediente.IdUsuarioAsignado
                });

                return result.ToList();
            }
        }

        public async Task<bool> CrearAnotacionAsync(ExpedienteAnotacionesModelo anotacion)
        {
            string insertarAsignacion = @"Insert into EXP_EXPEDIENTE_ANOTACIONES
                                       (ID_ENTIDAD, ID_PROCESO, ID_EXPEDIENTE, ID_FASE, FECHA_TRASLADO, FECHA_ASIGNACION, ID_ANOTACION, ANOTACION, ID_USUARIO, FECHA_REGISTRO, ID_TIPO_ACCESO)
                                        Values
                                       (@idEntidad, @idProceso, @idExpediente, @idFase, @fechaTraslado, @fechaAsignacion, 
                                        (select ISNULL(MAX(id_anotacion),0) +1
                                            from exp_Expediente_Anotaciones ea
                                            where ea.id_entidad = @idEntidad
                                            and ea.id_proceso = @idProceso
                                            and ea.id_expediente = @idExpediente
                                            and ea.id_fase = @idFase
                                            and ea.fecha_traslado = @fechaTraslado
                                            and ea.fecha_asignacion = @fechaAsignacion), @anotacion, @idUsuario, @fechaRegistro, @idPrivacidad)";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@idEntidad", anotacion.IdEntidad);
                    param.Add("@idProceso", anotacion.IdProceso);
                    param.Add("@idExpediente", anotacion.IdExpediente);
                    param.Add("@idFase", anotacion.IdFase);
                    param.Add("@fechaTraslado", anotacion.FechaTraslado);
                    param.Add("@fechaAsignacion", anotacion.FechaAsignacion);
                    param.Add("@anotacion", anotacion.Anotacion);
                    param.Add("@idUsuario", anotacion.IdUsuarioRegistro);
                    param.Add("@fechaRegistro", anotacion.FechaRegistro);
                    param.Add("@idPrivacidad", anotacion.idPrivacidad);

                    await connection.ExecuteAsync(insertarAsignacion, param, trx);

                    trx.Commit();
                }
            }

            return true;
        }

        public async Task<bool> EliminarAnotacionAsync(ExpedienteAnotacionesModelo anotacion)
        {
            const string eliminarFaseSQL = @"                
                DELETE EXP_EXPEDIENTE_ANOTACIONES
                WHERE id_entidad = @idEntidad
                and id_proceso = @idProceso
                and id_expediente = @idExpediente
                and id_fase = @idFase
                and fecha_traslado = @fechaTraslado
                and fecha_asignacion = @fechaAsignacion
                and id_anotacion = @idAnotacion";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var param = new DynamicParameters();
                param.Add("@idEntidad", anotacion.IdEntidad);
                param.Add("@idProceso", anotacion.IdProceso);
                param.Add("@idExpediente", anotacion.IdExpediente);
                param.Add("@idFase", anotacion.IdFase);
                param.Add("@fechaTraslado", anotacion.FechaTraslado);
                param.Add("@fechaAsignacion", anotacion.FechaAsignacion);
                param.Add("@idAnotacion", anotacion.IdAnotacion);

                var response = await connection.ExecuteAsync(eliminarFaseSQL, param);

                return response == 1 ? true : false;
            }

        }

        public async Task<bool> ConfirmarRecepcionExpedienteAsync(AsignacionModelo expediente)
        {
            if (await ExpedienteAsignadoAUsuario(expediente.IdEntidad, expediente.IdExpediente, expediente.IdUsuarioRegistro))
            {
                string actualizarAsignacion = @"update exp_expediente_asignaciones
                                        set id_Tipo_Operacion = @idTipoOperacion,
                                        fecha_operacion = @fechaOperacion
                                        where id_entidad = @idEntidad
                                        and id_Proceso = @idProceso
                                        and id_expediente = @idExpediente
                                        and id_fase = @idFase
                                        and fecha_traslado = @fechaTraslado
                                        and fecha_asignacion = @fechaAsignacion";

                string actualizarAsignacion2 = @"update exp_expediente
                                        set id_usuario_asignado = @idUsuarioAsignado,
                                        id_Fase_actual = @idFase,
                                        fecha_traslado = @fechaTraslado,
                                        fecha_asignacion = @fechaAsignacion,
                                        id_Tipo_operacion = @idTipoOperacion
                                        where id_entidad = @idEntidad
                                        and id_Proceso = @idProceso
                                        and id_expediente = @idExpediente";

                using (var connection = await connectionProvider.OpenAsync())
                {
                    using (var trx = connection.BeginTransaction())
                    {
                        var param = new DynamicParameters();
                        param.Add("@idProceso", expediente.IdProceso);
                        param.Add("@idEntidad", expediente.IdEntidad);
                        param.Add("@idExpediente", expediente.IdExpediente);
                        param.Add("@idFase", expediente.IdFaseDestino);
                        param.Add("@fechaTraslado", expediente.FechaTraslado);
                        param.Add("@fechaAsignacion", expediente.FechaAsignacion);
                        param.Add("@idTipoOperacion", expediente.IdTipoOperacion);
                        param.Add("@fechaOperacion", expediente.FechaOperacion);
                        param.Add("@idUsuarioAsignado", expediente.IdUsuarioAsignado);

                        await connection.ExecuteAsync(actualizarAsignacion, param, trx);

                        await connection.ExecuteAsync(actualizarAsignacion2, param, trx);

                        trx.Commit();
                    }

                }

                return true;
            }
            else
                return false;
        }

        public async Task<bool> ConfirmarMasivamenteRecepcionExpedientesAsync(AsignacionModelo[] expedientes)
        {

            string actualizarAsignacion = @"update exp_expediente_asignaciones
                                        set id_Tipo_Operacion = :idTipoOperacion,
                                        fecha_operacion = :fechaOperacion
                                        where id_entidad = :idEntidad
                                        and id_Proceso = :idProceso
                                        and id_expediente = :idExpediente
                                        and id_fase = :idFase
                                        and fecha_traslado = :fechaTraslado
                                        and fecha_asignacion = :fechaAsignacion";

            string actualizarAsignacion2 = @"update exp_expediente
                                        set id_usuario_asignado = :idUsuarioAsignado,
                                        id_Fase_actual = :idFase,
                                        fecha_traslado = :fechaTraslado,
                                        fecha_asignacion = :fechaAsignacion,
                                        id_Tipo_operacion = :idTipoOperacion
                                        where id_entidad = :idEntidad
                                        and id_Proceso = :idProceso
                                        and id_expediente = :idExpediente";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    foreach (var expediente in expedientes)
                    {
                        var param = new DynamicParameters();
                        param.Add("@idProceso", expediente.IdProceso);
                        param.Add("@idEntidad", expediente.IdEntidad);
                        param.Add("@idExpediente", expediente.IdExpediente);
                        param.Add("@idFase", expediente.IdFaseDestino);
                        param.Add("@fechaTraslado", expediente.FechaTraslado);
                        param.Add("@fechaAsignacion", expediente.FechaAsignacion);
                        param.Add("@idTipoOperacion", expediente.IdTipoOperacion);
                        param.Add("@fechaOperacion", expediente.FechaOperacion);
                        param.Add("@idUsuarioAsignado", expediente.IdUsuarioAsignado);

                        await connection.ExecuteAsync(actualizarAsignacion, param, trx);

                        await connection.ExecuteAsync(actualizarAsignacion2, param, trx);
                    }
                    trx.Commit();
                }
            }
            return true;
        }

        public async Task<bool> RechazarRecepcionExpedienteAsync(AsignacionModelo expediente)
        {
            if (await ExpedienteAsignadoAUsuario(expediente.IdEntidad, expediente.IdExpediente, expediente.IdUsuarioRegistro))
            {
                string actualizarAsignacion = @"update exp_expediente_asignaciones
                                        set id_Tipo_Operacion = :idTipoOperacion,
                                        observacion = :observacion,
                                        fecha_operacion = :fechaOperacion
                                        where id_entidad = :idEntidad
                                        and id_Proceso = :idProceso
                                        and id_expediente = :idExpediente
                                        and id_fase = :idFase
                                        and fecha_traslado = :fechaTraslado
                                        and fecha_asignacion = :fechaAsignacion";

                string actualizarAsignacion2 = @"update exp_expediente
                                        set id_Tipo_operacion = :idTipoOperacion
                                        where id_entidad = :idEntidad
                                        and id_Proceso = :idProceso
                                        and id_expediente = :idExpediente";

                using (var connection = await connectionProvider.OpenAsync())
                {
                    using (var trx = connection.BeginTransaction())
                    {
                        var param = new DynamicParameters();
                        param.Add("@idProceso", expediente.IdProceso);
                        param.Add("@idEntidad", expediente.IdEntidad);
                        param.Add("@idExpediente", expediente.IdExpediente);
                        param.Add("@idFase", expediente.IdFaseDestino);
                        param.Add("@fechaTraslado", expediente.FechaTraslado);
                        param.Add("@fechaAsignacion", expediente.FechaAsignacion);
                        param.Add("@idTipoOperacion", expediente.IdTipoOperacion);
                        param.Add("@observacion", expediente.Observacion);
                        param.Add("@fechaOperacion", expediente.FechaOperacion);

                        await connection.ExecuteAsync(actualizarAsignacion, param, trx);

                        await connection.ExecuteAsync(actualizarAsignacion2, param, trx);

                        trx.Commit();
                    }

                }

                return true;
            }
            else
                return false;
        }
        public async Task<bool> RechazarMasivamenteRecepcionExpedientesAsync(AsignacionModelo[] expedientes)
        {

            string actualizarAsignacion = @"update exp_expediente_asignaciones
                                        set id_Tipo_Operacion = :idTipoOperacion,
                                        observacion = :observacion,
                                        fecha_operacion = :fechaOperacion
                                        where id_entidad = :idEntidad
                                        and id_Proceso = :idProceso
                                        and id_expediente = :idExpediente
                                        and id_fase = :idFase
                                        and fecha_traslado = :fechaTraslado
                                        and fecha_asignacion = :fechaAsignacion";

            string actualizarAsignacion2 = @"update exp_expediente
                                        set id_Tipo_operacion = :idTipoOperacion
                                        where id_entidad = :idEntidad
                                        and id_Proceso = :idProceso
                                        and id_expediente = :idExpediente";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    foreach (var expediente in expedientes)
                    {
                        var param = new DynamicParameters();
                        param.Add("@idProceso", expediente.IdProceso);
                        param.Add("@idEntidad", expediente.IdEntidad);
                        param.Add("@idExpediente", expediente.IdExpediente);
                        param.Add("@idFase", expediente.IdFaseDestino);
                        param.Add("@fechaTraslado", expediente.FechaTraslado);
                        param.Add("@fechaAsignacion", expediente.FechaAsignacion);
                        param.Add("@idTipoOperacion", expediente.IdTipoOperacion);
                        param.Add("@observacion", expediente.Observacion);
                        param.Add("@fechaOperacion", expediente.FechaOperacion);

                        await connection.ExecuteAsync(actualizarAsignacion, param, trx);

                        await connection.ExecuteAsync(actualizarAsignacion2, param, trx);
                    }

                    trx.Commit();
                }

            }

            return true;
        }

        public async Task<bool> TomarExpedienteAsync(AsignacionModelo expediente)
        {
            if (await ExpedienteDisponible(expediente.IdEntidad, expediente.IdExpediente))
            {

                string actualizarAsignacion = @"update exp_expediente_asignaciones
                                        set id_Tipo_Operacion = :idTipoOperacion,
                                        id_usuario_asignado = :idUsuarioAsignado,
                                        fecha_operacion = :fechaOperacion
                                        where id_entidad = :idEntidad
                                        and id_Proceso = :idProceso
                                        and id_expediente = :idExpediente
                                        and id_fase = :idFase
                                        and fecha_traslado = :fechaTraslado
                                        and fecha_asignacion = :fechaAsignacion";

                string actualizarAsignacion2 = @"update exp_expediente
                                        set id_usuario_asignado = :idUsuarioAsignado,
                                        id_tipo_operacion = :idTipoOperacion
                                        where id_entidad = :idEntidad
                                        and id_Proceso = :idProceso
                                        and id_expediente = :idExpediente";

                using (var connection = await connectionProvider.OpenAsync())
                {
                    using (var trx = connection.BeginTransaction())
                    {
                        var param = new DynamicParameters();
                        param.Add("@idProceso", expediente.IdProceso);
                        param.Add("@idEntidad", expediente.IdEntidad);
                        param.Add("@idExpediente", expediente.IdExpediente);
                        param.Add("@idFase", expediente.IdFaseDestino);
                        param.Add("@fechaTraslado", expediente.FechaTraslado);
                        param.Add("@fechaAsignacion", expediente.FechaAsignacion);
                        param.Add("@idTipoOperacion", expediente.IdTipoOperacion);
                        param.Add("@idUsuarioAsignado", expediente.IdUsuarioAsignado);
                        param.Add("@fechaOperacion", expediente.FechaOperacion);

                        await connection.ExecuteAsync(actualizarAsignacion, param, trx);

                        await connection.ExecuteAsync(actualizarAsignacion2, param, trx);

                        trx.Commit();
                    }

                }

                return true;
            }
            else
                return false;
        }

        public async Task<bool> PuedeOperarse(int idEntidad, int idExpediente, int idUsuario)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string strConsulta = @"select count(1)
                                            from exp_expediente
                                            where id_entidad = @idEntidad
                                            and id_expediente = @idExpediente
                                            and id_usuario_asignado = @idUsuario";

                var result = await connection.QueryAsync<int>(strConsulta, new { idEntidad, idExpediente, idUsuario });

                return Convert.ToInt16(result.FirstOrDefault()) == 1 ? true : false;
            }
        }

        public async Task<bool> ExpedienteAsignadoAUsuario(int idEntidad, int idExpediente, int idUsuario)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string strConsulta = @"
                SELECT COUNT(1)
                FROM exp_expediente_asignaciones ea
                JOIN (
                    SELECT MAX(fecha_asignacion) AS fecha
                    FROM exp_expediente_asignaciones
                    WHERE id_entidad = @idEntidad
                    AND id_expediente = @idExpediente
                ) fm ON ea.fecha_asignacion = fm.fecha
                WHERE ea.id_entidad = @idEntidad
                AND ea.id_expediente = @idExpediente
                AND id_usuario_asignado = @idUsuario
                ";

                var result = await connection.QueryAsync<int>(strConsulta, new { idEntidad, idExpediente, idUsuario });

                return Convert.ToInt16(result.FirstOrDefault()) == 1 ? true : false;
            }
        }

        public async Task<bool> ExpedienteDisponible(int idEntidad, int idExpediente)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string strConsulta = @"select count(1) 
                                        from exp_expediente
                                        where id_entidad = :idEntidad
                                        and id_expediente = :idExpediente
                                        and id_tipo_operacion = 6
                                        and id_usuario_asignado is null";

                var result = await connection.QueryAsync<int>(strConsulta, new { idEntidad, idExpediente });

                return Convert.ToInt16(result.FirstOrDefault()) == 1 ? true : false;
            }
        }

        public async Task<bool> VincularExpediente(VinculacionExpedienteModelo expedienteVinculado)
        {

            string insertarVinculacion = @"Insert into exp_expediente_vinculaciones 
                                        (id_expediente, id_Expediente_vinculado,id_entidad, id_usuario, fecha_registro)
                                        values (@idExpediente, @idExpedienteVinculado, @idEntidad, @idUsuario, @fechaRegistro)";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@idEntidad", expedienteVinculado.IdEntidad);
                    param.Add("@idExpediente", expedienteVinculado.IdExpediente);
                    param.Add("@idExpedienteVinculado", expedienteVinculado.IdExpedienteVinculado);
                    param.Add("@idUsuario", expedienteVinculado.IdUsuarioRegistro);
                    param.Add("@fechaRegistro", expedienteVinculado.FechaRegistro);

                    await connection.ExecuteAsync(insertarVinculacion, param, trx);

                    trx.Commit();
                }
            }
            return true;
        }

        public async Task<bool> EliminarVinculacionAsync(int idExpediente, int idExpedienteVinculado)
        {
            const string eliminarVinculacionSQL = @"                
               DELETE exp_expediente_vinculaciones
                WHERE id_expediente  = :idExpediente
                and id_expediente_vinculado = :idExpedienteVinculado";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var param = new DynamicParameters();
                param.Add("@idExpediente", idExpediente);
                param.Add("@idExpedienteVinculado", idExpedienteVinculado);

                var response = await connection.ExecuteAsync(eliminarVinculacionSQL, param);

                return response == 1 ? true : false;
            }
        }

        public async Task<List<ListadoExpedientesVinculadosModelo>> ObtenerExpedientesVinculadosAsync(int idExpediente)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                // tipo = 1 Relación directa
                // tipo = 2 Relación indirecta
                string sqlQuery = @"
                SELECT ev.id_expediente_vinculado AS idExpediente,
                       e.descripcion,
                       o.NOMBRE AS origen,
                       e.EMISOR,
                       1 AS tipo
                FROM exp_expediente_vinculaciones ev
                JOIN exp_expediente e ON e.id_expediente = ev.id_expediente_vinculado
                JOIN ca_origenes o ON o.id_origen = e.id_origen AND o.id_entidad = e.id_entidad
                WHERE ev.id_expediente = @idExpediente

                UNION ALL

                SELECT ev.id_expediente AS idExpediente,
                       e.descripcion,
                       o.NOMBRE AS origen,
                       e.EMISOR,
                       2 AS tipo
                FROM exp_expediente_vinculaciones ev
                JOIN exp_expediente e ON e.id_expediente = ev.id_expediente
                JOIN ca_origenes o ON o.id_origen = e.id_origen AND o.id_entidad = e.id_entidad
                WHERE ev.id_expediente_vinculado = @idExpediente
                ";

                var result = await connection.QueryAsync<ListadoExpedientesVinculadosModelo>(sqlQuery, new
                {
                    idExpediente = idExpediente
                });

                return result.ToList();
            }
        }

        public async Task<List<ListadoExpedientesVinculadosModelo>> ObtenerExpedientesDisponiblesVincularAsync(int UsuarioAsignado, int idExpediente)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"
                SELECT e.id_expediente AS idExpediente,
                       e.descripcion,
                       e.emisor,
                       o.nombre AS origen
                FROM exp_expediente e
                JOIN ca_origenes o ON o.id_origen = e.id_origen AND o.id_entidad = e.id_entidad
                WHERE e.id_usuario_asignado = @UsuarioAsignado
                  AND NOT EXISTS (
                      SELECT 1
                      FROM exp_expediente_vinculaciones ev
                      WHERE ev.id_expediente_vinculado = e.id_expediente
                        AND ev.id_expediente = @idExpediente
                  )
                ";

                var result = await connection.QueryAsync<ListadoExpedientesVinculadosModelo>(sqlQuery, new
                {
                    UsuarioAsignado = UsuarioAsignado,
                    idExpediente = idExpediente
                });

                return result.ToList();
            }
        }

        public async Task<ExpedienteModelo> ObtenerExpedienteAVincularAsync(int idExpediente)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"SELECT 
	                                    ID_EXPEDIENTE AS IdExpediente,	                                   
	                                    DESCRIPCION AS Descripcion
                                    FROM EXP_EXPEDIENTE
                                    WHERE ID_EXPEDIENTE = :IdExpediente";

                var result = await connection.QueryAsync<ExpedienteModelo>(sqlQuery, new { idExpediente });
                return result.FirstOrDefault();
            }
        }

        public async Task<bool> CopiarExpediente(CopiaExpedienteModelo copiaExpediente, DateTime Fecha)
        {
            const string copiarExpExpediente = @"INSERT INTO EXP_EXPEDIENTE 
                                                    (id_expediente, emisor, descripcion, id_origen, id_entidad, id_proceso, fecha_traslado, fecha_asignacion, id_usuario_asignado, fecha_registro, id_usuario_registro, id_fase_actual, id_tipo_operacion)
                                                    Select @idExpediente, emisor, descripcion, id_origen, id_entidad, id_proceso, fecha_traslado, fecha_asignacion, id_usuario_asignado, fecha_registro, id_usuario_registro, id_fase_actual, id_tipo_operacion 
                                                    from EXP_EXPEDIENTE where id_expediente = @idExpedienteOrigen";

            const string obtenerExpedientesDocumentos = @"Select id_Expediente IdExpediente, id_documento IdDocumento, nombre, ruta, extension, id_integracion IdIntegracion, id_usuario_carga IdUsuarioCarga, id_entidad_carga IdEntidadCarga, id_usuario_modificacion IdUsuarioModificacion, id_entidad_modificacion IdEntidadModificacion, fecha_carga FechaCarga, fecha_modificacion FechaModificacion, observaciones, id_documento_guid IdDocumentoGuid, id_almacenamiento IdAlmacenamiento
                                                                from EXP_EXPEDIENTE_DOCUMENTOS where id_expediente = @idExpedienteOrigen";
            
            const string copiarExpExpedientesDocumentos = @"Insert into EXP_EXPEDIENTE_DOCUMENTOS
                                                                (id_Expediente, id_Documento, nombre, ruta, extension, id_integracion, id_usuario_carga, id_entidad_carga, id_usuario_modificacion, id_entidad_modificacion, fecha_carga, fecha_modificacion, observaciones, id_documento_guid, id_almacenamiento)
                                                                Select @idExpediente, @idDocumento, nombre, ruta, extension, id_integracion, id_usuario_carga, id_entidad_carga, id_usuario_modificacion, id_entidad_modificacion, fecha_carga, fecha_modificacion, observaciones, id_documento_guid, id_almacenamiento
                                                                from EXP_EXPEDIENTE_DOCUMENTOS where id_expediente = @idExpedienteOrigen and id_documento = @idDocumentoOriginal";

            const string copiarHistoricoDocumento = @"INSERT INTO HISTORICO_DOCUMENTO
                                                        (
                                                            id_historico_documento, 
                                                            id_Documento_Reemplazado, 
                                                            nombre, 
                                                            ruta, 
                                                            extension, 
                                                            id_integracion, 
                                                            id_usuario_carga, 
                                                            id_entidad_carga, 
                                                            id_usuario_modificacion, 
                                                            id_entidad_modificacion, 
                                                            fecha_carga, 
                                                            fecha_modificacion, 
                                                            observaciones
                                                        )
                                                        SELECT 
                                                            (SELECT MAX(id_historico_documento) + 1 FROM historico_documento),
                                                            @idDocumentoReemplazo, 
                                                            hd.nombre, 
                                                            hd.ruta, 
                                                            hd.extension, 
                                                            hd.id_integracion, 
                                                            hd.id_usuario_carga, 
                                                            hd.id_entidad_carga, 
                                                            hd.id_usuario_modificacion, 
                                                            hd.id_entidad_modificacion, 
                                                            hd.fecha_carga, 
                                                            hd.fecha_modificacion, 
                                                            hd.observaciones 
                                                        FROM EXP_EXPEDIENTE_DOCUMENTOS ed
                                                        JOIN historico_documento hd 
                                                            ON hd.id_documento_reemplazado = ed.id_documento
                                                        WHERE ed.id_expediente = @idExpedienteOrigen
                                                          AND ed.id_documento = @idDocumento
                                                        ";

            const string copiarExpExpedienteTraslados = @"Insert into EXP_EXPEDIENTE_TRASLADOS
                                                    (id_Expediente, id_proceso, id_entidad, id_fase_origen, nombre_fase_origen, id_fase_destino, nombre_fase_destino, fecha_traslado, observacion, id_usuario_Registro)
                                                    Select @idExpediente, id_proceso, id_entidad, id_fase_origen, nombre_fase_origen, id_fase_destino, nombre_fase_destino, fecha_traslado, observacion, id_usuario_Registro
                                                    from EXP_EXPEDIENTE_TRASLADOS where id_expediente = @idExpedienteOrigen";

            const string copiarExpExpedienteAsignaciones = @"Insert into EXP_EXPEDIENTE_ASIGNACIONES
                                                    (id_Expediente, id_proceso, id_entidad, id_fase, fecha_traslado, fecha_asignacion, observacion, id_usuario_Registro, fecha_limite_atencion, id_tipo_operacion, id_usuario_asignado, fecha_operacion)
                                                    Select @idExpediente, id_proceso, id_entidad, id_fase, fecha_traslado, fecha_asignacion, observacion, id_usuario_Registro, fecha_limite_atencion, id_tipo_operacion, id_usuario_asignado, fecha_operacion
                                                    from EXP_EXPEDIENTE_ASIGNACIONES where id_expediente = @idExpedienteOrigen";

            const string copiarExpExpedienteRequisitosGestion = @"Insert into EXP_EXPEDIENTE_REQUISITOS_GESTION
                                                    (id_expediente, id_requisito, requisito, obligatorio, presentado, observacion, fecha_registro, id_entidad, id_usuario_registro)
                                                    Select @idExpediente, id_requisito, requisito, obligatorio, presentado, observacion, fecha_registro, id_entidad, id_usuario_registro from EXP_EXPEDIENTE_REQUISITOS_GESTION where id_expediente = @idExpedienteOrigen";

            const string copiarExpExpedienteTrasladoRequisitos = @"Insert into EXP_EXPEDIENTE_TRASLADO_REQUISITOS
                                                    (id_expediente, id_proceso, id_entidad, id_fase, fecha_traslado, id_requisito, id_tipo_campo, campo, obligatorio, valor, fecha_registro, id_usuario_registro)
                                                    Select @idExpediente, id_proceso, id_entidad, id_fase, fecha_traslado, id_requisito, id_tipo_campo, campo, obligatorio, valor, fecha_registro, id_usuario_registro from EXP_EXPEDIENTE_TRASLADO_REQUISITOS where id_expediente = @idExpedienteOrigen";

            const string copiarExpExpedienteVinculaciones = @"Insert into EXP_EXPEDIENTE_VINCULACIONES
                                                    (id_expediente, id_expediente_vinculado, fecha_registro, id_entidad, id_usuario)
                                                    Select @idExpediente, id_expediente_vinculado, fecha_registro, id_entidad, id_usuario
                                                    from EXP_EXPEDIENTE_VINCULACIONES where id_expediente = @idExpedienteOrigen";          

            const string copiarExpExpedienteAnotaciones = @"Insert into EXP_EXPEDIENTE_ANOTACIONES
                                                    (id_entidad, id_proceso, id_expediente, id_fase, fecha_traslado, fecha_asignacion, id_anotacion, anotacion, id_usuario, fecha_Registro, id_tipo_acceso) 
                                                    Select id_entidad, id_proceso, @idExpediente, id_fase, fecha_traslado, fecha_asignacion, id_anotacion, anotacion, id_usuario, fecha_Registro, id_tipo_acceso
                                                    from EXP_EXPEDIENTE_ANOTACIONES where id_expediente = @idExpedienteOrigen";            

            const string crearVinculacion = @"Insert into exp_expediente_vinculaciones
                                                (id_expediente, id_expediente_vinculado, fecha_registro, id_entidad, id_usuario)
                                                values
                                                (@idExpediente, @idExpedienteOrigen, @fechaRegistro, @idEntidad, @idUsuario)";

            using (var connection = await connectionProvider.OpenAsync())
            {
                for (int i = 0; i < copiaExpediente.NumeroCopias; i++)
                {
                    using (var trx = connection.BeginTransaction())
                    {                        
                        // correlativo expediente                   
                        int idExpediente = await GetIdExpediente(connection, Fecha.Year, trx);

                        // Copiar Expediente
                        var paramExpediente = new DynamicParameters();
                        paramExpediente.Add("@idExpediente", idExpediente);
                        paramExpediente.Add("@idExpedienteOrigen", copiaExpediente.IdExpediente);

                        await connection.ExecuteAsync(copiarExpExpediente, paramExpediente, trx);                        

                        // Copiar ExpExpedientesDocumentos y HistoricoDocumento
                        var listaDocumentos = await connection.QueryAsync<DocumentoModelo>(obtenerExpedientesDocumentos, paramExpediente, trx);

                        var idMaxDocumento = await connection.ExecuteScalarAsync<int>("select max(id_documento) from exp_expediente_documentos", null, trx);                        

                        foreach (var documento in listaDocumentos.ToList())
                        {
                            // Copiar ExpExpedientesDocumentos
                            idMaxDocumento = idMaxDocumento + 1;
                            var paramDocumentos = new DynamicParameters();
                            paramDocumentos.Add("@idExpediente", idExpediente);
                            paramDocumentos.Add("@idExpedienteOrigen", copiaExpediente.IdExpediente);
                            paramDocumentos.Add("@idDocumento", idMaxDocumento);
                            paramDocumentos.Add("@idDocumentoOriginal", documento.IdDocumento);                                      

                            await connection.ExecuteAsync(copiarExpExpedientesDocumentos, paramDocumentos, trx);                            

                            // Copiar HistoricoDocumento
                            var paramHistoricoDocumentos = new DynamicParameters();
                            paramHistoricoDocumentos.Add("@IdExpedienteOrigen", copiaExpediente.IdExpediente);
                            paramHistoricoDocumentos.Add("@idDocumentoReemplazo", idMaxDocumento);
                            paramHistoricoDocumentos.Add("@idDocumento", documento.IdDocumento);

                            await connection.ExecuteAsync(copiarHistoricoDocumento, paramHistoricoDocumentos, trx);
                        }

                        // Copiar ExpExpedienteTraslados
                        await connection.ExecuteAsync(copiarExpExpedienteTraslados, paramExpediente, trx);

                        // Copiar ExpExpedienteAsignaciones
                        await connection.ExecuteAsync(copiarExpExpedienteAsignaciones, paramExpediente, trx);

                        // Copiar ExpExpedienteRequisitosGestion
                        await connection.ExecuteAsync(copiarExpExpedienteRequisitosGestion, paramExpediente, trx);

                        // Copiar ExpExpedienteTrasladoRequisitos
                        await connection.ExecuteAsync(copiarExpExpedienteTrasladoRequisitos, paramExpediente, trx);                                                

                        // Copiar ExpExpedienteAnotaciones
                        await connection.ExecuteAsync(copiarExpExpedienteAnotaciones, paramExpediente, trx);

                        // Copiar ExpExpedienteVinculaciones
                        await connection.ExecuteAsync(copiarExpExpedienteVinculaciones, paramExpediente, trx);

                        // Crear Vinculación
                        var paramVinculacion = new DynamicParameters();
                        paramVinculacion.Add("@idExpediente", idExpediente);
                        paramVinculacion.Add("@idExpedienteOrigen", copiaExpediente.IdExpediente);
                        paramVinculacion.Add("@fechaRegistro", Fecha);
                        paramVinculacion.Add("@idEntidad", copiaExpediente.IdEntidad);
                        paramVinculacion.Add("@idUsuario", copiaExpediente.IdUsuarioRegistro);
                        await connection.ExecuteAsync(crearVinculacion, paramVinculacion, trx);
                        
                        if (listaDocumentos.Any())
                            await _documentosServicio.CopiarDocumentosExpediente(copiaExpediente.IdExpediente, idExpediente);
                        
                        trx.Commit();
                    }
                }

                return true;
            }

            return true;
        }

        public async Task<bool> UnificarExpedientes(ExpedienteModelo[] expedientes, DateTime Fecha)
        {
            // Identifica cual es el expediente mas antiguo
            int minAnio = 3000;
            int minCorrelativo = 1;
            int ExpedienteUnico = 0;

            foreach (var expediente in expedientes)
            {
                int anio = Convert.ToInt32(expediente.IdExpediente.ToString().Substring(expediente.IdExpediente.ToString().Length - 4, 4));
                int correlativo = Convert.ToInt32(expediente.IdExpediente.ToString().Substring(0, expediente.IdExpediente.ToString().Length - 4));

                if (anio <= minAnio)
                {
                    if (anio < minAnio)
                        minCorrelativo = 1000000;

                    minAnio = anio;

                    if (correlativo < minCorrelativo)
                        minCorrelativo = correlativo;
                }
            }

            ExpedienteUnico = Convert.ToInt32(String.Concat(minCorrelativo.ToString(), minAnio.ToString()));

            using (var connection = await connectionProvider.OpenAsync())
            {
                foreach (var expediente in expedientes)
                {
                    if (expediente.IdExpediente != ExpedienteUnico)
                    {

                        using (var trx = connection.BeginTransaction())
                        {
                            var obtenerDatosFases = @"
                            SELECT 
                                pf.nombre AS FaseOrigen, 
                                pf2.id_fase AS IdFaseDestino, 
                                pf2.nombre AS FaseDestino
                            FROM 
                                proceso_fase pf
                            JOIN 
                                proceso_fase pf2
                                ON pf2.id_entidad = pf.id_entidad
                                AND pf2.id_proceso = pf.id_proceso
                            WHERE 
                                pf.id_entidad = @idEntidad
                                AND pf.id_proceso = @idProceso
                                AND pf.id_fase = @idFaseActual
                                AND pf2.id_tipo_fase = 3";

                            var paramExpediente = new DynamicParameters();
                            paramExpediente.Add("@idEntidad", expediente.IdEntidad);
                            paramExpediente.Add("@idProceso", expediente.IdProceso);
                            paramExpediente.Add("@idFaseActual", expediente.IdFaseActual);

                            var datosFases = await connection.QueryAsync<DatosFases>(obtenerDatosFases, paramExpediente, trx);

                            foreach (var dFases in datosFases.ToList())
                            {

                                TrasladoModelo expedienteTraslado = new TrasladoModelo();

                                expedienteTraslado.IdEntidad = expediente.IdEntidad;
                                expedienteTraslado.IdProceso = expediente.IdProceso;
                                expedienteTraslado.IdExpediente = expediente.IdExpediente;
                                expedienteTraslado.IdTipoOperacion = 9;
                                expedienteTraslado.IdUsuarioRegistro = expediente.IdUsuarioRegistro;
                                expedienteTraslado.Observacion = String.Concat(minCorrelativo.ToString(), minAnio.ToString());
                                expedienteTraslado.FechaTraslado = Fecha;
                                expedienteTraslado.IdFaseOrigen = expediente.IdFaseActual;
                                expedienteTraslado.FaseOrigen = dFases.FaseOrigen;
                                expedienteTraslado.IdFaseDestino = dFases.IdFaseDestino;
                                expedienteTraslado.FaseDestino = dFases.FaseDestino;

                                await TrasladarExpediente(expedienteTraslado);
                            }
                        }
                    }
                }
            }

            return true;
        }
    }

    public class FaseIniciaModelo
    {
        public int IdFaseInicial { get; set; }
        public string FaseInicial { get; set; }
    }
}

