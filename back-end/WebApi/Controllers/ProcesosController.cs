using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Qfile.Core.Modelos;
using Qfile.Core.Servicios;
using WebApi.Modelos;
using Exceptionless;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcesosController : Controller
    {
        private readonly ApplicationSettingsModelo _appSetings;
        private readonly IProcesoServicio _servicio;

        public ProcesosController(IOptions<ApplicationSettingsModelo> appSettings, IProcesoServicio servicio)
        {
            _appSetings = appSettings.Value;
            _servicio = servicio;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerTodosAsync(idEntidad);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CrearProceso([FromBody] ProcesoModelo proceso)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;

                if (identity != null)
                {
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    proceso.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.CrearProcesoAsync(proceso, idUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> ActualizarProceso([FromBody] ProcesoModelo proceso)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    proceso.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ActualizarProcesoAsync(proceso);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{idProceso}")]
        public async Task<IActionResult> EliminarProceso(int idProceso)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }
                if (await _servicio.existenExpedientes(idProceso))
                {
                    return Ok(new  { 
                        mensaje = "No es posible eliminar este Proceso.  Existen expedientes creados de este tipo de proceso."
                    });
                }

                if (await _servicio.existenSecciones(idProceso)) {
                    return Ok(new{
                        mensaje = "Es posible que el proceso contenga elementos internos como plantillas, fases, requisitos de gestión, entre otros.  Favor de revisar y eliminar estos elementos antes de volver a intentarlo.."
                    });
                }

                    var result = await _servicio.EliminarProcesoAsync(idEntidad, idProceso);
                    return Ok();
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{idProceso}")]
        public async Task<IActionResult> ObtenerProceso(int idProceso)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerProcesoAsync(idEntidad, idProceso);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("ObtenerProcesosActivos")]
        public async Task<IActionResult> ObtenerProcesosActivos()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerProcesosActivosAsync(idEntidad, idUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }
    }
}