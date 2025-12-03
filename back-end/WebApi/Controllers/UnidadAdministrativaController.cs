using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qfile.Core.Modelos;
using Qfile.Core.Servicios;
using Exceptionless;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnidadAdministrativaController : ControllerBase
    {
        private readonly IUnidadAdministrativaServicio _unidadAdministrativaServicio;

        public UnidadAdministrativaController(IUnidadAdministrativaServicio unidadAdministrativaServicio)
        {
            _unidadAdministrativaServicio = unidadAdministrativaServicio;
        }

        [HttpPost]
        public async Task<IActionResult> CrearUnidadAdministrativa([FromBody] UnidadAdministrativaModelo unidadAdministrativa)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuarioRegistro = 0;

                if (identity != null)
                {
                    idUsuarioRegistro = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                if (unidadAdministrativa != null)
                    unidadAdministrativa.IdUsuarioRegistro = idUsuarioRegistro;

                if (unidadAdministrativa.IdUnidadAdministrativaPadre == 0)
                    unidadAdministrativa.IdUnidadAdministrativaPadre = null;

                var result = await _unidadAdministrativaServicio.CrearUnidadAdministrativaAsync(unidadAdministrativa);
                return Ok();
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarUnidadAdministrativa([FromBody] UnidadAdministrativaModelo unidadAdministrativa)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    unidadAdministrativa.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                if (unidadAdministrativa.IdUnidadAdministrativaPadre == 0)
                    unidadAdministrativa.IdUnidadAdministrativaPadre = null;

                var result = await _unidadAdministrativaServicio.ActualizarUnidadAdministrativaAsync(unidadAdministrativa);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{idUnidadAdministrativa}")]
        public async Task<IActionResult> EliminarUnidadAdministrativa(int idUnidadAdministrativa)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                //var idEntidad = 0;

                //if (identity != null)
                //{
                //    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                //}

                var result = await _unidadAdministrativaServicio.EliminarUnidadAdministrativaAsync(idUnidadAdministrativa);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUnidadesAdministrativas()
        {
            try
            {
                var result = await _unidadAdministrativaServicio.ObtenerUnidadesAdministrativasAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{idUnidadAdministrativa}")]
        public async Task<IActionResult> ObtenerUnidadAdministrativa(int idUnidadAdministrativa)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                //int idEntidad = 0;

                //if (identity != null)
                //{
                //    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                //}

                var result = await _unidadAdministrativaServicio.ObtenerPorIdAsync(idUnidadAdministrativa);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}