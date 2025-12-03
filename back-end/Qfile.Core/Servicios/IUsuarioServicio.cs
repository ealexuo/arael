using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public interface IUsuarioServicio
    {
        Task<UsuarioModelo> ObtenerPorNombreUsuarioAsync(string nombreUsuario);
        Task<UsuarioModelo> ObtenerPorIdAsync(int idUsuario);
        Task<int> CrearUsuarioAsync(UsuarioModelo usuario, int idUsuarioRegistro);
        Task<int> EditarUsuarioAsync(UsuarioModelo usuario);
        Task<List<UsuarioListaModelo>> ObtenerUsuariosAsync(int pagina, int cantidad, string buscarTexto);
        Task<bool> EliminarUsuarioAsync(int idEntidad, int idUsuario);
        Task<UsuarioModelo> ObtenerUsuarioAsync(int idUsuario);        
    }
}
