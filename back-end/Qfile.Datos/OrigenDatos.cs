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
    public class OrigenDatos : IOrigenDatos
    {
        private readonly IConnectionProvider connectionProvider;

        public OrigenDatos(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }

        public async Task<List<OrigenModelo>> ObtenerOrigenesAsync()
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"SELECT 
	                                    ID_ORIGEN AS IdOrigen, 
	                                    NOMBRE, 
	                                    ACTIVO, 
	                                    FECHA_REGISTRO AS FechaRegistro, 
	                                    ID_ENTIDAD AS IdEntidad, 
	                                    ID_USUARIO AS IdUsuarioRegistro
                                    FROM CA_ORIGENES";

                var result = await connection.QueryAsync<OrigenModelo>(sqlQuery, new {});

                return result.ToList();
            }
        }

        public async Task<OrigenModelo> ObtenerOrigenAsync(int idOrigen)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"SELECT 
	                                    ID_ORIGEN AS IdOrigen, 
	                                    NOMBRE, 
	                                    ACTIVO, 
	                                    FECHA_REGISTRO AS FechaRegistro, 
	                                    ID_ENTIDAD AS IdEntidad, 
	                                    ID_USUARIO AS IdUsuarioRegistro
                                    FROM CA_ORIGENES
                                    WHERE ID_ORIGEN = @IdOrigen";

                var result = await connection.QueryAsync<OrigenModelo>(sqlQuery, new { IdOrigen = idOrigen });

                return result.FirstOrDefault();
            }
        }

        public async Task<int> CrearOrigenAsync(OrigenModelo origen, int idUsuarioRegistro, DateTime fechaRegistro, int idEntidad)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            const string insertarExpedienteSQL = @"                
                INSERT INTO CA_ORIGENES
                (ID_ORIGEN, NOMBRE, ACTIVO, FECHA_REGISTRO, ID_ENTIDAD, ID_USUARIO)
                VALUES((select max (id_origen) + 1 from ca_origenes), @Nombre, @Activo, @FechaRegistro, @IdEntidad, @IdUsuarioRegistro)";

            using (var connection = await connectionProvider.OpenAsync())
            {

                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@IdOrigen", origen.IdOrigen);
                    param.Add("@Nombre", origen.Nombre);
                    param.Add("@Activo", origen.Activo);
                    param.Add("@FechaRegistro", fechaRegistro);
                    param.Add("@IdEntidad", idEntidad);
                    param.Add("@IdUsuarioRegistro", origen.IdUsuarioRegistro);

                    await connection.ExecuteAsync(insertarExpedienteSQL, param, trx);

                    origen.IdOrigen = param.Get<int>("IdOrigen");

                    trx.Commit();
                }
            }

            return origen.IdOrigen;
        }

        public async Task<bool> ActualizarOrigenAsync(OrigenModelo origen, int idUsuarioRegistro, int idEntidad)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                int registrosAfectados = 0;

                string instruccionSQL = @"UPDATE CA_ORIGENES 
                SET NOMBRE = @Nombre, 
                ACTIVO = @Activo
                WHERE ID_ORIGEN = @IdOrigen
                AND ID_ENTIDAD = @IdEntidad";

                //DBNull.Value
                using (var trx = connection.BeginTransaction())
                {
                    registrosAfectados += await connection.ExecuteAsync(instruccionSQL, new
                    {
                        origen.Nombre,
                        IdEntidad = idEntidad,                                                
                        Activo = origen.Activo ? 1 : 0,
                        origen.IdOrigen
                    }, trx);
                    trx.Commit();
                }
                return registrosAfectados > 0;
            }
        }

        public async Task<bool> EliminarOrigenAsync(int idOrigen)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                int registrosAfectados = 0;

                string instruccionSQL = @"DELETE CA_ORIGENES WHERE ID_ORIGEN = @idOrigen";

                using (var trx = connection.BeginTransaction())
                {
                    registrosAfectados += await connection.ExecuteAsync(instruccionSQL, new { idOrigen }, trx);
                    trx.Commit();
                }
                return registrosAfectados > 0;
            }
        }
    }
}
