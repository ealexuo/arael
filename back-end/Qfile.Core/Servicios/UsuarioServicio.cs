using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using Qfile.Core.Tipos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public class UsuarioServicio : IUsuarioServicio
    {
        private readonly IUsuarioDatos _datos;
        private readonly IEncryptPasswordServicio _encryptServicio;
        private readonly ICorreoElectronicoServicio _correoElectronicoServicio;
        private readonly IInhabilitacionServicio _inhabilitacionServicio;

        public UsuarioServicio(
            IUsuarioDatos datos, 
            IEncryptPasswordServicio encryptServicio, 
            ICorreoElectronicoServicio correoElectronicoServicio,
            IInhabilitacionServicio inhabilitacionServicio
            )
        {
            _datos = datos;
            _encryptServicio = encryptServicio;
            _correoElectronicoServicio = correoElectronicoServicio;
            _inhabilitacionServicio = inhabilitacionServicio;
        }

        public async Task<UsuarioModelo> ObtenerPorNombreUsuarioAsync(string nombreUsuario)
        {
            string correoElectronico = "";
            string identificacionPersonal = "";

            Regex regexCorreo = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regexCorreo.Match(nombreUsuario);

            if (match.Success)
                correoElectronico = nombreUsuario;
            else
                identificacionPersonal = nombreUsuario;

            return await _datos.ObtenerPorNombreUsuarioAsync(identificacionPersonal, correoElectronico);
        }

        public async Task<UsuarioModelo> ObtenerPorIdAsync(int idUsuario)
        {
            return await _datos.ObtenerPorIdAsync(idUsuario);
        }

        public async Task<int> CrearUsuarioAsync(UsuarioModelo usuario, int idUsuarioRegistro)
        {
            var usuarioExistente = await _datos.ObtenerPorNombreUsuarioAsync(usuario.NoIdentificacionPersonal, usuario.CorreoElectronico);

            if (usuarioExistente == null)
            {
                string passwordTemporal = UtilidadesServicio.GenerarCadenaAleatoria();
                int resultado = await _datos.CrearUsuarioAsync(usuario, _encryptServicio.HashPassword(passwordTemporal), UtilidadesServicio.FechaActualUtc, idUsuarioRegistro);

                if (resultado != 0)
                {
                    await _correoElectronicoServicio.Enviar(
                        usuario.CorreoElectronico,
                        "QFile - Su Password Temporal",
                        "<p>Su nombre de usuario es: " + usuario.NoIdentificacionPersonal + ". Su contraseña temporal es: " + passwordTemporal + "</p> <a href=\"" + UtilidadesServicio.UrlQfile + "\">Click Aqui</a>"
                    );
                }

                return resultado;
            }
            else 
                return -1;
        }

        public async Task<int> EditarUsuarioAsync(UsuarioModelo usuario)
        {
            return await _datos.EditarUsuarioAsync(usuario, UtilidadesServicio.FechaActualUtc);
        }

        public async Task<List<UsuarioListaModelo>> ObtenerUsuariosAsync(int pagina, int cantidad, string buscarTexto)
        {
            var listaUsuarios = await _datos.ObtenerUsuariosAsync(pagina, cantidad, buscarTexto);
            
            foreach(var usuario in listaUsuarios)
            {
                usuario.FechasInhabilitacion = await _inhabilitacionServicio.ObtenerFechasInhabilitacionUsuarioAsync(usuario.IdUsuario);
            }

            return listaUsuarios;
        }

        public async Task<bool> EliminarUsuarioAsync(int idEntidad, int idUsuario)
        {
            return await _datos.EliminarUsuarioAsync(idEntidad, idUsuario);
        }

        public async Task<UsuarioModelo> ObtenerUsuarioAsync(int idUsuario)
        {
            return await _datos.ObtenerUsuarioAsync(idUsuario);
        }
    }
}
