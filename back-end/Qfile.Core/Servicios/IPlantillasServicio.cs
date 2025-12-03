using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public interface IPlantillasServicio
    {
        Task<List<PlantillaModelo>> ObtenerPlantillasAsync(int idEntidad, int idProceso);
        Task<int> CrearPlantillaAsync(PlantillaModelo plantilla, int idUsuario);
        Task<int> ActualizarPlantillaAsync(PlantillaModelo plantilla);
        Task<int> EliminarPlantillaAsync(int idEntidad, int idProceso, int idPlantilla);
        Task<int> CrearSeccionAsync(SeccionModelo proceso, int idUsuario);
        Task<int> ActualizarSeccionAsync(SeccionModelo seccion, int idUsuario);
        Task<int> EliminarSeccionAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idUsuario);
        Task<int> EliminarSeccionesAsync(int idEntidad, int idProceso, int idPlantilla, int idUsuario);
        Task<bool> SeccionTieneCampos(int idEntidad, int idProceso, int idPlantilla, int idSeccion);
        Task<bool> ExistenSeccionesConCampos(int idEntidad, int idProceso, int idPlantilla);
        Task<List<TipoCampoModelo>> ObtenerTiposCampoAsync();
        Task<int> CrearCampoAsync(CampoModelo proceso, int idUsuario);
        Task<int> ActualizarCampoAsync(CampoModelo seccion, int idUsuario);
        Task<int> EliminarCampoAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idUsuario);
        Task<int> EliminarCamposAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idUsuario);
        Task<bool> CampoTieneValores(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo);
        Task<bool> ExistenCamposConValores(int idEntidad, int idProceso, int idPlantilla, int idSeccion);
        Task<List<PlantillaModelo>> ObtenerPlantillaActualAsync(int idEntidad, int idProceso, int idPlantilla);
        Task<int> PublicarVersionPropuestaAsync(HistoricoPlantillasModelo modelo);
        Task<int> CambiarOrdenAsync(SeccionModelo[] secciones, int idEntidad, int idUsuario);
        Task<int> CambiarOrdenCamposAsync(CampoModelo[] campos, int idEntidad, int idUsuario);
        Task<bool> ExisteComoCampoPadreAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo);
        Task<bool> ExisteComoValorPadreAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idValor);
        Task<List<CampoModelo>> ObtenerListasAsync(int IdEntidad, int IdProceso, int IdPlantilla, int IdSeccion, int idCampo);
        Task<List<ValorListaModelo>> ObtenerValoresListaAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idCampoPadre, int idValorPadre);
        Task<List<ValorListaModelo>> ObtenerValoresListaPlantillaActualAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idCampoPadre, int idValorPadre);
        Task<int> AgregarValorListaAsync(ValorListaModelo valor, int usuarioRegistro);
        Task<int> EliminarValorListaAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int idValor, int usuarioRegistro);
        Task<int> EliminarValoresListaAsync(int idEntidad, int idProceso, int idPlantilla, int idSeccion, int idCampo, int usuarioRegistro);
        Task<int> CambiarOrdenValoresAsync(ValorListaModelo[] valores, int idEntidad, int idUsuario);
        Task<int> PredeterminarValorListaAsync(ValorListaModelo valor, int usuarioRegistro);
        Task<int> RevertirCambiosPlantillaAsync(HistoricoPlantillasModelo modelo);
    }
}
