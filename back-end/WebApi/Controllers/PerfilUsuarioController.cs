using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qfile.Core.Servicios;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilUsuarioController : ControllerBase
    {
        private IUsuarioServicio _usuarioServicio;

        public PerfilUsuarioController(IUsuarioServicio usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
        }

        [HttpGet]
        [Authorize]
        //GET : /api/PerfilUsuario
        public async Task<Object> ObtenerPerfilUsuario()
        {
            string idUsuario = User.Claims.First(c => c.Type == "IdUsuario").Value;
            var usuario = await _usuarioServicio.ObtenerPorIdAsync(Int32.Parse(idUsuario));

            return new
            {
                usuario.PrimerNombre,
                usuario.PrimerApellido,
                usuario.CorreoElectronico
            };
        }
    }
}