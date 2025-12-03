using Dapper;
using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Datos
{
    public class InhabilitacionDatos : IInhabilitacionDatos
    {
        private readonly IConnectionProvider _connectionProvider;        

        public InhabilitacionDatos(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<int> GuardarInhabilitacionAsync(HistoricoInhabilitacionModelo inhabilitacion)
        {
            const string instartarInhabilitacion = @"                
            INSERT INTO HISTORICO_INHABILITACIONES
                (ID_ENTIDAD, ID_USUARIO, ID_HISTORICO_INHABILITACION, FECHA_INICIO, FECHA_FIN, FECHA_REGISTRO, USUARIO_REGISTRO)
            VALUES
                (@IdEntidad, @IdUsuario, (SELECT ISNULL(MAX(ID_HISTORICO_INHABILITACION), 0) + 1 FROM HISTORICO_INHABILITACIONES WHERE ID_ENTIDAD = @IdEntidad AND ID_USUARIO = @IdUsuario), @FechaInicio, @FechaFin, @FechaRegistro, @UsuarioRegistro)";

            using (var connection = await _connectionProvider.OpenAsync())
            {

                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@IdEntidad", inhabilitacion.IdEntidad);
                    param.Add("@IdUsuario", inhabilitacion.IdUsuario);
                    param.Add("@IdHistoricoInhabilitacion", inhabilitacion.IdHistoricoInhabilitacion);
                    param.Add("@FechaInicio", inhabilitacion.FechaInicio);
                    param.Add("@FechaFin", inhabilitacion.FechaFin);
                    param.Add("@FechaRegistro", inhabilitacion.FechaRegistro);
                    param.Add("@UsuarioRegistro", inhabilitacion.UsuarioRegistro);
                    await connection.ExecuteAsync(instartarInhabilitacion, param, trx);

                    inhabilitacion.IdHistoricoInhabilitacion = param.Get<int>("IdHistoricoInhabilitacion");

                    trx.Commit();
                }
            }

            return inhabilitacion.IdHistoricoInhabilitacion;
        }

        public async Task<int> ActualizarInhabilitacionAsync(HistoricoInhabilitacionModelo inhabilitacion)
        {
            const string actualizarSQL = @"                
                UPDATE HISTORICO_INHABILITACIONES SET 
                    FECHA_INICIO = @FechaInicio,
                    FECHA_FIN = @FechaFin
                WHERE
                    ID_ENTIDAD = @IdEntidad AND
                    ID_USUARIO = @IdUsuario AND
                    ID_HISTORICO_INHABILITACION = @IdHistoricoInhabilitacion";

            using (var connection = await _connectionProvider.OpenAsync())
            {
                var param = new DynamicParameters();
                param.Add("@IdEntidad", inhabilitacion.IdEntidad);
                param.Add("@IdUsuario", inhabilitacion.IdUsuario);
                param.Add("@IdHistoricoInhabilitacion", inhabilitacion.IdHistoricoInhabilitacion);
                param.Add("@FechaInicio", inhabilitacion.FechaInicio);
                param.Add("@FechaFin", inhabilitacion.FechaFin);
                param.Add("@FechaRegistro", inhabilitacion.FechaRegistro);
                param.Add("@UsuarioRegistro", inhabilitacion.UsuarioRegistro);
                await connection.ExecuteAsync(actualizarSQL, param);

                var response = await connection.ExecuteAsync(actualizarSQL, param);

                return response;
            }
        }

        public async Task<FechasInhabilitacionUsuarioModelo> ObtenerFechasInhabilitacionUsuarioAsync(int IdUsuario)
        {
            const string ObtenerFechasInhabilitacionUsuarioSQL = @"                
            SELECT TOP 1 ID_USUARIO AS IdUsuario, FECHA_INICIO AS FechaInicio, FECHA_FIN AS FechaFin 
            FROM (
                SELECT ID_USUARIO, FECHA_INICIO, FECHA_FIN, ID_HISTORICO_INHABILITACION 
                FROM HISTORICO_INHABILITACIONES
                WHERE ID_USUARIO = @IdUsuario
            ) q1
            ORDER BY ID_HISTORICO_INHABILITACION DESC";

            using (var connection = await _connectionProvider.OpenAsync())
            {
                return await connection.QueryFirstOrDefaultAsync<FechasInhabilitacionUsuarioModelo>(ObtenerFechasInhabilitacionUsuarioSQL, new { IdUsuario });
                
            }            
        }

        public async Task<List<HistoricoInhabilitacionModelo>> ObtenerInhabilitacionesUsuarioAsync(int idEntidad, int IdUsuario)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await _connectionProvider.OpenAsync())
            {
                string sqlQuery = @"select id_entidad IdEntidad, id_usuario IdUsuario, id_historico_inhabilitacion IdHistoricoInhabilitacion, FECHA_INICIO FechaInicio, FECHA_FIN FechaFin, fecha_registro FechaRegistro, id_Usuario UsuarioRegistro
                                    from HISTORICO_INHABILITACIONES
                                    where id_entidad = @IdEntidad
                                    and id_usuario = @IdUsuario
                                    order by FECHA_REGISTRO";

                var result = await connection.QueryAsync<HistoricoInhabilitacionModelo>(sqlQuery, new { idEntidad, IdUsuario });
                return result.ToList();
            }
        }

        public async Task<bool> EliminarInhabilitacionAsync(int idEntidad, int idUsuario, int idInhabilitacion)
        {
            using (var connection = await _connectionProvider.OpenAsync())
            {
                int registrosAfectados = 0;

                string instruccionSQL = @"DELETE HISTORICO_INHABILITACIONES WHERE ID_ENTIDAD = @idEntidad AND ID_USUARIO = @idUsuario AND ID_HISTORICO_INHABILITACION = @idHistoricoInhabilitacion";

                using (var trx = connection.BeginTransaction())
                {
                    registrosAfectados += await connection.ExecuteAsync(instruccionSQL, new { idEntidad, idUsuario, idHistoricoInhabilitacion=idInhabilitacion }, trx);
                    trx.Commit();
                }
                return registrosAfectados > 0;
            }
        }
        public async Task<bool> VerificarTraslapeDeFechaAsync(int idEntidad, int idUsuario, DateTime Fecha, int idHistoricoInhabilitacion)
        {
            using (var connection = await _connectionProvider.OpenAsync())
            {
                string instruccionSQL = @"
            SELECT COUNT(1) 
            FROM HISTORICO_INHABILITACIONES 
            WHERE ID_ENTIDAD = @idEntidad 
              AND ID_USUARIO = @idUsuario 
              AND CONVERT(DATE, @Fecha) BETWEEN CONVERT(DATE, FECHA_INICIO) AND CONVERT(DATE, FECHA_FIN)
              AND ID_HISTORICO_INHABILITACION <> @idHistoricoInhabilitacion";

                int count = await connection.ExecuteScalarAsync<int>(instruccionSQL, new { idEntidad, idUsuario, Fecha, idHistoricoInhabilitacion });

                return count > 0;
            }
        }
    }
}
