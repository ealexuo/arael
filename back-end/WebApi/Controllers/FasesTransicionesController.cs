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
    public class FasesTransicionesController : Controller
    {
        private readonly ApplicationSettingsModelo _appSetings;
        private readonly IFasesTransicionesServicio _servicio;

        public FasesTransicionesController(IOptions<ApplicationSettingsModelo> appSettings, IFasesTransicionesServicio servicio)
        {
            _appSetings = appSettings.Value;
            _servicio = servicio;
        }

        #region Fases

        [Authorize]
        [HttpGet("ObtenerFases/{idProceso}")]
        public async Task<IActionResult> ObtenerFases(int idProceso)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerFasesAsync(idEntidad, idProceso);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("ObtenerTiposFases/")]
        public async Task<IActionResult> ObtenerTiposFases(int idProceso)
        {
            try
            {
                var result = await _servicio.ObtenerTiposFasesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("ObtenerTiposAccesos/")]
        public async Task<IActionResult> ObtenerTiposAccesos()
        {
            try
            {
                var result = await _servicio.ObtenerTiposAccesosAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("ObtenerUnidadesMedida/")]
        public async Task<IActionResult> ObtenerUnidadesMedida()
        {
            try
            {
                var result = await _servicio.ObtenerUnidadesMedidaAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("Fase")]
        public async Task<IActionResult> CrearFase([FromBody] FaseModelo fase)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    fase.UsuarioRegistro = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    fase.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.CrearFaseAsync(fase);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("Fase/{IdProceso}/{IdFase}")]
        public async Task<IActionResult> EliminarFase(int IdProceso, int IdFase)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.EliminarFaseAsync(idEntidad, IdProceso, IdFase);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("Fase")]
        public async Task<IActionResult> ActualizaFase([FromBody] FaseModelo fase)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    fase.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ActualizarFaseAsync(fase);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region Usuarios Por fase

        [Authorize]
        [HttpGet("UsuariosFase/{idProceso}/{idFase}")]
        public async Task<IActionResult> ObtenerUsuariosPorFase(int idProceso, int idFase)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerUsuariosPorFaseAsync(idEntidad, idProceso, idFase);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("UsuariosFase")]
        public async Task<IActionResult> CrearUsuarioFaseAsync([FromBody] UsuarioFaseModelo usuario)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    usuario.UsuarioRegistro = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    usuario.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.CrearUsuarioFaseAsync(usuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("UsuariosFase/{IdProceso}/{IdFase}/{IdUsuario}")]
        public async Task<IActionResult> EliminarUsuarioFase(int IdProceso, int IdFase, int idUsuario)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.EliminarUsuarioFaseAsync(idEntidad, IdProceso, IdFase, idUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("UsuariosFase/ObtenerPendientes/{idProceso}/{IdFase}/{idUnidadAdministrativa}")]
        public async Task<IActionResult> ObtenerUsuariosPorUA(int idProceso, int idFase, int idUnidadAdministrativa)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerUsuariosPorUAAsync(idEntidad, idProceso, idFase, idUnidadAdministrativa);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("UsuariosFase")]
        public async Task<IActionResult> PermisoRecepcion([FromBody] UsuarioFaseModelo usuario)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    usuario.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.PermisoRecepcionAsync(usuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region Transiciones

        [Authorize]
        [HttpGet("Transiciones/{idProceso}/{idFaseOrigen}")]
        public async Task<IActionResult> ObtenerTransicionesPorFase(int idProceso, int idFaseOrigen)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerTransicionesPorFaseAsync(idEntidad, idProceso, idFaseOrigen);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("Transiciones")]
        public async Task<IActionResult> CrearTransicione([FromBody] TransicionesModelo transicion)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    transicion.UsuarioRegistro = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    transicion.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.CrearTransicioneAsync(transicion);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("Transiciones/{IdProceso}/{IdFaseOrigen}/{idFaseDestino}")]
        public async Task<IActionResult> EliminarTransicion(int IdProceso, int IdFaseOrigen, int idFaseDestino)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.EliminarTransicionAsync(idEntidad, IdProceso, IdFaseOrigen, idFaseDestino);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("Transiciones/ObtenerPendientes/{idProceso}/{IdFaseOrigen}")]
        public async Task<IActionResult> ObtenerFasesPendientes(int idProceso, int idFaseOrigen)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerFasesPendientesAsync(idEntidad, idProceso, idFaseOrigen);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("Transiciones")]
        public async Task<IActionResult> ActivarTransicion([FromBody] TransicionesModelo transicion)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    transicion.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ActivarTransicionAsync(transicion);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region Usuarios Por Transición

        [Authorize]
        [HttpGet("TransicionUsuarios/{idProceso}/{idFaseOrigen}/{IdFaseDestino}")]
        public async Task<IActionResult> ObtenerUsuariosPorTransicion(int idProceso, int idFaseOrigen, int idFaseDestino)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerUsuariosPorTransicionAsync(idEntidad, idProceso, idFaseOrigen, idFaseDestino);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("TransicionUsuarios")]
        public async Task<IActionResult> CrearUsuarioTransicion([FromBody] TransicionUsuarioModelo usuario)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    usuario.UsuarioRegistro = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    usuario.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.CrearUsuarioTransicionAsync(usuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("TransicionUsuarios/{IdProceso}/{IdFaseOrigen}/{IdFaseDestino}/{IdUsuario}")]
        public async Task<IActionResult> EliminarUsuarioTransicion(int IdProceso, int IdFaseOrigen, int IdFaseDestino, int idUsuario)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.EliminarUsuarioTransicionAsync(idEntidad, IdProceso, IdFaseOrigen, IdFaseDestino, idUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("TransicionUsuarios/ObtenerPendientes/{idProceso}/{idFaseOrigen}/{IdFaseDestino}")]
        public async Task<IActionResult> ObtenerUsuariosTransicionPendientes(int idProceso, int idFaseOrigen, int idFaseDestino)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerUsuariosTransicionPendientesAsync(idEntidad, idProceso, idFaseOrigen, idFaseDestino);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region Transicion Notificacioes

        [Authorize]
        [HttpPost("TransicionNotificaciones")]
        public async Task<IActionResult> CrearNotificacionTransicion([FromBody] TransicionNotificacionModelo notificacion)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    notificacion.UsuarioRegistro = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    notificacion.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.CrearNotificacionTransicionAsync(notificacion);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("TransicionNotificaciones/{IdProceso}/{IdFaseOrigen}/{IdFaseDestino}/{Correo}")]
        public async Task<IActionResult> EliminarNotificacionTransicionAsync(int IdProceso, int IdFaseOrigen, int IdFaseDestino, string correo)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.EliminarNotificacionTransicionAsync(idEntidad, IdProceso, IdFaseOrigen, IdFaseDestino, correo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("TransicionNotificaciones/{idProceso}/{idFaseOrigen}/{IdFaseDestino}")]
        public async Task<IActionResult> ObtenerNotificacionesPorTransicion(int idProceso, int idFaseOrigen, int idFaseDestino)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerNotificacionesPorTransicionAsync(idEntidad, idProceso, idFaseOrigen, idFaseDestino);

                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }


        #endregion

        #region Requisito por Transición

        [Authorize]
        [HttpGet("RequisitosPorTransicion/{idProceso}/{idFaseOrigen}/{idFaseDestino}")]
        public async Task<IActionResult> ObtenerRequisitosPorTransicion(int idProceso, int idFaseOrigen, int idFaseDestino)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerRequisitosPorTransicionAsync(idEntidad, idProceso, idFaseOrigen, idFaseDestino);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("RequisitosPorTransicion")]
        public async Task<IActionResult> CrearRequisitoPorTransicion([FromBody] RequisitoPorTransicionModelo requisito)
        {

            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    requisito.UsuarioRegistro = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    requisito.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.CrearRequisitoPorTransicionAsync(requisito); return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("RequisitosPorTransicion/{IdProceso}/{IdFaseOrigen}/{idFaseDestino}/{idRequisito}")]
        public async Task<IActionResult> EliminarRequisitoPorTransicion(int idProceso, int idFaseOrigen, int idFaseDestino, int idRequisito)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }
                var result = await _servicio.EliminarRequisitoPorTransicionAsync(idEntidad, idProceso, idFaseOrigen, idFaseDestino, idRequisito);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("RequisitosPorTransicion/{IdProceso}/{IdFaseOrigen}/{idFaseDestino}/{idRequisito}")]
        public async Task<IActionResult> ActualizarRequisitoPorTransicion([FromBody] RequisitoPorTransicionModelo requisito)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    requisito.UsuarioRegistro = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    requisito.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ActualizarRequisitoPorTransicionAsync(requisito);

                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }
        #endregion
    }
}