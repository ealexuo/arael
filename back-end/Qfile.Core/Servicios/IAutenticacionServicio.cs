using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public interface IAutenticacionServicio
    {
        Task<int> ValidaPasswordAsync(int idUsuario, string password);
        Task<int> CambiarPasswordAsync(int idUsuario, string passwordNuevo);
        Task<int> RegistrarLogeo(int idEntidad, int idUsuario);
    }
}
