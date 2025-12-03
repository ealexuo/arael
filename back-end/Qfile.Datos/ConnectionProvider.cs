using Qfile.Core.Datos;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Datos
{
    public class ConnectionProvider : IConnectionProvider
    {
        private readonly IConfiguration configuration;

        public ConnectionProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IDbConnection> OpenAsync()
        {
            var connection = new OracleConnection(configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            return connection;
        }
    }
}
