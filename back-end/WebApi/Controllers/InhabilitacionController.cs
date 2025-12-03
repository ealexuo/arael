using Exceptionless;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qfile.Core.Modelos;
using Qfile.Core.Servicios;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InhabilitacionController : ControllerBase
    {
        private readonly IInhabilitacionServicio _servicio;

        public InhabilitacionController(IInhabilitacionServicio servicio)
        {
            _servicio = servicio;
        }

        // GET: api/<InhabilitacionController>
        [Authorize]
        [HttpGet]
        [HttpGet("ObtenerInhabilitacionesUsuario/{idUsuario}")]        
        public async Task<IActionResult> ObtenerInhabilitacionesUsuario(int idUsuario)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }               

                var result = await _servicio.ObtenerInhabilitacionesUsuarioAsync(idEntidad, idUsuario);
                    return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<InhabilitacionController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<InhabilitacionController>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] HistoricoInhabilitacionModelo inhabilitacion)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuarioRegistro = 0;

                if (identity != null)
                {
                    idUsuarioRegistro = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    inhabilitacion.UsuarioRegistro = idUsuarioRegistro;
                    inhabilitacion.FechaRegistro = UtilidadesServicio.FechaActualUtc;
                }
                else
                {
                    throw new Exception("[InhabilitacionController][POST] Error al obtener usuario registro.");
                }
                
                var result = await _servicio.GuardarInhabilitacionAsync(inhabilitacion);

                if(result.ResultType == ResultType.Invalid)
                {
                    return BadRequest(result.Errors.FirstOrDefault());
                }

                return Ok();
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/<InhabilitacionController>/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> ObtenerInhabilitacionesUsuario([FromBody] HistoricoInhabilitacionModelo inhabilitacion)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    inhabilitacion.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ActualizarInhabilitacionAsync(inhabilitacion);
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
        public async Task<IActionResult> ActualizarOrigenAsync([FromBody] HistoricoInhabilitacionModelo inhabilitacion)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;                

                if (identity != null)
                {
                    inhabilitacion.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    inhabilitacion.UsuarioRegistro = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                var result = await _servicio.ActualizarInhabilitacionAsync(inhabilitacion);

                if (result.ResultType == ResultType.Invalid)
                {
                    return BadRequest(result.Errors.FirstOrDefault());
                }

                return Ok();
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<InhabilitacionController>/5
        [HttpDelete("{idUsuario}/{idInhabilitacion}")]
        public async Task<IActionResult> Delete(int idUsuario, int idInhabilitacion)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.EliminarInhabilitacionAsync(idEntidad, idUsuario, idInhabilitacion);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
