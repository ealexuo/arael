using Exceptionless;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Qfile.Core.Modelos;
using Qfile.Core.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpedienteController : ControllerBase
    {
        private readonly IExpedienteServicio _servicio;

        public ExpedienteController(IExpedienteServicio servicio)
        {
            _servicio = servicio;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ObtenerExpedientes([FromQuery] int Pagina, [FromQuery] int Cantidad, [FromQuery] string BuscarTexto)
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

                var listaExpedientes = await _servicio.ObtenerExpedientesAsync(idEntidad, idUsuario, Pagina, Cantidad, BuscarTexto);

                return Ok(new
                {
                    listaExpedientes = listaExpedientes,
                    cantidadTotal = listaExpedientes.Count() > 0 ? listaExpedientes[0].CantidadTotal : 0
                });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GuardarExpediente([FromBody] ExpedienteModelo expediente)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;

                if (identity != null)
                {
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    expediente.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.GuardarExpedienteAsync(expediente, idUsuario);

                return Ok();
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{idExpediente}")]
        public async Task<IActionResult> ObtenerExpediente(int idExpediente)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;
                int idEntidad = 0;

                if (identity != null)
                {
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerExpedienteAsync(idExpediente, idUsuario, idEntidad);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }


        [Authorize]
        [HttpGet("ObtenerDatos/{idExpediente}")]
        public async Task<IActionResult> ObtenerExpedienteDatos(int idExpediente)
        {
            try
            {
                var result = await _servicio.ObtenerExpedienteDatosAsync(idExpediente);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("GuardarDatos")]
        public async Task<IActionResult> GuardarExpedienteDatos([FromBody] List<ExpedienteSeccionDatosModelo> datosExpediente)
        {
            try
            {
                var result = await _servicio.GuardarExpedienteDatosAsync(datosExpediente);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("ObtenerRequisitos/{idExpediente}")]
        public async Task<IActionResult> ObtenerRequisitos(int idExpediente)
        {
            try
            {
                var listaRequisitos = await _servicio.ListaRequisitosGestion(idExpediente);

                return Ok(listaRequisitos);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("GuardarRequisitos")]
        public async Task<IActionResult> GuardarRequisito([FromBody] ExpedienteRequisitosModelo requisito)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;

                if (identity != null)
                {
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                requisito.IdUsuarioRegistro = idUsuario;
              
                var result = await _servicio.GuardarRequisitoAsync(requisito);

                return Ok();
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("ImprimirRequisitos/{idExpediente}")]
        public async Task<IActionResult> ImprimirRequisitosGestion(int idExpediente, List<ExpedienteRequisitosModelo> requisitos) {

            try
            {
                var result = await _servicio.ImprimirRequisitosGestion(idExpediente, requisitos);
                return File(result, "application/pdf");
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("ImprimirCedula/{idExpediente}")]
        public async Task<IActionResult> ImprimirCedula(int idExpediente, CedulaExpedienteModelo cedulaExpediente)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;
                int idEntidad = 0;

                if (identity != null)
                {
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ImprimirCedula(idExpediente, cedulaExpediente, idUsuario, idEntidad);
                return File(result, "application/pdf");
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("ObtenerTraslados/{idExpediente}")]
        public async Task<IActionResult> ObtenerTraslados(int idExpediente)
        {
            try
            {
                var listaTraslados = await _servicio.ObtenerTraslados(idExpediente);

                return Ok(listaTraslados);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize]
        [HttpGet("ObtenerAsignaciones/{idProceso}/{idExpediente}/{fechaTraslado}")]
        public async Task<IActionResult> ObtenerAsignaciones(int idProceso, int idExpediente, DateTime fechaTraslado)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var listaAsignaciones = await _servicio.ObtenerAsignaciones(idEntidad, idProceso, idExpediente, fechaTraslado);

                return Ok(listaAsignaciones);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("ObtenerAnotaciones/{idProceso}/{idExpediente}/{fechaTraslado}/{fechaAsignacion?}")]
        public async Task<IActionResult> ObtenerAnotaciones(int idProceso, int idExpediente, DateTime fechaTraslado, DateTime? fechaAsignacion=null)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var listaAsignaciones = await _servicio.ObtenerAnotaciones(idEntidad, idProceso, idExpediente, fechaTraslado, fechaAsignacion);

                return Ok(listaAsignaciones);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("ObtenerAsignacionInterna/{idProceso}/{idExpediente}")]
        public async Task<IActionResult> ObtenerAsignacionInterna(int idProceso, int idExpediente)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var asignacion = await _servicio.ObtenerAsignacionInterna(idEntidad, idProceso, idExpediente);

                return Ok(asignacion);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("ObtenerFasesTraslado/{idExpediente}")]
        public async Task<IActionResult> ObtenerFasesTraslado(int idExpediente)
        {
            try
            {
                var asignacion = await _servicio.ObtenerFasesUsuariosTraslado( idExpediente);

                return Ok(asignacion);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("TrasladarExpediente")]
        public async Task<IActionResult> TrasladarExpediente([FromBody] TrasladoModelo expediente)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                expediente.IdUsuarioRegistro = idUsuario;
                expediente.IdEntidad = idEntidad;

                var result = await _servicio.TrasladarExpediente(expediente);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("TrasladarMasivamenteExpedientes")]
        public async Task<IActionResult> TrasladarMasivamenteExpedientes([FromBody] TrasladoModelo[] expedientes)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                foreach (var expediente in expedientes)
                {
                    expediente.IdUsuarioRegistro = idUsuario;
                    expediente.IdEntidad = idEntidad;
                }
                var result = await _servicio.TrasladarMasivamenteExpedientes(expedientes);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("ObtenerUsuariosAsignacionIntera/{idProceso}/{idFase}")]
        public async Task<IActionResult> ObtenerUsuariosAsignacionIntera(int idProceso, int idFase)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                var listaUsuarios = await _servicio.ObtenerUsuariosAsignacionIntera(idEntidad, idProceso, idFase, idUsuario);

                return Ok(listaUsuarios);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("AsignarExpediente")]
        public async Task<IActionResult> AsignarExpediente([FromBody] AsignacionModelo expediente)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }
                expediente.IdEntidad = idEntidad;
                expediente.IdUsuarioRegistro = idUsuario;

                var result = await _servicio.AsignarExpediente(expediente);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("AsignarMasivamenteExpedientes")]
        public async Task<IActionResult> AsignarMasivamenteExpedientes([FromBody] AsignacionModelo[] expedientes)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                foreach (var expediente in expedientes)
                {
                    expediente.IdEntidad = idEntidad;
                    expediente.IdUsuarioRegistro = idUsuario;
                }
                

                var result = await _servicio.AsignarMasivamenteExpedientes(expedientes);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("Anotaciones/ObtenerAnotaciones")]
        public async Task<IActionResult> ObtenerAnotacionesEnFasePorUsuarioAsync([FromBody] ExpedienteModelo expediente)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }
                expediente.IdEntidad = idEntidad;
                expediente.IdUsuarioAsignado = idUsuario;

                var listaAnotaciones = await _servicio.ObtenerAnotacionesEnFasePorUsuarioAsync(expediente);

                return Ok(listaAnotaciones);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("Anotaciones")]
        public async Task<IActionResult> CrearAnotacionAsync([FromBody] ExpedienteAnotacionesModelo anotacion)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }
                anotacion.IdEntidad = idEntidad;
                anotacion.IdUsuarioRegistro = idUsuario;

                var result = await _servicio.CrearAnotacionAsync(anotacion);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("Anotaciones/EliminarAnotacion")]
        public async Task<IActionResult> EliminarAnotacionAsync(ExpedienteAnotacionesModelo anotacion)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                anotacion.IdEntidad = idEntidad;

                var result = await _servicio.EliminarAnotacionAsync(anotacion);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("ConfirmaRecepcion")]
        public async Task<IActionResult> ConfirmarRecepcionExpedienteAsync([FromBody] AsignacionModelo expediente)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }
                expediente.IdEntidad = idEntidad;
                expediente.IdUsuarioAsignado = idUsuario;
                expediente.IdUsuarioRegistro = idUsuario;

                var result = await _servicio.ConfirmarRecepcionExpedienteAsync(expediente);

                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("ConfirmarMasivamenteRecepcionExpedientes")]
        public async Task<IActionResult> ConfirmarMasivamenteRecepcionExpedientesAsync([FromBody] AsignacionModelo[] expedientes)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                foreach (var expediente in expedientes)
                {
                    expediente.IdEntidad = idEntidad;
                    expediente.IdUsuarioAsignado = idUsuario;
                    expediente.IdUsuarioRegistro = idUsuario;
                }                

                var result = await _servicio.ConfirmarMasivamenteRecepcionExpedientesAsync(expedientes);

                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("RechazaRecepcion")]
        public async Task<IActionResult> RechazarRecepcionExpedienteAsync([FromBody] AsignacionModelo expediente)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }
                expediente.IdEntidad = idEntidad;
                expediente.IdUsuarioAsignado = idUsuario;
                expediente.IdUsuarioRegistro = idUsuario;

                var result = await _servicio.RechazarRecepcionExpedienteAsync(expediente);

                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("RechazarMasivamenteRecepcionExpedientes")]
        public async Task<IActionResult> RechazarMasivamenteRecepcionExpedientesAsync([FromBody] AsignacionModelo[] expedientes)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                foreach (var expediente in expedientes)
                {
                    expediente.IdEntidad = idEntidad;
                    expediente.IdUsuarioAsignado = idUsuario;
                    expediente.IdUsuarioRegistro = idUsuario;
                }                

                var result = await _servicio.RechazarMasivamenteRecepcionExpedientesAsync(expedientes);

                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("TomarExpediente")]
        public async Task<IActionResult> TomarExpedienteAsync([FromBody] AsignacionModelo expediente)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }
                expediente.IdEntidad = idEntidad;
                expediente.IdUsuarioAsignado = idUsuario;

                var result = await _servicio.TomarExpedienteAsync(expediente);

                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("PuedeOperarse/{idExpediente}/{idUsuario}")]
        public async Task<IActionResult> PuedeOperarse(int idExpediente)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                var asignacion = await _servicio.PuedeOperarse(idEntidad, idExpediente, idUsuario);

                return Ok(asignacion);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("VincularExpediente")]
        public async Task<IActionResult> VinculaExpediente([FromBody] VinculacionExpedienteModelo expedienteVinculado)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }
                expedienteVinculado.IdEntidad = idEntidad;
                expedienteVinculado.IdUsuarioRegistro = idUsuario;

                var result = await _servicio.VincularExpediente(expedienteVinculado);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("Vinculaciones/{idExpediente}/{idExpedienteVinculado}/{tipo}")]
        public async Task<IActionResult> EliminarVinculacionAsync(int idExpediente, int idExpedienteVinculado, int tipo)
        {
            try
            {
                Boolean result;

                if (tipo==1)
                    result = await _servicio.EliminarVinculacionAsync(idExpediente, idExpedienteVinculado);
                else
                    result = await _servicio.EliminarVinculacionAsync(idExpedienteVinculado, idExpediente);

                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("ObtenerVinculaciones/{idExpediente}")]
        public async Task<IActionResult> ObtenerExpedientesVinculadosAsync(int idExpediente)
        {
            try
            {

                var result = await _servicio.ObtenerExpedientesVinculadosAsync(idExpediente);

                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("ObtenerVinculacionesDisponibles/{idExpediente}")]
        public async Task<IActionResult> ObtenerExpedientesDisponiblesVincularAsync(int idExpediente)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                var result = await _servicio.ObtenerExpedientesDisponiblesVincularAsync(idUsuario, idExpediente);

                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("Vinculaciones/{idExpediente}")]
        public async Task<IActionResult> ObtenerExpedienteAVincularAsync(int idExpediente)
        {
            try
            {
                var result = await _servicio.ObtenerExpedienteAVincularAsync(idExpediente);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("CopiarExpediente")]
        public async Task<IActionResult> CopiarExpediente([FromBody] CopiaExpedienteModelo copiaExpediente)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }
                copiaExpediente.IdEntidad = idEntidad;
                copiaExpediente.IdUsuarioRegistro = idUsuario;

                var result = await _servicio.CopiarExpediente(copiaExpediente);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize]
        [HttpPost("UnificarExpedientes")]
        public async Task<IActionResult> UnificarExpedientes([FromBody] ExpedienteModelo[] expedientes)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                foreach (var expediente in expedientes)
                {
                    expediente.IdEntidad = idEntidad;
                    expediente.IdUsuarioRegistro = idUsuario;
                }                

                var result = await _servicio.UnificarExpedientes(expedientes);
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
