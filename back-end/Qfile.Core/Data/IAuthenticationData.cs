using Org.BouncyCastle.Bcpg;
using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Data
{
    public interface IAuthenticationData
    {
        Task<int> LogSignInAsync(int UserId);
        /*Task<int> GuardarPassword(LoginModelo loginModelo);
        Task<int> CambiarPasswordAsync(int idUsuario, string password);*/
        
    }
}
