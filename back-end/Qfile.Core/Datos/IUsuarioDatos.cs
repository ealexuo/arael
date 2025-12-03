using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Datos
{
    public interface IUsuarioDatos
    {
        Task<UsuarioModelo> ObtenerPorNombreUsuarioAsync(string identificacionPersonal, string correoElectronico);
        Task<UsuarioModelo> ObtenerPorIdAsync(int idUsuario);
        Task<int> CrearUsuarioAsync(UsuarioModelo usuario, String password, DateTime fechaRegistro, int idUsuarioRegistro);
        Task<int> EditarUsuarioAsync(UsuarioModelo usuario, DateTime fechaRegistro);
        Task<List<UsuarioListaModelo>> ObtenerUsuariosAsync(int pagina, int cantidad, string buscarTexto);
        Task<bool> EliminarUsuarioAsync(int idEntidad, int idUsuario);
        Task<UsuarioModelo> ObtenerUsuarioAsync(int idUsuario);
    }
}
