using Exceptionless;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qfile.Core.Modelos;
using Qfile.Core.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcesoPermisoController : ControllerBase
    {
        private readonly IProcesoPermisoServicio _servicio;

        public ProcesoPermisoController(IProcesoPermisoServicio servicio)
        {
            _servicio = servicio;
        }

        [Authorize]
        [HttpGet("{idUsuario}")]
        public async Task<IActionResult> ObtenerPermisos(int idUsuario)
        {
            try
            {
                ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
                List<ProcesoPermisosModelo> listaProcesosPermisos = null;

                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    listaProcesosPermisos = await _servicio.ObtenerProcesosPermisosPorUsuarioAsync(idEntidad, idUsuario);
                }
                
                return Ok(listaProcesosPermisos);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GuardarPermisos([FromBody] ProcesosPermisosUsuarioModelo procesosPermisosUsuario)
        {
            try
            {
                int resutlado = await _servicio.GuardarPermisosAsync(procesosPermisosUsuario);
                return Ok(resutlado);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        /*
        [Authorize]
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [Authorize]
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [Authorize]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        */
    }
}
