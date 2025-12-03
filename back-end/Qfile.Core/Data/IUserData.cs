using Qfile.Core.Modelos;
using Qfile.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Data
{
    public interface IUserData
    {
        Task<UserModel> GetByUserNameAsync(string userName);
        /*
        Task<UsuarioModelo> ObtenerPorIdAsync(int idUsuario);
        Task<int> CrearUsuarioAsync(UsuarioModelo usuario, string password, DateTime fechaRegistro, int idUsuarioRegistro);
        Task<int> EditarUsuarioAsync(UsuarioModelo usuario, DateTime fechaRegistro);
        Task<List<UsuarioListaModelo>> ObtenerUsuariosAsync(int pagina, int cantidad, string buscarTexto);
        Task<bool> EliminarUsuarioAsync(int idEntidad, int idUsuario);
        Task<UsuarioModelo> ObtenerUsuarioAsync(int idUsuario);
        */
    }
}
