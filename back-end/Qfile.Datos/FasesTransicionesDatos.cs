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
    public class FasesTransicionesDatos : IFasesTransicionesDatos
    {
        private readonly IConnectionProvider connectionProvider;

        public FasesTransicionesDatos(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }

        public async Task<List<FaseModelo>> ObtenerFasesAsync(int idEntidad, int idProceso)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                const string sqlQuery = @"
                SELECT 
                    pf.id_proceso AS IdProceso, 
                    pf.id_entidad AS IdEntidad, 
                    pf.id_fase AS IdFase, 
                    pf.id_tipo_fase AS IdTipoFase, 
                    tf.nombre AS TipoFase, 
                    pf.id_unidad_administrativa AS IdUnidadAdministrativa,
                    ua.nombre AS UnidadAdministrativa, 
                    pf.nombre, 
                    pf.descripcion, 
                    pf.tiempo_promedio AS TiempoPromedio, 
                    pf.id_unidad_medida AS IdUnidadMedida,
                    um.Nombre AS UnidadMedida,
                    pf.asignacion_obligatoria AS AsignacionObligatoria, 
                    pf.activa, 
                    pf.acuse_recibo_obligatorio AS AcuseRecibido, 
                    pf.id_tipo_acceso AS IdTipoAcceso,
                    ta.nombre AS TipoAcceso
                FROM 
                    proceso_fase pf
                    INNER JOIN ca_tipo_fase tf ON pf.id_tipo_fase = tf.id_tipo_fase
                    INNER JOIN ad_unidades_administrativas ua ON pf.id_unidad_administrativa = ua.id_unidad_administrativa
                    INNER JOIN CA_UNIDAD_MEDIDA um ON pf.ID_UNIDAD_MEDIDA = um.ID_UNIDAD_MEDIDA
                    INNER JOIN CA_TIPO_ACCESO ta ON pf.ID_TIPO_ACCESO = ta.ID_TIPO_ACCESO
                WHERE 
                    pf.id_proceso = @idProceso
                    AND pf.id_entidad = @idEntidad
                ORDER BY 
                    pf.id_fase";

                var result = await connection.QueryAsync<FaseModelo>(sqlQuery, new { idEntidad, idProceso });

                var listaFases = result.ToList();

                foreach(FaseModelo fase in listaFases)
                {
                    fase.ListaTransiciones = await ObtenerTransicionesPorFaseAsync(fase.IdEntidad, fase.IdProceso, fase.IdFase);
                }

                return result.ToList();
            }
        }
        public async Task<FaseModelo> ObtenerFaseAsync(int idEntidad, int idProceso, int idFase)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                const string sqlQuery = @"
                SELECT 
                    pf.id_proceso AS IdProceso, 
                    pf.id_entidad AS IdEntidad, 
                    pf.id_fase AS IdFase, 
                    pf.id_tipo_fase AS IdTipoFase, 
                    tf.nombre AS TipoFase, 
                    pf.id_unidad_administrativa AS IdUnidadAdministrativa,
                    ua.nombre AS UnidadAdministrativa, 
                    pf.nombre, 
                    pf.descripcion, 
                    pf.tiempo_promedio AS TiempoPromedio, 
                    pf.id_unidad_medida AS IdUnidadMedida,
                    pf.asignacion_obligatoria AS AsignacionObligatoria, 
                    pf.activa, 
                    pf.acuse_recibo_obligatorio AS AcuseRecibido, 
                    pf.id_tipo_acceso AS IdTipoAcceso
                FROM 
                    proceso_fase pf
                JOIN 
                    ca_tipo_fase tf ON pf.id_tipo_fase = tf.id_tipo_fase
                JOIN 
                    ad_unidades_administrativas ua ON pf.id_unidad_administrativa = ua.id_unidad_administrativa
                WHERE 
                    pf.id_proceso = @idProceso
                    AND pf.id_entidad = @idEntidad
                    AND pf.id_fase = @idFase
                ";

                var result = await connection.QueryAsync<FaseModelo>(sqlQuery, new { idEntidad, idProceso, idFase });

                return result.FirstOrDefault();
            }
        }
        public async Task<List<TipoFaseModelo>> ObtenerTiposFasesAsync()
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                const string sqlQuery = @"select id_tipo_fase IdTipoFase, nombre from ca_tipo_fase";

                var result = await connection.QueryAsync<TipoFaseModelo>(sqlQuery);

                return result.ToList();
            }
        }
        public async Task<List<TipoAccesoModelo>> ObtenerTiposAccesosAsync()
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                const string sqlQuery = @"select id_tipo_acceso IdTipoAcceso, nombre from ca_tipo_acceso";

                var result = await connection.QueryAsync<TipoAccesoModelo>(sqlQuery);

                return result.ToList();
            }
        }
        public async Task<List<UnidadMedidaModelo>> ObtenerUnidadesMedidaAsync()
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                const string sqlQuery = @"select id_unidad_medida IdUnidadMedida, nombre from ca_unidad_medida";

                var result = await connection.QueryAsync<UnidadMedidaModelo>(sqlQuery);

                return result.ToList();
            }
        }
        public async Task<int> CrearFaseAsync(FaseModelo fase)
        {
            const string insertarFaseSQL = @"
                Insert into PROCESO_FASE
                   (ID_PROCESO, ID_ENTIDAD, ID_FASE, ID_TIPO_FASE, ID_UNIDAD_ADMINISTRATIVA, 
                    NOMBRE, DESCRIPCION, TIEMPO_PROMEDIO, ID_UNIDAD_MEDIDA, ASIGNACION_OBLIGATORIA, 
                    ACTIVA, ACUSE_RECIBO_OBLIGATORIO, ID_TIPO_ACCESO, FECHA_REGISTRO, USUARIO_REGISTRO)
                OUTPUT INSERTED.ID_FASE
                 Values
                   (@IdProceso, @IdEntidad, (select ISNULL(MAX(id_fase), 0) +1 from proceso_fase where id_entidad = @IdEntidad and id_proceso = @IdProceso), @IdTipoFase, @IdUnidadAdministrativa, 
                    @Nombre, @Descripcion, @TiempoPromedio, @IdUnidadMedida, @AsignacionObligatoria, @Activa, @AcuseRecibido, @IdTipoAcceso, @FechaRegistro, @UsuarioRegistro)
                ";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var param = new DynamicParameters();
                param.Add("@IdEntidad", fase.IdEntidad);
                param.Add("@IdProceso", fase.IdProceso);
                param.Add("@IdFase", fase.IdFase);
                param.Add("@IdTipoFase", fase.IdTipoFase);
                param.Add("@IdUnidadAdministrativa", fase.IdUnidadAdministrativa);
                param.Add("@Nombre", fase.Nombre);
                param.Add("@Descripcion", fase.Descripcion);
                param.Add("@TiempoPromedio", fase.TiempoPromedio);
                param.Add("@IdUnidadMedida", fase.IdUnidadMedida);
                param.Add("@AsignacionObligatoria", fase.AsignacionObligatoria ? 1 : 0);
                param.Add("@Activa", 0);
                param.Add("@AcuseRecibido", fase.AcuseRecibido ? 1 : 0);
                param.Add("@IdTipoAcceso", fase.IdTipoAcceso);
                param.Add("@FechaRegistro", fase.FechaRegistro);
                param.Add("@UsuarioRegistro", fase.UsuarioRegistro);

                await connection.ExecuteAsync(insertarFaseSQL, param);

                fase.IdFase = param.Get<int>("IdFase");
            }

            return fase.IdFase;

        }
        public async Task<int> EliminarFaseAsync(int idEntidad, int idProceso, int IdFase)
        {
            const string eliminarFaseSQL = @"                
                DELETE PROCESO_FASE 
                WHERE ID_ENTIDAD = @IdEntidad 
                AND ID_PROCESO = @IdProceso 
                AND ID_FASE = @IdFase";

            using (var connection = await connectionProvider.OpenAsync())
            {

                var response = await connection.ExecuteAsync(eliminarFaseSQL, new { idEntidad, idProceso, IdFase });

                return response;
            }

        }
        public async Task<int> ActualizarFaseAsync(FaseModelo fase)
        {
            int response;

            const string actualizarFaseSQL = @"                
                UPDATE PROCESO_FASE SET
                    NOMBRE = @Nombre,
                    DESCRIPCION = @Descripcion,
                    ID_TIPO_FASE = @IdTipoFase,
                    ID_TIPO_ACCESO = @IdTipoAcceso,
                    ID_UNIDAD_ADMINISTRATIVA = @IdUnidadAdministrativa,
                    TIEMPO_PROMEDIO = @TiempoPromedio,
                    ID_UNIDAD_MEDIDA = @IdUnidadMedida,
                    ASIGNACION_OBLIGATORIA = @AsignacionObligatoria,
                    ACUSE_RECIBO_OBLIGATORIO = @AcuseRecibido,
                    ACTIVA = @Activa
                WHERE
                    ID_ENTIDAD = @IdEntidad
                    AND ID_PROCESO = @IdProceso
                    AND ID_FASE = @IdFase";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var param = new DynamicParameters();
                param.Add("@IdEntidad", fase.IdEntidad);
                param.Add("@IdProceso", fase.IdProceso);
                param.Add("@IdFase", fase.IdFase);
                param.Add("@IdTipoFase", fase.IdTipoFase);
                param.Add("@IdUnidadAdministrativa", fase.IdUnidadAdministrativa);
                param.Add("@Nombre", fase.Nombre);
                param.Add("@Descripcion", fase.Descripcion);
                param.Add("@TiempoPromedio", fase.TiempoPromedio);
                param.Add("@IdUnidadMedida", fase.IdUnidadMedida);
                param.Add("@AsignacionObligatoria", fase.AsignacionObligatoria ? 1 : 0);
                param.Add("@Activa", fase.Activa ? 1: 0);
                param.Add("@AcuseRecibido", fase.AcuseRecibido ? 1 : 0);
                param.Add("@IdTipoAcceso", fase.IdTipoAcceso);

                response = await connection.ExecuteAsync(actualizarFaseSQL, param);

                return response;
            }

        }
        public async Task<bool> ExisteFaseInicial(int idEntidad, int idProceso)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                const string sqlQuery = @"select count(1) 
                                            from proceso_fase
                                            where id_proceso = @idProceso
                                            and id_entidad = @idEntidad
                                            and id_tipo_fase = 1";

                var result = await connection.ExecuteScalarAsync<int>(sqlQuery, new { idEntidad, idProceso });

                return result == 0 ? false : true;
            }
        }
        public async Task<bool> ExistenExpedientesAsigandosALaFase(int idEntidad, int idProceso, int idFase) {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                const string sqlQuery = @"select count(1) 
                                            from exp_expediente
                                            where id_proceso = @idProceso
                                            and id_entidad = @idEntidad
                                            and id_fase_actual = @idFase";

                var result = await connection.ExecuteScalarAsync<int>(sqlQuery, new { idEntidad, idProceso, idFase });

                return result == 0 ? false : true;
            }
        }

        #region Fase Usuarios
        public async Task<List<UsuarioFaseModelo>> ObtenerUsuariosPorFaseAsync(int idEntidad, int idProceso, int idFase)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                    string sqlQuery = @"
                    SELECT 
                        fu.id_entidad AS IdEntidad, 
                        fu.id_proceso AS IdProceso, 
                        fu.id_fase AS IdFase,
                        fu.id_usuario AS idusuario, 
                        fu.recepcion_traslado AS recepciontraslado,
                        (COALESCE(us.primer_nombre, '') 
                            + COALESCE(' ' + LTRIM(RTRIM(us.segundo_nombre)), '') 
                            + COALESCE(' ' + LTRIM(RTRIM(us.otros_nombres)), '') 
                            + COALESCE(' ' + LTRIM(RTRIM(us.primer_apellido)), '') 
                            + COALESCE(' ' + LTRIM(RTRIM(us.segundo_apellido)), '') 
                            + COALESCE(' de ' + LTRIM(RTRIM(us.apellido_casada)), '')) AS Nombre
                    FROM 
                        proceso_fase_usuarios fu
                    JOIN 
                        ad_usuarios us ON us.id_usuario = fu.id_usuario
                    WHERE 
                        fu.id_entidad = @IdEntidad
                        AND fu.id_proceso = @IdProceso
                        AND fu.id_fase = @IdFase;

                    ";

                var result = await connection.QueryAsync<UsuarioFaseModelo>(sqlQuery, new { IdEntidad = idEntidad, IdProceso = idProceso, IdFase = idFase });

                return result.ToList();
            }
        }       
        public async Task<int> CrearUsuarioFaseAsync(UsuarioFaseModelo usuario)
        {
            const string insertarSQL = @"
                Insert into PROCESO_FASE_USUARIOS
                    (ID_ENTIDAD, ID_PROCESO, ID_FASE, ID_USUARIO, RECEPCION_TRASLADO, FECHA_REGISTRO, USUARIO_REGISTRO)
                OUTPUT INSERTED.ID_USUARIO
                Values
                    (@IdEntidad, @IdProceso, @IdFase, @IdUsuario, @RecepcionTraslado, @FechaRegistro, @UsuarioRegistro)
                ";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var param = new DynamicParameters();
                param.Add("@IdEntidad", usuario.IdEntidad);
                param.Add("@IdProceso", usuario.IdProceso);
                param.Add("@IdFase", usuario.IdFase);
                param.Add("@IdUsuario", usuario.IdUsuario);
                param.Add("@RecepcionTraslado", usuario.RecepcionTraslado ? 1 : 0);
                param.Add("@FechaRegistro", usuario.FechaRegistro);
                param.Add("@UsuarioRegistro", usuario.UsuarioRegistro);

                await connection.ExecuteAsync(insertarSQL, param);

                usuario.IdUsuario = param.Get<int>("IdUsuario");
            }

            return usuario.IdUsuario;

        }
        public async Task<int> EliminarUsuarioFaseAsync(int idEntidad, int idProceso, int IdFase, int idUsuario)
        {
            const string eliminarUsuarioFaseSQL = @"                
                DELETE proceso_fase_usuarios
                WHERE id_entidad = @IdEntidad
                AND id_proceso = @IdProceso
                AND id_fase = @IdFase
                AND id_usuario = @IdUsuario";

            const string eliminarUsuarioTransicionesSQL = @"                
                DELETE proceso_transicion_usuarios
                WHERE id_entidad = @IdEntidad
                AND id_proceso = @IdProceso
                AND id_fase_origen = @IdFase                
                AND id_usuario = @IdUsuario";

            using (var connection = await connectionProvider.OpenAsync())
            {
                using (var trx = connection.BeginTransaction())
                {
                    await connection.ExecuteAsync(eliminarUsuarioFaseSQL, new { idEntidad, idProceso, IdFase, idUsuario }, trx);

                    await connection.ExecuteAsync(eliminarUsuarioTransicionesSQL, new { idEntidad, idProceso, IdFase, idUsuario }, trx);

                    trx.Commit();
                }
                return 1;
            }
        }
        public async Task<List<UsuarioListaModelo>> ObtenerUsuariosPorUAAsync(int idEntidad, int idProceso, int idFase, int idUnidadAdministrativa)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"
                SELECT 
                    us.id_usuario AS IdUsuario,
                    (COALESCE(us.primer_nombre, '') 
                     + COALESCE(' ' + LTRIM(RTRIM(us.segundo_nombre)), '') 
                     + COALESCE(' ' + LTRIM(RTRIM(us.otros_nombres)), '') 
                     + COALESCE(' ' + LTRIM(RTRIM(us.primer_apellido)), '') 
                     + COALESCE(' ' + LTRIM(RTRIM(us.segundo_apellido)), '') 
                     + COALESCE(' de ' + LTRIM(RTRIM(us.apellido_casada)), '')) AS Nombre
                FROM 
                    ad_usuarios us
                WHERE 
                    us.id_Entidad = @IdEntidad
                    AND us.id_unidad_administrativa = @IdUnidadAdministrativa
                    AND NOT EXISTS (
                        SELECT 1 
                        FROM proceso_fase_usuarios fu
                        WHERE fu.id_entidad = @IdEntidad 
                          AND fu.id_proceso = @IdProceso 
                          AND fu.id_fase = @IdFase 
                          AND fu.id_usuario = us.id_usuario
                    )";

                var result = await connection.QueryAsync<UsuarioListaModelo>(sqlQuery, new { idEntidad, idProceso, idFase, idUnidadAdministrativa });

                return result.ToList();
            }
        }
        public async Task<int> PermisoRecepcionAsync(UsuarioFaseModelo usuario)
        {
            int response;

            const string actualizarSQL = @"                
                UPDATE PROCESO_FASE_USUARIOS
                    SET RECEPCION_TRASLADO = @RecepcionTraslado
                WHERE
                    ID_ENTIDAD = @IdEntidad
                    AND ID_PROCESO = @IdProceso
                    AND ID_FASE = @IdFase
                    AND ID_USUARIO = @IdUsuario";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var param = new DynamicParameters();
                param.Add("@IdEntidad", usuario.IdEntidad);
                param.Add("@IdProceso", usuario.IdProceso);
                param.Add("@IdFase", usuario.IdFase);
                param.Add("@IdUsuario", usuario.IdUsuario);
                param.Add("@RecepcionTraslado", usuario.RecepcionTraslado? 1 : 0);

                response = await connection.ExecuteAsync(actualizarSQL, param);

                return response;
            }

        }
        #endregion

        #region Transiciones
        public async Task<List<TransicionesModelo>> ObtenerTransicionesPorFaseAsync(int idEntidad, int idProceso, int idFaseOrigen)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"SELECT tr.id_entidad IdEntidad, tr.id_proceso IdProceso, tr.id_fase_origen IdFaseOrigen, fo.nombre FaseOrigen, uafo.nombre UnidadAdministrativaFO,
                                       tr.id_fase_destino IdFaseDestino,fd.NOMBRE FaseDestino, uafd.nombre UnidadAdministrativaFD, 
                                       tr.usuario_registro UsuarioRegistro, tr.activa, tr.fecha_registro FechaRegistro
                                  FROM proceso_transiciones tr, proceso_fase fd, proceso_fase fo, ad_unidades_administrativas uafd, ad_unidades_administrativas uafo
                                 WHERE uafo.id_unidad_administrativa = fo.id_unidad_administrativa
                                   AND fo.id_fase = tr.id_fase_origen 
                                   AND fo.id_proceso = tr.id_proceso
                                   AND fd.id_fase = tr.id_fase_destino
                                   AND fd.id_proceso = tr.id_proceso
                                   AND uafd.id_unidad_administrativa = fd.id_unidad_administrativa
                                   AND tr.id_entidad = @IdEntidad
                                   AND tr.id_proceso = @IdProceso
                                   AND tr.id_fase_origen = @IdFaseOrigen";

                var result = await connection.QueryAsync<TransicionesModelo>(sqlQuery, new { IdEntidad = idEntidad, IdProceso = idProceso, IdFaseOrigen = idFaseOrigen });

                return result.ToList();
            }
        }
        public async Task<int> CrearTransicioneAsync(TransicionesModelo transicion)
        {
            const string insertarSQL = @"
                Insert into PROCESO_TRANSICIONES
                   (ID_ENTIDAD, ID_PROCESO, ID_FASE_ORIGEN, ID_FASE_DESTINO, ACTIVA, FECHA_REGISTRO, USUARIO_REGISTRO)
                 Values
                   (@IdEntidad, @IdProceso, @IdFaseOrigen, @IdFaseDestino, @Activa, @FechaRegistro, @UsuarioRegistro)";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var param = new DynamicParameters();
                param.Add("@IdEntidad", transicion.IdEntidad);
                param.Add("@IdProceso", transicion.IdProceso);
                param.Add("@IdFaseOrigen", transicion.IdFaseOrigen);
                param.Add("@IdFaseDestino", transicion.IdFaseDestino);
                param.Add("@Activa", transicion.Activa ? 1 : 0);
                param.Add("@FechaRegistro", transicion.FechaRegistro);
                param.Add("@UsuarioRegistro", transicion.UsuarioRegistro);

                await connection.ExecuteAsync(insertarSQL, param);
            }

            return transicion.IdFaseDestino;

        }
        public async Task<int> EliminarTransicionAsync(int idEntidad, int idProceso, int IdFaseOrigen, int idFaseDestino)
        {
            const string eliminarFaseSQL = @"                
                DELETE  proceso_transiciones
                WHERE id_entidad = @IdEntidad
                AND id_proceso = @IdProceso
                AND id_fase_origen = @IdFaseOrigen
                AND id_fase_destino = @IdFaseDestino";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var response = await connection.ExecuteAsync(eliminarFaseSQL, new { idEntidad, idProceso, IdFaseOrigen, idFaseDestino });

                return response;
            }

        }
        public async Task<List<FaseModelo>> ObtenerFasesPendientesAsync(int idEntidad, int idProceso, int IdFaseOrigen)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"SELECT fa.id_entidad IdEntidad,fa.id_proceso IdProceso, fa.id_fase IdFase, fa.nombre
                                    FROM proceso_fase fa
                                    WHERE fa.id_Entidad = @IdEntidad
                                    AND fa.id_proceso = @IdProceso
                                    AND fa.id_fase <> @idFaseOrigen
                                    AND not exists (SELECT 1 
                                                FROM proceso_transiciones tr
                                                WHERE tr.id_entidad = @IdEntidad 
                                                AND tr.id_proceso = @IdProceso 
                                                AND tr.id_fase_origen = @IdFaseOrigen
                                                AND tr.id_fase_destino = fa.id_Fase)";

                var result = await connection.QueryAsync<FaseModelo>(sqlQuery, new { idEntidad, idProceso, IdFaseOrigen });

                return result.ToList();
            }
        }
        public async Task<int> ActivarTransicionAsync(TransicionesModelo transicion)
        {
            int response;

            const string actualizarSQL = @"                
                UPDATE PROCESO_TRANSICIONES
                    SET ACTIVA = @Activa
                WHERE
                    ID_ENTIDAD = @IdEntidad
                    AND ID_PROCESO = @IdProceso
                    AND ID_FASE_ORIGEN = @IdFaseOrigen
                    AND ID_FASE_DESTINO = @IdFaseDestino";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var param = new DynamicParameters();
                param.Add("@IdEntidad", transicion.IdEntidad);
                param.Add("@IdProceso", transicion.IdProceso);
                param.Add("@IdFaseOrigen", transicion.IdFaseOrigen);
                param.Add("@IdFaseDestino", transicion.IdFaseDestino);
                param.Add("@Activa", transicion.Activa ? 1 : 0);

                response = await connection.ExecuteAsync(actualizarSQL, param);

                return response;
            }

        }
        #endregion

        #region Transición Usuarios
        public async Task<List<TransicionUsuarioModelo>> ObtenerUsuariosPorTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"
                SELECT 
                    tu.id_entidad AS IdEntidad, 
                    tu.id_proceso AS IdProceso, 
                    tu.id_fase_origen AS IdFaseOrigen, 
                    tu.id_fase_destino AS IdFaseDestino, 
                    tu.id_usuario AS IdUsuario,
                    (COALESCE(us.primer_nombre, '') 
                     + COALESCE(' ' + LTRIM(RTRIM(us.segundo_nombre)), '') 
                     + COALESCE(' ' + LTRIM(RTRIM(us.otros_nombres)), '') 
                     + COALESCE(' ' + LTRIM(RTRIM(us.primer_apellido)), '') 
                     + COALESCE(' ' + LTRIM(RTRIM(us.segundo_apellido)), '') 
                     + COALESCE(' de ' + LTRIM(RTRIM(us.apellido_casada)), '')) AS Usuario
                FROM 
                    proceso_transicion_usuarios tu
                JOIN 
                    ad_usuarios us ON us.id_usuario = tu.id_usuario
                WHERE 
                    tu.id_entidad = @IdEntidad
                    AND tu.id_proceso = @IdProceso
                    AND tu.id_fase_origen = @IdFaseOrigen
                    AND tu.id_fase_destino = @IdFaseDestino;
                ";

                var result = await connection.QueryAsync<TransicionUsuarioModelo>(sqlQuery, new { IdEntidad = idEntidad, IdProceso = idProceso, IdFaseOrigen = idFaseOrigen, idFaseDestino = idFaseDestino });

                return result.ToList();
            }
        }
        public async Task<int> CrearUsuarioTransicionAsync(TransicionUsuarioModelo usuario)
        {
            const string insertarSQL = @"
                Insert into PROCESO_TRANSICION_USUARIOS
                    (ID_ENTIDAD, ID_PROCESO, ID_FASE_ORIGEN, ID_FASE_DESTINO, ID_USUARIO, FECHA_REGISTRO, USUARIO_REGISTRO)
                OUTPUT INSERTED.ID_USUARIO
                Values
                    (@IdEntidad, @IdProceso, @IdFaseOrigen, @IdFaseDestino, @IdUsuario, @FechaRegistro, @UsuarioRegistro)
                ";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var param = new DynamicParameters();
                param.Add("@IdUsuario", usuario.IdUsuario);
                param.Add("@IdEntidad", usuario.IdEntidad);
                param.Add("@IdProceso", usuario.IdProceso);
                param.Add("@IdFaseOrigen", usuario.IdFaseOrigen);
                param.Add("@IdFaseDestino", usuario.IdFaseDestino);
                param.Add("@IdUsuario", usuario.IdUsuario);
                param.Add("@FechaRegistro", usuario.FechaRegistro);
                param.Add("@UsuarioRegistro", usuario.UsuarioRegistro);

                await connection.ExecuteAsync(insertarSQL, param);

                usuario.IdUsuario = param.Get<int>("IdUsuario");
            }

            return usuario.IdUsuario;

        }
        public async Task<int> EliminarUsuarioTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino, int idUsuario)
        {
            const string eliminarFaseSQL = @"                
                DELETE      proceso_transicion_usuarios
                WHERE id_entidad = @IdEntidad
                AND id_proceso = @IdProceso
                AND id_fase_origen = @IdFaseOrigen
                AND id_fase_destino = @IdFaseDestino
                AND id_usuario = @IdUsuario";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var response = await connection.ExecuteAsync(eliminarFaseSQL, new { idEntidad, idProceso, idFaseOrigen, idFaseDestino, idUsuario });

                return response;
            }

        }
        public async Task<List<UsuarioFaseModelo>> ObtenerUsuariosTransicionPendientesAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"
                SELECT 
                    fu.id_entidad AS IdEntidad, 
                    fu.id_proceso AS IdProceso, 
                    fu.id_fase AS IdFase,
                    fu.id_usuario AS idusuario, 
                    fu.recepcion_traslado AS recepciontraslado,
                    (COALESCE(us.primer_nombre, '') 
                     + COALESCE(' ' + LTRIM(RTRIM(us.segundo_nombre)), '') 
                     + COALESCE(' ' + LTRIM(RTRIM(us.otros_nombres)), '') 
                     + COALESCE(' ' + LTRIM(RTRIM(us.primer_apellido)), '') 
                     + COALESCE(' ' + LTRIM(RTRIM(us.segundo_apellido)), '') 
                     + COALESCE(' de ' + LTRIM(RTRIM(us.apellido_casada)), '')) AS Nombre
                FROM 
                    proceso_fase_usuarios fu
                JOIN 
                    ad_usuarios us ON us.id_usuario = fu.id_usuario
                WHERE 
                    fu.id_entidad = @IdEntidad
                    AND fu.id_proceso = @IdProceso
                    AND fu.id_fase = @IdFaseOrigen
                    AND NOT EXISTS (
                        SELECT 1 
                        FROM proceso_transicion_usuarios tu
                        WHERE tu.id_entidad = @IdEntidad 
                          AND tu.id_proceso = @IdProceso
                          AND tu.id_fase_origen = @IdFaseOrigen
                          AND tu.id_fase_destino = @IdFaseDestino
                          AND tu.id_usuario = us.id_usuario
                    );";

                var result = await connection.QueryAsync<UsuarioFaseModelo>(sqlQuery, new { idEntidad, idProceso, idFaseOrigen, idFaseDestino });

                return result.ToList();
            }
        }

        #endregion

        #region Transición Notificaciones

        public async Task<int> CrearNotificacionTransicionAsync(TransicionNotificacionModelo notificacion)
        {
            const string insertarSQL = @"
                Insert into PROCESO_TRANSICION_NOTIFICACIONES
                    (ID_ENTIDAD, ID_PROCESO, ID_FASE_ORIGEN, ID_FASE_DESTINO, CORREO, FECHA_REGISTRO, USUARIO_REGISTRO)
                Values
                    (@IdEntidad, @IdProceso, @IdFaseOrigen, @IdFaseDestino, @Correo, @FechaRegistro, @UsuarioRegistro)";

            int result = 0;
             using (var connection = await connectionProvider.OpenAsync())
            {
                var param = new DynamicParameters();
                param.Add("@IdEntidad", notificacion.IdEntidad);
                param.Add("@IdProceso", notificacion.IdProceso);
                param.Add("@IdFaseOrigen", notificacion.IdFaseOrigen);
                param.Add("@IdFaseDestino", notificacion.IdFaseDestino);
                param.Add("@Correo", notificacion.Correo);
                param.Add("@FechaRegistro", notificacion.FechaRegistro);
                param.Add("@UsuarioRegistro", notificacion.UsuarioRegistro);

                await connection.ExecuteAsync(insertarSQL, param);
                result = 1;
            }

            return result;

        }
        public async Task<int> EliminarNotificacionTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino, string correo)
        {
            const string eliminarFaseSQL = @"                
                DELETE      proceso_transicion_notificaciones
                WHERE id_entidad = @IdEntidad
                AND id_proceso = @IdProceso
                AND id_fase_origen = @IdFaseOrigen
                AND id_fase_destino = @IdFaseDestino
                AND correo = @Correo";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var response = await connection.ExecuteAsync(eliminarFaseSQL, new { idEntidad, idProceso, idFaseOrigen, idFaseDestino, correo });

                return response;
            }

        }
        public async Task<List<TransicionNotificacionModelo>> ObtenerNotificacionesPorTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"select tn.id_entidad IdEntidad, tn.id_proceso IdProceso, tn.id_fase_origen IdFaseOrigen, fo.nombre FaseOrigen,
                                    tn.id_fase_destino IdFaseDestino, fd.nombre FaseDestino, tn.correo
                                    from proceso_transicion_notificaciones tn, proceso_fase fo, proceso_fase fd
                                    where tn.id_entidad = fo.id_entidad
                                    and tn.id_proceso = fo.id_proceso
                                    and tn.id_fase_origen = fo.id_fase
                                    and tn.id_entidad = fd.id_entidad
                                    and tn.id_proceso = fd.id_proceso
                                    and tn.id_fase_destino = fd.id_fase
                                    and tn.id_entidad = @IdEntidad
                                    and tn.id_proceso = @IdProceso
                                    and tn.id_fase_origen = @IdFaseOrigen
                                    and tn.id_fase_destino = @IdFaseDestino";

                var result = await connection.QueryAsync<TransicionNotificacionModelo>(sqlQuery, new { IdEntidad = idEntidad, IdProceso = idProceso, IdFaseOrigen = idFaseOrigen, idFaseDestino = idFaseDestino });

                return result.ToList();
            }
        }

        #endregion

        #region Requisito Por Transición

        public async Task<List<RequisitoPorTransicionModelo>> ObtenerRequisitosPorTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"SELECT
                                        prpt.ID_ENTIDAD IdEntidad, 
                                        prpt.ID_PROCESO IdProceso, 
                                        prpt.ID_FASE_ORIGEN IdFaseOrigen, 
                                        prpt.ID_FASE_DESTINO IdFaseDestino, 
                                        prpt.ID_REQUISITO IdRequisito, 
                                        prpt.ID_TIPO_CAMPO IdTipoCampo, 
                                        ctc.NOMBRE NombreTipoCampo, 
                                        prpt.CAMPO Campo, 
                                        prpt.OBLIGATORIO Obligatorio, 
                                        FECHA_REGISTRO FechaRegistro, 
                                        USUARIO_REGISTRO UsuarioRegistro
                                    FROM PROCESO_REQUISITOS_POR_TRANSICION prpt 
                                    JOIN CA_TIPO_CAMPO ctc ON prpt.ID_TIPO_CAMPO = ctc.ID_TIPO_CAMPO 
                                    WHERE 
                                        ID_ENTIDAD = @idEntidad
                                        AND ID_PROCESO = @idProceso
                                        AND ID_FASE_ORIGEN = @idFaseOrigen
                                        AND ID_FASE_DESTINO = @idFaseDestino";

                var result = await connection.QueryAsync<RequisitoPorTransicionModelo>(sqlQuery, new { 
                    idEntidad, idProceso, idFaseOrigen, idFaseDestino
                });

                return result.ToList();
            }
        }


        public async Task<int> CrearRequisitoPorTransicionAsync(RequisitoPorTransicionModelo requisito)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            const string insertarSQL = @"
                INSERT INTO PROCESO_REQUISITOS_POR_TRANSICION
                    (ID_ENTIDAD, ID_PROCESO, ID_FASE_ORIGEN, ID_FASE_DESTINO, ID_REQUISITO, ID_TIPO_CAMPO, CAMPO, OBLIGATORIO, FECHA_REGISTRO, USUARIO_REGISTRO)
                    OUTPUT INSERTED.ID_REQUISITO                
                    VALUES
                    (@IdEntidad, @IdProceso, @IdFaseOrigen, @IdFaseDestino, (SELECT ISNULL(MAX(ID_REQUISITO), 0) + 1 FROM PROCESO_REQUISITOS_POR_TRANSICION), @IdTipoCampo, @Campo, @Obligatorio, @FechaRegistro, @UsuarioRegistro)
                ";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var param = new DynamicParameters();
                param.Add("@IdEntidad", requisito.IdEntidad);
                param.Add("@IdProceso", requisito.IdProceso);
                param.Add("@IdFaseOrigen", requisito.IdFaseOrigen);
                param.Add("@IdFaseDestino", requisito.IdFaseDestino);
                param.Add("@IdRequisito", requisito.IdRequisito);
                param.Add("@IdTipoCampo", requisito.IdTipoCampo);
                param.Add("@Campo", requisito.Campo);
                param.Add("@Obligatorio", requisito.Obligatorio);
                param.Add("@FechaRegistro", requisito.FechaRegistro);
                param.Add("@UsuarioRegistro", requisito.UsuarioRegistro);

                await connection.ExecuteAsync(insertarSQL, param);

                requisito.IdRequisito = param.Get<int>("IdRequisito");
            }

            return requisito.IdRequisito;
        }

        public async Task<int> EliminarRequisitoPorTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino, int idRequisito)
        {
            const string eliminarSQL = @"                
                DELETE PROCESO_REQUISITOS_POR_TRANSICION
                WHERE id_entidad = @idEntidad
                AND id_proceso = @idProceso
                AND id_fase_origen = @idFaseOrigen
                AND id_fase_destino = @idFaseDestino
                AND id_requisito = @idRequisito";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var response = await connection.ExecuteAsync(eliminarSQL, new { idEntidad, idProceso, idFaseOrigen, idFaseDestino, idRequisito });

                return response;
            }
        }

        public async Task<int> ActualizarRequisitoPorTransicionAsync(RequisitoPorTransicionModelo requisito)
        {
            int response = 0;

            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            const string actualizarSQL = @"                
                    UPDATE PROCESO_REQUISITOS_POR_TRANSICION
                    SET 
                        ID_TIPO_CAMPO = :IdTipoCampo, 
                        CAMPO = :Campo, 
                        OBLIGATORIO = :Obligatorio, 
                        FECHA_REGISTRO = :FechaRegistro, 
                        USUARIO_REGISTRO = :UsuarioRegistro
                    WHERE 
                        ID_ENTIDAD = :IdEntidad 
                        AND ID_PROCESO = :IdProceso 
                        AND ID_FASE_ORIGEN = :FaseOrigen 
                        AND ID_FASE_DESTINO = :IdFaseDestino 
                        AND ID_REQUISITO = :idRequisito";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var param = new DynamicParameters();
                param.Add("@IdEntidad", requisito.IdEntidad);
                param.Add("@IdProceso", requisito.IdProceso);
                param.Add("@IdFaseOrigen", requisito.IdFaseOrigen);
                param.Add("@IdFaseDestino", requisito.IdFaseDestino);
                param.Add("@IdRequisito", requisito.IdRequisito);
                param.Add("@IsTipoCampo", requisito.IdTipoCampo);
                param.Add("@Campo", requisito.Campo);
                param.Add("@Obligatorio", requisito.Obligatorio);
                param.Add("@FechaRegistro", requisito.FechaRegistro);
                param.Add("@UsuarioRegistro", requisito.UsuarioRegistro);

                response = await connection.ExecuteAsync(actualizarSQL, param);
            }

            return response;
        }
        
        #endregion
    }
}