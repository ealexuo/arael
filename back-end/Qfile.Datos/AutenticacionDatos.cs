using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using Dapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Data;

namespace Qfile.Datos
{
    public class AutenticacionDatos : IAutenticacionDatos
    {
        private readonly IConnectionProvider connectionProvider;

        public AutenticacionDatos(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }

        public async Task<LoginModelo> ObtenerPasswordBDDAsync(int idUsuario)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"select password, REQUIERE_CAMBIO_PASSWORD as RequiereCambioPassword from ad_usuarios where ID_USUARIO = @idUsuario";

                var result = await connection.QueryAsync<LoginModelo>(sqlQuery, new { idUsuario });

                return result.FirstOrDefault();
            }
        }

        public async Task<int> GuardarPassword(LoginModelo loginModelo)
        {
            const string InsertarUsuarioSql = @"UPDATE AD_USUARIOS SET PASSWORD = @Password WHERE ID_USUARIO = @idUsuario";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var response = await connection.ExecuteAsync(InsertarUsuarioSql, new
                {
                    loginModelo.idUsuario,
                    loginModelo.Password
                });

                return response;
            }
        }

        public async Task<int> CambiarPasswordAsync(int idUsuario, string password)
        {
            const string InsertarUsuarioSql = @"UPDATE AD_USUARIOS SET PASSWORD = @Password, REQUIERE_CAMBIO_PASSWORD = 0 WHERE ID_USUARIO = @idUsuario";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var response = await connection.ExecuteAsync(InsertarUsuarioSql, new
                {
                    idUsuario,
                    password
                });

                return response;
            }
        }

        public async Task<int> RegistrarLogeo(int idEntidad, int idUsuario, DateTime fechaRegistro)
        {
            const string InsertarHistoricoLogeoSql = @"INSERT INTO HISTORICO_LOGUEO (ID_ENTIDAD, ID_USUARIO, FECHA_REGISTRO)
                VALUES(@IdEntidad, @IdUsuario, @FechaRegistro)";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var response = await connection.ExecuteAsync(InsertarHistoricoLogeoSql, new
                {
                    IdEntidad = idEntidad,
                    IdUsuario = idUsuario,
                    FechaRegistro = fechaRegistro
                });

                return response;
            }

        }
    }
}
