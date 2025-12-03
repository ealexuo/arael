using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public interface IExpedienteServicio
    {
        Task<List<ExpedienteListaModelo>> ObtenerExpedientesAsync(int idEntidad, int idUsuario, int pagina, int cantidad, string buscarTexto);
        Task<int> GuardarExpedienteAsync(ExpedienteModelo expediente, int idUsuarioRegistro);
        Task<ExpedienteModelo> ObtenerExpedienteAsync(int idExpediente, int idUsuario, int idEntidad);
        Task<List<ExpedienteSeccionDatosModelo>> ObtenerExpedienteDatosAsync(int idExpediente);
        Task<int> GuardarExpedienteDatosAsync(List<ExpedienteSeccionDatosModelo> datosExpediente);
        Task<List<ExpedienteRequisitosModelo>> ListaRequisitosGestion(int idExpediente);
        Task<int> GuardarRequisitoAsync(ExpedienteRequisitosModelo requisito);
        Task<byte[]> ImprimirRequisitosGestion(int idExpediente, List<ExpedienteRequisitosModelo> requisitos);
        Task<byte[]> ImprimirCedula(int idExpediente, CedulaExpedienteModelo cedulaExpedienten, int idUsuario, int idEntidad);
        Task<List<ExpedienteTraslados>> ObtenerTraslados(int idExpediente);
        Task<List<ExpedienteAsignaciones>> ObtenerAsignaciones(int idEntidad, int idProceso, int idExpediente, DateTime fechaTraslado);
        Task<List<ExpedienteAnotacionesModelo>> ObtenerAnotaciones(int idEntidad, int idProceso, int idExpediente, DateTime fechaTraslado, DateTime? fechaAsignacion);
        Task<ExpedienteAsignacionInternaModelo> ObtenerAsignacionInterna(int idEntidad, int idProceso, int idExpediente);
        Task<List<FaseTrasladoModelo>> ObtenerFasesUsuariosTraslado(int idExpediente);
        Task<bool> TrasladarExpediente(TrasladoModelo expediente);
        Task<bool> TrasladarMasivamenteExpedientes(TrasladoModelo[] expediente);
        Task<List<UsuarioFase>> ObtenerUsuariosAsignacionIntera(int idEntidad, int idProceso, int idFase, int idUsuario);
        Task<bool> AsignarExpediente(AsignacionModelo expediente);
        Task<bool> AsignarMasivamenteExpedientes(AsignacionModelo[] expediente);
        Task<List<ExpedienteAnotacionesModelo>> ObtenerAnotacionesEnFasePorUsuarioAsync(ExpedienteModelo expediente);
        Task<bool> CrearAnotacionAsync(ExpedienteAnotacionesModelo anotacion);
        Task<bool> EliminarAnotacionAsync(ExpedienteAnotacionesModelo anotacion);
        Task<bool> ConfirmarRecepcionExpedienteAsync(AsignacionModelo expediente);
        Task<bool> ConfirmarMasivamenteRecepcionExpedientesAsync(AsignacionModelo[] expedientes);
        Task<bool> RechazarRecepcionExpedienteAsync(AsignacionModelo expediente);
        Task<bool> RechazarMasivamenteRecepcionExpedientesAsync(AsignacionModelo[] expedientes);
        Task<bool> TomarExpedienteAsync(AsignacionModelo expediente);
        Task<bool> PuedeOperarse(int idEntidad, int idExpediente, int idUsuario);
        Task<bool> VincularExpediente(VinculacionExpedienteModelo expedienteVinculado);
        Task<bool> EliminarVinculacionAsync(int idExpediente, int idExpedienteVinculado);
        Task<List<ListadoExpedientesVinculadosModelo>> ObtenerExpedientesVinculadosAsync(int idExpediente);
        Task<List<ListadoExpedientesVinculadosModelo>> ObtenerExpedientesDisponiblesVincularAsync(int UsuarioAsignado, int idExpediente);
        Task<ExpedienteModelo> ObtenerExpedienteAVincularAsync(int idExpediente);
        Task<bool> CopiarExpediente(CopiaExpedienteModelo expedienteVinculado);
        Task<bool> UnificarExpedientes(ExpedienteModelo[] expedientes);
    }
}
