using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qfile.Core.Servicios.Documentos;
using Microsoft.AspNetCore.Authorization;
using Exceptionless;
using System.Security.Claims;
using Qfile.Core.Servicios;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentoController : ControllerBase
    {
        private readonly IDocumentoServicio _servicio;
        private readonly IProcesoPermisoServicio _servicioProcesoPermiso;
        private readonly IExpedienteServicio _servicioExpediente;

        public DocumentoController(IDocumentoServicio servicio,
            IExpedienteServicio servicioExpediente,
            IProcesoPermisoServicio servicioProcesoPermiso)
        {
            _servicio = servicio;
            _servicioExpediente = servicioExpediente;
            _servicioProcesoPermiso = servicioProcesoPermiso;
        }

        [Authorize]
        [HttpPost("{idExpediente}")]
        public async Task<IActionResult> GuardarDocumento(IFormFile documento, int idExpediente)
        {
            try
            {
                ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;
                int idEntidad = 0;

                if (identity != null)
                {
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var resultado = await _servicio.GuardarDocumentoAsync(documento, idUsuario, idEntidad, idExpediente);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("{idExpediente}/{idDocumento}")]
        public async Task<IActionResult> ReemplazarDocumento(IFormFile documento, [FromForm] string observaciones, int idExpediente, int idDocumento)
        {
            try
            {
                ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;
                int idEntidad = 0;               

                if (identity != null)
                {
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var resultado = await _servicio.ReemplazarDocumentoAsync(documento, idUsuario, idEntidad, idExpediente, idDocumento, observaciones);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{idExpediente}/{nombreDocumento}")]
        public async Task<IActionResult> ObtenerDocumento(int idExpediente, string nombreDocumento)
        {
            try
            {
                var resultado = await _servicio.ObtenerDocumentoAsync(idExpediente, nombreDocumento);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{idExpediente}")]
        public async Task<IActionResult> ObtenerDocumentos(int idExpediente)
        {
            try
            {
                // Revisar el permiso por proceso
                ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;
                int idEntidad = 0;

                if (identity != null)
                {
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var expediente = await _servicioExpediente.ObtenerExpedienteAsync(idExpediente, idUsuario, idEntidad);
                var tieneProcesoPermiso = await _servicioProcesoPermiso.UsuarioTienePermiso("Consulta de documentos", idUsuario, idEntidad, expediente.IdProceso);

                if (!tieneProcesoPermiso)
                    return Forbid();

                var resultado = await _servicio.ObtenerDocumentosAsync(idExpediente);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("historico/{idDocumento}")]
        public async Task<IActionResult> ObtenerHistoricoDocumento(int idDocumento)
        {
            try
            {
                var resultado = await _servicio.ObtenerHistoricoDocumentoAsync(idDocumento);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("descargar/{idDocumento}/{esDocumentoActual}")]
        public async Task<IActionResult> DescargarDocumento(int idDocumento, bool esDocumentoActual)
        {
            try
            {
                var resultado = await _servicio.DescargarDocumento(idDocumento, esDocumentoActual);

                if (resultado == null)
                    return NotFound();

                return File(resultado, "application/octet-stream");
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{idDocumento}")]
        public async Task<IActionResult> EliminarDocumento(int idDocumento)
        {
            try
            {

                // Revisar el permiso por proceso
                ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;
                int idEntidad = 0;

                if (identity != null)
                {
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var documento = await _servicio.ObtenerDocumentoPorIdAsync(idDocumento);
                var expediente = await _servicioExpediente.ObtenerExpedienteAsync(documento.IdExpediente, idUsuario, idEntidad);
                var tieneProcesoPermiso = await _servicioProcesoPermiso.UsuarioTienePermiso("Eliminación de documentos", idUsuario, idEntidad, expediente.IdProceso);

                if (!tieneProcesoPermiso)
                    return Forbid();

                bool resultado = await _servicio.EliminarDocumentoAsync(idDocumento);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }
    }
}
