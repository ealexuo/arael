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
    public class PlantillasController : Controller
    {
        private readonly ApplicationSettingsModelo _appSetings;
        private readonly IPlantillasServicio _servicio;

        public PlantillasController(IOptions<ApplicationSettingsModelo> appSettings, IPlantillasServicio servicio)
        {
            _appSetings = appSettings.Value;
            _servicio = servicio;
        }

        #region Plantillas

        [Authorize]
        [HttpGet("ObtenerPlantillas/{idProceso}")]
        public async Task<IActionResult> ObtenerPlantillas(int idProceso)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerPlantillasAsync(idEntidad, idProceso);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("ObtenerPlantillaActual/{idProceso}/{idPlantilla}")]
        public async Task<IActionResult> ObtenerPlantillaActual(int idProceso, int idPlantilla)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerPlantillaActualAsync(idEntidad, idProceso, idPlantilla);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("Plantilla")]
        public async Task<IActionResult> CrearPlantilla([FromBody] PlantillaModelo plantilla)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;

                if (identity != null)
                {
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    plantilla.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.CrearPlantillaAsync(plantilla, idUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("Plantilla")]
        public async Task<IActionResult> ActualizarPlantilla([FromBody] PlantillaModelo plantilla)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    plantilla.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ActualizarPlantillaAsync(plantilla);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("Plantilla/{idProceso}/{idPlantilla}")]
        public async Task<IActionResult> EliminarPlantilla(int idProceso, int idPlantilla)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var idEntidad = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.EliminarPlantillaAsync(idEntidad, idProceso, idPlantilla);
                return Ok();
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }


        [Authorize]
        [HttpPut("PublicarPlantilla")]
        public async Task<IActionResult> PublicarVersionPropuesta([FromBody] HistoricoPlantillasModelo modelo)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    modelo.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.PublicarVersionPropuestaAsync(modelo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("Plantilla/Listas/{IdProceso}/{IdPlantilla}/{IdSeccion}/{IdCampo}")]
        public async Task<IActionResult> ObtenerListas(int IdProceso, int IdPlantilla, int IdSeccion, int IdCampo)
        {
            try
            {
                int IdEntidad = 0;

                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerListasAsync(IdEntidad, IdProceso, IdPlantilla, IdSeccion, IdCampo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("Plantilla/Lista/{IdProceso}/{IdPlantilla}/{IdSeccion}/{IdCampo}/{IdCampoPadre}/{IdValorPadre}")]
        public async Task<IActionResult> ObtenerValoresLista(int IdProceso, int IdPlantilla, int IdSeccion, int IdCampo, int idCampoPadre, int IdValorPadre)
        {
            try
            {
                int IdEntidad = 0;

                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerValoresListaAsync(IdEntidad, IdProceso, IdPlantilla, IdSeccion, IdCampo, idCampoPadre, IdValorPadre);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("PlantillaActual/Lista/{IdProceso}/{IdPlantilla}/{IdSeccion}/{IdCampo}/{IdCampoPadre}/{IdValorPadre}")]
        public async Task<IActionResult> ObtenerValoresListaPlantillaActual(int IdProceso, int IdPlantilla, int IdSeccion, int IdCampo, int idCampoPadre, int IdValorPadre)
        {
            try
            {
                int IdEntidad = 0;

                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ObtenerValoresListaPlantillaActualAsync(IdEntidad, IdProceso, IdPlantilla, IdSeccion, IdCampo, idCampoPadre, IdValorPadre);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("Plantilla/Lista")]
        public async Task<IActionResult> AgregarValorLista([FromBody] ValorListaModelo valor)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;

                if (identity != null)
                {
                    valor.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                var result = await _servicio.AgregarValorListaAsync(valor, idUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("Plantilla/Lista/{IdProceso}/{IdPlantilla}/{IdSeccion}/{IdCampo}/{IdValor}")]
        public async Task<IActionResult> EliminarValorLista(int IdProceso, int IdPlantilla, int IdSeccion, int IdCampo, int IdValor)
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

                var existeComoValorPadre = await _servicio.ExisteComoValorPadreAsync(idEntidad, IdProceso, IdPlantilla, IdSeccion, IdCampo, IdValor);
                if (existeComoValorPadre)
                {
                    return Conflict("VALOR_PADRE");
                }

                var result = await _servicio.EliminarValorListaAsync(idEntidad, IdProceso, IdPlantilla, IdSeccion, IdCampo, IdValor, idUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("Plantilla/Lista/{IdProceso}/{IdPlantilla}/{IdSeccion}/{IdCampo}")]
        public async Task<IActionResult> EliminarValoresLista(int IdProceso, int IdPlantilla, int IdSeccion, int IdCampo)
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

                var existeComoCampoPadre = await _servicio.ExisteComoCampoPadreAsync(idEntidad, IdProceso, IdPlantilla, IdSeccion, IdCampo);
                if(existeComoCampoPadre)
                {
                    return Conflict("CAMPO_PADRE");
                }

                var result = await _servicio.EliminarValoresListaAsync(idEntidad, IdProceso, IdPlantilla, IdSeccion, IdCampo, idUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("Plantilla/Lista/Valor/Orden")]
        public async Task<IActionResult> CambiarOrdenValores([FromBody] ValorListaModelo[] valores)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int IdEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("idUsuario").Value);
                }

                var result = await _servicio.CambiarOrdenValoresAsync(valores, IdEntidad, idUsuario);

                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("Plantilla/Lista")]
        public async Task<IActionResult> PrdeterminarValorLista([FromBody] ValorListaModelo valor)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;

                if (identity != null)
                {
                    valor.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                var result = await _servicio.PredeterminarValorListaAsync(valor, idUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("RevertirPlantilla")]
        public async Task<IActionResult> RevertirCambiosPlantilla([FromBody] HistoricoPlantillasModelo modelo)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    modelo.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.RevertirCambiosPlantillaAsync(modelo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region Secciones

        [Authorize]
        [HttpPost("Seccion")]
        public async Task<IActionResult> CrearSeccion([FromBody] SeccionModelo seccion)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;

                if (identity != null)
                {
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    seccion.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.CrearSeccionAsync(seccion, idUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("Seccion")]
        public async Task<IActionResult> ActualizarSeccion([FromBody] SeccionModelo seccion)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;

                if (identity != null)
                {
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    seccion.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.ActualizarSeccionAsync(seccion, idUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("Seccion/{idProceso}/{idPlantilla}/{IdSeccion}")]
        public async Task<IActionResult> EliminarSeccion(int idProceso, int idPlantilla, int idSeccion)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var seccionTieneCampos = await _servicio.SeccionTieneCampos(idEntidad, idProceso, idPlantilla, idSeccion);
                if (seccionTieneCampos)
                {
                    return Conflict("SECCION_CAMPOS");
                }

                var result = await _servicio.EliminarSeccionAsync(idEntidad, idProceso, idPlantilla, idSeccion, idUsuario);
                return Ok();
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("Seccion/{idProceso}/{idPlantilla}")]
        public async Task<IActionResult> EliminarSecciones(int idProceso, int idPlantilla)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                var existenSeccionesConCampos = await _servicio.ExistenSeccionesConCampos(idEntidad, idProceso, idPlantilla);
                if (existenSeccionesConCampos)
                {
                    return Conflict("SECCION_CAMPOS");
                }

                var result = await _servicio.EliminarSeccionesAsync(idEntidad, idProceso, idPlantilla, idUsuario);
                return Ok();
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("Seccion/Orden")]
        public async Task<IActionResult> CambiarOrden([FromBody] SeccionModelo[] secciones)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int IdEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("idUsuario").Value);
                }

                var result = await _servicio.CambiarOrdenAsync(secciones, IdEntidad, idUsuario);

                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region Campos

        [Authorize]
        [HttpGet("TiposCampo")]
        public async Task<IActionResult> ObtenerTiposCampos()
        {
            try
            {
                var result = await _servicio.ObtenerTiposCampoAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("Campo")]
        public async Task<IActionResult> CrearCampo([FromBody] CampoModelo campo)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;

                if (identity != null)
                {
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                    campo.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                }

                var result = await _servicio.CrearCampoAsync(campo, idUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("Campo")]
        public async Task<IActionResult> ActualizarCampo([FromBody] CampoModelo campo)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = 0;

                if (identity != null)
                {
                    campo.IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                var result = await _servicio.ActualizarCampoAsync(campo, idUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("Campo/{idProceso}/{idPlantilla}/{IdSeccion}/{IdCampo}")]
        public async Task<IActionResult> EliminarCampo(int idProceso, int idPlantilla, int idSeccion, int idCampo)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                var tieneValores = await _servicio.CampoTieneValores(idEntidad, idProceso, idPlantilla, idSeccion, idCampo);
                if (tieneValores)
                {
                    return Conflict("CAMPO_VALORES");
                }

                var result = await _servicio.EliminarCampoAsync(idEntidad, idProceso, idPlantilla, idSeccion, idCampo, idUsuario);
                return Ok();
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("Campo/{idProceso}/{idPlantilla}/{IdSeccion}")]
        public async Task<IActionResult> EliminarCampos(int idProceso, int idPlantilla, int idSeccion)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var idEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    idEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("IdUsuario").Value);
                }

                var existenCamposConValores = await _servicio.ExistenCamposConValores(idEntidad, idProceso, idPlantilla, idSeccion);
                if(existenCamposConValores)
                {
                    return Conflict("CAMPO_VALORES");
                }
                
                var result = await _servicio.EliminarCamposAsync(idEntidad, idProceso, idPlantilla, idSeccion, idUsuario);
                return Ok();
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("Campo/Orden")]
        public async Task<IActionResult> CambiarOrdenCampo([FromBody] CampoModelo[] campos)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int IdEntidad = 0;
                int idUsuario = 0;

                if (identity != null)
                {
                    IdEntidad = Int32.Parse(identity.FindFirst("IdEntidad").Value);
                    idUsuario = Int32.Parse(identity.FindFirst("idUsuario").Value);
                }

                var result = await _servicio.CambiarOrdenCamposAsync(campos, IdEntidad, idUsuario);

                return Ok(result);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("Campo/ExisteCampoPadre{idProceso}/{idPlantilla}/{IdSeccion}/{idCampo}")]
        public async Task<IActionResult> ExisteComoCampoPadreAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo)
        {
            try
            {
                var result = await _servicio.ExisteComoCampoPadreAsync(idEntidad, idProceso, idPlantilla, idSeccion, idCampo);
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