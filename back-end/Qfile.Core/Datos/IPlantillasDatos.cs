using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Datos
{
    public interface IPlantillasDatos
    {
        Task<List<PlantillaModelo>> ObtenerPlantillasAsync(int idEntidad, int idProceso);
        Task<int> ActualizarPlantillaAsync(PlantillaModelo plantilla);
        Task<int> CrearPlantillaAsync(PlantillaModelo plantilla, int idUsuario, DateTime fechaRegistro);
        Task<int> EliminarPlantillaAsync(int idEntidad, int idProceso, int idPlantilla);        
        Task<int> CrearSeccionAsync(SeccionModelo seccion, DateTime fechaRegistro, int usuarioRegistro);
        Task<int> ActualizarSeccionAsync(SeccionModelo seccion, DateTime fechaRegistro, int usuarioRegistro);
        Task<int> EliminarSeccionAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, DateTime fechaRegistro, int usuarioRegistro);
        Task<int> EliminarSeccionesAsync(int idEntidad, int idProceso, int idPlantilla, DateTime fechaRegistro, int usuarioRegistro);
        Task<bool> SeccionTieneCampos(int idEntidad, int idProceso, int idPlantilla, int idSeccion);
        Task<bool> ExistenSeccionesConCampos(int idEntidad, int idProceso, int idPlantilla);
        Task<List<TipoCampoModelo>> ObtenerTiposCampoAsync();
        Task<int> CrearCampoAsync(CampoModelo seccion, DateTime fechaRegistro, int usuarioRegistro);
        Task<int> ActualizarCampoAsync(CampoModelo seccion, DateTime fechaRegistro, int usuarioRegistro);
        Task<int> EliminarCampoAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, DateTime fechaRegistro, int usuarioRegistro);
        Task<int> EliminarCamposAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, DateTime fechaRegistro, int usuarioRegistro);
        Task<bool> CampoTieneValores(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo);
        Task<bool> ExistenCamposConValores(int idEntidad, int idProceso, int idPlantilla, int idSeccion);
        Task<List<PlantillaModelo>> ObtenerPlantillaActualAsync(int idEntidad, int idProceso, int idPlantilla);
        Task<int> PublicarVersionPropuestaAsync(HistoricoPlantillasModelo modelo);
        Task<int> CambiarOrdenAsync(SeccionModelo[] secciones, int idEntidad, int usuarioRegistro, DateTime fechaRegistro);
        Task<int> CambiarOrdenCamposAsync(CampoModelo[] campos, int idEntidad, int idUsuario, DateTime fechaRegistro);
        Task<bool> ExisteComoCampoPadreAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo);
        Task<bool> ExisteComoValorPadreAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idValor);
        Task <List<CampoModelo>> ObtenerListasAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo);
        Task<bool> ValidaAsigancionListaPadreAsync(CampoModelo campo);
        Task<List<ValorListaModelo>> ObtenerValoresListaAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idCampoPadre, int idValorPadre);
        Task<List<ValorListaModelo>> ObtenerValoresListaPlantillaActualAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idCampoPadre, int idValorPadre);
        Task<int> AgregarValorListaAsync(ValorListaModelo valor, DateTime fechaRegistro, int usuarioRegistro);
        Task<int> EliminarValorListaAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idValor, int usuarioRegistro, DateTime fechaRegistro);
        Task<int> EliminarValoresListaAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int usuarioRegistro, DateTime fechaRegistro);
        Task<int> CambiarOrdenValoresAsync(ValorListaModelo[] campos, int idEntidad, int idUsuario, DateTime fechaRegistro);
        Task<int> PredeterminarValorListaAsync(ValorListaModelo valores, DateTime fechaRegistro, int usuarioRegistro);
        Task<int> RevertirCambiosPlantillaAsync(HistoricoPlantillasModelo modelo);
    }
}
