using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Datos
{
    public interface IAutenticacionDatos
    {
        Task<LoginModelo> ObtenerPasswordBDDAsync(int idUsuario);
        Task<int> GuardarPassword(LoginModelo loginModelo);
        Task<int> CambiarPasswordAsync(int idUsuario, string password);
        Task<int> RegistrarLogeo(int idEntidad, int idUsuario, DateTime fechaRegistro);
    }
}
