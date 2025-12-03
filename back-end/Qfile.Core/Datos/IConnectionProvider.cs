using System.Data;
using System.Threading.Tasks;

namespace Qfile.Core.Datos
{
    public interface IConnectionProvider
    {
        Task<IDbConnection> OpenAsync();
    }
}
