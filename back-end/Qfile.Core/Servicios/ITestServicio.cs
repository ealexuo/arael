using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public interface ITestServicio
    {
        Task<string> GetTestAsync();

        Task<string> ReadinessProbe();
    }
}
