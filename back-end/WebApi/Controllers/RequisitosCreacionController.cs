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
    public class RequisitosCreacionController : Controller
    {
        private readonly ApplicationSettingsModelo _appSetings;
        private readonly IRequisitoGestionServicio _servicio;

        public RequisitosCreacionController(IOptions<ApplicationSettingsModelo> appSettings, IRequisitoGestionServicio servicio)
        {
            _appSetings = appSettings.Value;
            _servicio = servicio;
        }


        [Authorize]
        [HttpGet("ObtenerRequisitos/{idProceso}")]
        public async Task<IActionResult> ObtenerRequisitos(int idProceso)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }
                
                var result = await _servicio.ObtenerRequisitosAsync(idEntidad, idProceso);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("Requisito")]
        public async Task<IActionResult> CrearRequisito([FromBody] RequisitoGestionModelo requisito)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    requisito.UsuarioRegistro = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    requisito.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.CrearRequisitoAsync(requisito);
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
        public async Task<IActionResult> ActualizarRequisito([FromBody] RequisitoGestionModelo requisito)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    requisito.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ActualizarRequisitoAsync(requisito);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("Requisito/{IdProceso}/{IdRequisito}")]
        public async Task<IActionResult> EliminarRequisito(int IdProceso, int idRequisito)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.EliminarRequisitoAsync(idEntidad, IdProceso, idRequisito);
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