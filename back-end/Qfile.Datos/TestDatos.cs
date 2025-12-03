using Qfile.Core.Datos;
using System;
using Dapper;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Qfile.Datos
{
    public class TestDatos: ITestDatos
    {
        private readonly IConnectionProvider connectionProvider;

        public TestDatos(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }

        public async Task<string> GetTestAsync()
        {
            await Task.Delay(1000);
            return "Datos obtenidos desde el back end";
        }

        public async Task<string> ReadinessProbe() {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"select 200 from dual";

                var result = await connection.QueryAsync<string>(sqlQuery);

                return result.FirstOrDefault();
            }
        }
    }
}
