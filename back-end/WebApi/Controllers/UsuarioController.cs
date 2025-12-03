using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Exceptionless;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Qfile.Core.Modelos;
using Qfile.Core.Servicios;
using Qfile.Core.Tipos;
using WebApi.Modelos;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationSettingsModelo _appSetings;
        private readonly IUsuarioServicio _servicio;

        public UsuarioController(IOptions<ApplicationSettingsModelo> appSettings, IUsuarioServicio servicio)
        {
            _appSetings = appSettings.Value;
            _servicio = servicio;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] UsuarioModelo usuario)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuarioRegistro = 0;

                if (identity != null)
                {
                    idUsuarioRegistro = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                var result = await _servicio.CrearUsuarioAsync(usuario, idUsuarioRegistro);

                if (result == -1)
                    return StatusCode(409, "Ya existe un usuario con el mismo Número de Identificación Personal o Correo Electrónico");
                else
                    return Ok();
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{idEntidad}/{idUsuario}")]
        public async Task<IActionResult> EditarUsuario(int idEntidad, int idUsuario, [FromBody] UsuarioModelo usuario)
        {
            try
            {
                usuario.IdEntidad = idEntidad;
                usuario.IdUsuario = idUsuario;

                var result = await _servicio.EditarUsuarioAsync(usuario);
                return Ok();
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ObtenerUsuarios([FromQuery] int Pagina, [FromQuery] int Cantidad, [FromQuery] string BuscarTexto)
        {
            try
            {
                var listaUsuarios = await _servicio.ObtenerUsuariosAsync(Pagina, Cantidad, BuscarTexto);

                return Ok(new
                {
                    listaUsuarios = listaUsuarios,
                    cantidadTotal = listaUsuarios.Count() > 0 ? listaUsuarios[0].CantidadTotal : 0
                });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{idUsuario}")]
        public async Task<IActionResult> ObtenerUsuario(int idUsuario)
        {
            try
            {
                var result = await _servicio.ObtenerUsuarioAsync(idUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{idEntidad}/{idUsuario}")]
        public async Task<IActionResult> EliminarUsuario(int idEntidad, int idUsuario)
        {
            try
            {
                var result = await _servicio.EliminarUsuarioAsync(idEntidad, idUsuario);
                return Ok();
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

    }
}
