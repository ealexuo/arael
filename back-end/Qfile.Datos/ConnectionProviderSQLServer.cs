using Qfile.Core.Datos;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Qfile.Datos
{
    public class ConnectionProviderSQLServer : IConnectionProvider
    {
        private readonly IConfiguration configuration;

        public ConnectionProviderSQLServer(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IDbConnection> OpenAsync()
        {
            var connection = new SqlConnection(configuration.GetConnectionString("SQLConnection"));
            await connection.OpenAsync();
            return connection;
        }
    }
}