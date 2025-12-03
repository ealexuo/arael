using Exceptionless;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qfile.Core.Modelos;
using Qfile.Core.Servicios;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrigenController : ControllerBase
    {
        private readonly IOrigenServicio _servicio;

        public OrigenController(IOrigenServicio servicio)
        {
            _servicio = servicio;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ObtenerOrigenes()
        {
            try
            {
                List<OrigenModelo> listaOrigenes = await _servicio.ObtenerOrigenesAsync();
                return Ok(listaOrigenes);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{idOrigen}")]
        public async Task<IActionResult> ObtenerOrigen(int idOrigen)
        {
            try
            {
                OrigenModelo origen = await _servicio.ObtenerOrigenAsync(idOrigen);
                return Ok(origen);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CrearOrigen([FromBody] OrigenModelo origen)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                var result = await _servicio.CrearOrigenAsync(origen, idUsuario, idEntidad);

                return Ok();
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> ActualizarOrigenAsync([FromBody] OrigenModelo origen)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                var result = await _servicio.ActualizarOrigenAsync(origen, idUsuario, idEntidad);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{idOrigen}")]
        public async Task<IActionResult> EliminarOrigenAsync(int idOrigen)
        {
            try
            {
                var result = await _servicio.EliminarOrigenAsync(idOrigen);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
