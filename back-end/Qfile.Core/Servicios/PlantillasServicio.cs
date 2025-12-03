using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using Qfile.Core.Tipos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public class PlantillasServicio : IPlantillasServicio
    {
        private readonly IPlantillasDatos _datos;


        public PlantillasServicio(IPlantillasDatos datos)
        {
            _datos = datos;
        }
     
        public async Task<List<PlantillaModelo>> ObtenerPlantillasAsync(int idEntidad, int idProceso)
        {
            try
            {
                return await _datos.ObtenerPlantillasAsync(idEntidad, idProceso);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> CrearPlantillaAsync(PlantillaModelo plantilla, int idUsuario)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.CrearPlantillaAsync(plantilla, idUsuario, fechaRegistro);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }   
        }
        public async Task<int> ActualizarPlantillaAsync(PlantillaModelo plantilla)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.ActualizarPlantillaAsync(plantilla);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> EliminarPlantillaAsync(int idEntidad, int idProceso, int idPlantilla)
        {
            try
            {
                // PENDIETNE REALIZAR VALIDACIONEs

                return await _datos.EliminarPlantillaAsync(idEntidad, idProceso, idPlantilla);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> CrearSeccionAsync(SeccionModelo seccion, int idUsuario)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.CrearSeccionAsync(seccion, fechaRegistro, idUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> ActualizarSeccionAsync(SeccionModelo seccion, int idUsuario)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.ActualizarSeccionAsync(seccion, fechaRegistro, idUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> EliminarSeccionAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idUsuario)
        {
            try
            {
                // PENDIETNE REALIZAR VALIDACIONES
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.EliminarSeccionAsync(idEntidad, idProceso, idPlantilla, idSeccion, fechaRegistro, idUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> EliminarSeccionesAsync(int idEntidad, int idProceso, int idPlantilla, int idUsuario)
        {
            try
            {
                // PENDIETNE REALIZAR VALIDACIONES
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.EliminarSeccionesAsync(idEntidad, idProceso, idPlantilla, fechaRegistro, idUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> SeccionTieneCampos(int idEntidad, int idProceso, int idPlantilla, int idSeccion)
        {
            try
            {
                return await _datos.SeccionTieneCampos(idEntidad, idProceso, idPlantilla, idSeccion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> ExistenSeccionesConCampos(int idEntidad, int idProceso, int idPlantilla)
        {
            try
            {
                return await _datos.ExistenSeccionesConCampos(idEntidad, idProceso, idPlantilla);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<TipoCampoModelo>> ObtenerTiposCampoAsync()
        {
            try
            {
                return await _datos.ObtenerTiposCampoAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> CrearCampoAsync(CampoModelo campo, int idUsuario)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.CrearCampoAsync(campo, fechaRegistro, idUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> ActualizarCampoAsync(CampoModelo campo, int idUsuario)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;

                if (campo.IdCampoPadre != 0 && campo.IdTipoCampo == TiposCampo.Lista)
                {
                    if (await _datos.ValidaAsigancionListaPadreAsync(campo))
                        return await _datos.ActualizarCampoAsync(campo, fechaRegistro, idUsuario);
                }
                else
                {
                    return await _datos.ActualizarCampoAsync(campo, fechaRegistro, idUsuario);
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> EliminarCampoAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idUsuario)
        {
            try
            {
                // PENDIETNE REALIZAR VALIDACIONES
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.EliminarCampoAsync(idEntidad, idProceso, idPlantilla, idSeccion, idCampo, fechaRegistro, idUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> EliminarCamposAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idUsuario)
        {
            // PENDIETNE REALIZAR VALIDACIONES
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.EliminarCamposAsync(idEntidad, idProceso, idPlantilla, idSeccion, fechaRegistro, idUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<bool> CampoTieneValores(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo)
        {
            try
            {
                return await _datos.CampoTieneValores(idEntidad, idProceso, idPlantilla, idSeccion, idCampo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> ExistenCamposConValores(int idEntidad, int idProceso, int idPlantilla, int idSeccion)
        {
            try
            {
                return await _datos.ExistenCamposConValores(idEntidad, idProceso, idPlantilla, idSeccion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<PlantillaModelo>> ObtenerPlantillaActualAsync(int idEntidad, int idProceso, int idPlantilla)
        {
            try
            {
                return await _datos.ObtenerPlantillaActualAsync(idEntidad, idProceso, idPlantilla);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> PublicarVersionPropuestaAsync(HistoricoPlantillasModelo modelo)
        {
            try
            {
                modelo.fechaPublicacion = UtilidadesServicio.FechaActualUtc;

                return await _datos.PublicarVersionPropuestaAsync(modelo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> CambiarOrdenAsync(SeccionModelo[] secciones, int idEntidad, int idUsuario)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.CambiarOrdenAsync(secciones, idEntidad, idUsuario, fechaRegistro);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> CambiarOrdenCamposAsync(CampoModelo[] campos, int idEntidad, int idUsuario)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.CambiarOrdenCamposAsync(campos, idEntidad, idUsuario, fechaRegistro);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public async Task<bool> ExisteComoCampoPadreAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo)
        {
            try
            {
                return await _datos.ExisteComoCampoPadreAsync(idEntidad, idProceso, idPlantilla, idSeccion, idCampo);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> ExisteComoValorPadreAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idValor)
        {
            try
            {
                return await _datos.ExisteComoValorPadreAsync(idEntidad, idProceso, idPlantilla, idSeccion, idCampo, idValor);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<CampoModelo>> ObtenerListasAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo)
        {
            try
            {
                return await _datos.ObtenerListasAsync(idEntidad, idProceso, idPlantilla, idSeccion, idCampo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<List<ValorListaModelo>> ObtenerValoresListaAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idCampoPadre, int idValorPadre)
        {
            try
            {
                return await _datos.ObtenerValoresListaAsync(idEntidad, idProceso, idPlantilla, idSeccion, idCampo, idCampoPadre, idValorPadre);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<ValorListaModelo>> ObtenerValoresListaPlantillaActualAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idCampoPadre, int idValorPadre)
        {
            try
            {
                return await _datos.ObtenerValoresListaPlantillaActualAsync(idEntidad, idProceso, idPlantilla, idSeccion, idCampo, idCampoPadre, idValorPadre);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> AgregarValorListaAsync(ValorListaModelo valor, int usuarioRegistro)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;
                return await _datos.AgregarValorListaAsync(valor, fechaRegistro, usuarioRegistro);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }
        public async Task<int> EliminarValorListaAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idValor, int usuarioRegistro)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;
                return await _datos.EliminarValorListaAsync(idEntidad, idProceso, idPlantilla, idSeccion, idCampo, idValor, usuarioRegistro, fechaRegistro);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> EliminarValoresListaAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int usuarioRegistro)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;
                return await _datos.EliminarValoresListaAsync(idEntidad, idProceso, idPlantilla, idSeccion, idCampo, usuarioRegistro, fechaRegistro);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> CambiarOrdenValoresAsync(ValorListaModelo[] valores, int idEntidad, int idUsuario)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.CambiarOrdenValoresAsync(valores, idEntidad, idUsuario, fechaRegistro);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> PredeterminarValorListaAsync(ValorListaModelo valor, int usuarioRegistro)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;
                return await _datos.PredeterminarValorListaAsync(valor, fechaRegistro, usuarioRegistro);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> RevertirCambiosPlantillaAsync(HistoricoPlantillasModelo modelo)
        {
            try
            {
                modelo.fechaPublicacion = UtilidadesServicio.FechaActualUtc;

                return await _datos.RevertirCambiosPlantillaAsync(modelo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }
    }
}
