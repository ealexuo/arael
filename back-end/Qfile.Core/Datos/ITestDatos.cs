using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Datos
{
    public interface ITestDatos
    {
        Task<string> GetTestAsync();

        Task<string> ReadinessProbe();
    }
}
