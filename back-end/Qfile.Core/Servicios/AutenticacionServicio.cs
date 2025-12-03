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
    public class AutenticacionServicio : IAutenticacionServicio
    {
        private readonly IAutenticacionDatos _datos;
        private readonly IUsuarioDatos _usuarioDatos;
        private readonly IEncryptPasswordServicio _encryptServicio;
        private readonly ICorreoElectronicoServicio _correoElectronicoServicio;

        public AutenticacionServicio(IAutenticacionDatos datos, IUsuarioDatos usuarioDatos, IEncryptPasswordServicio encryptServicio, ICorreoElectronicoServicio correoElectronicoServicio)
        {
            _datos = datos;
            _usuarioDatos = usuarioDatos;
            _encryptServicio = encryptServicio;
            _correoElectronicoServicio = correoElectronicoServicio;
        }

        public async Task<int> ValidaPasswordAsync(int idUsuario, string password)
        {
            var datosPassword = await _datos.ObtenerPasswordBDDAsync(idUsuario);

            if (datosPassword == null)
                return ValidaPasswordTipos.Invalido;

            var esPasswordValido = _encryptServicio.VerifyHashedPassword(datosPassword.Password, password);

            if (esPasswordValido && (datosPassword.RequiereCambioPassword))
            {
                return ValidaPasswordTipos.ValidoRequiereCambio;
            }
            else if (esPasswordValido)
            {
                return ValidaPasswordTipos.Valido;
            }
            else
            {
                return ValidaPasswordTipos.Invalido;
            }
        }

        public async Task<int> CambiarPasswordAsync(int idUsuario, string passwordNuevo)
        {
            var password = _encryptServicio.HashPassword(passwordNuevo);
            var resultado = await _datos.CambiarPasswordAsync(idUsuario, password);

            //TODO: Definir plantilla para correo
            if (resultado != 0)
            {
                var usuario = await _usuarioDatos.ObtenerPorIdAsync(idUsuario);
                await _correoElectronicoServicio.Enviar(
                    usuario.CorreoElectronico,
                    "QFile - Su Password ha sido modificado",
                    "<p>Su password ha sido modificado exitosamente.</p> <a href=\"" + UtilidadesServicio.UrlQfile + "\">Click Aqui</a>"
                );
            }

            return resultado;
        }

        public async Task<int> RegistrarLogeo(int idEntidad, int idUsuario)
        {
            DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;
            return await _datos.RegistrarLogeo(idEntidad, idUsuario, fechaRegistro);
        }
    }
}
