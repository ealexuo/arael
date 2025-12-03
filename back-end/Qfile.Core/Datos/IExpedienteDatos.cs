using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Qfile.Core.Datos
{
    public interface IExpedienteDatos
    {
        Task<List<ExpedienteListaModelo>> ObtenerExpedientesAsync(int idEntidad, int idUsuario, int pagina, int cantidad, string buscarTexto);
        Task<int> CrearExpedienteAsync(ExpedienteModelo expediente, int idUsuarioRegistro, DateTime fechaRegistro);
        Task<ExpedienteModelo> ObtenerExpedienteAsync(int idExpediente);
        Task<List<ExpedienteSeccionDatosModelo>> ObtenerExpedienteDatosAsync(int idExpediente);
        Task<int> GuardarExpedienteDatosAsync(List<ExpedienteSeccionDatosModelo> datosExpediente);
        Task<int> ActualizarExpedienteAsync(ExpedienteModelo expediente);
        Task<List<ExpedienteRequisitosModelo>> ListaRequisitosGestion(int idExpediente);
        Task<int> GuardarRequisitoAsync(ExpedienteRequisitosModelo requisito);
        Task<List<ExpedienteTraslados>> ObtenerTraslados(int idExpediente);
        Task<List<ExpedienteAsignaciones>> ObtenerAsignaciones(int idEntidad, int idProceso, int idExpediente, DateTime fechaTraslado);
        Task<List<ExpedienteAnotacionesModelo>> ObtenerAnotaciones(int idEntidad, int idProceso, int idExpediente, DateTime fechaTraslado, DateTime? fechaAsignacion);
        Task<ExpedienteAsignacionInternaModelo> ObtenerAsignacionInterna(int idEntidad, int idProceso, int idExpediente);
        Task<List<FaseTrasladoModelo>> ObtenerFasesUsuariosTraslado(int idExpediente);
        Task<bool> TrasladarExpediente(TrasladoModelo expediente);
        Task<bool> TrasladarMasivamenteExpedientes(TrasladoModelo[] expediente);
        Task<List<UsuarioFase>> ObtenerUsuariosAsignacionIntera(int idEntidad, int idProceso, int idFase, int idUsuario);
        Task<bool> AsignarExpediente(AsignacionModelo expediente);
        Task<bool> AsignarMasivamenteExpedientes(AsignacionModelo[] expedientes);
        Task<List<ExpedienteAnotacionesModelo>> ObtenerAnotacionesEnFasePorUsuarioAsync(ExpedienteModelo expediente);
        Task<bool> CrearAnotacionAsync(ExpedienteAnotacionesModelo anotacion);
        Task<bool> EliminarAnotacionAsync(ExpedienteAnotacionesModelo anotacion);
        Task<bool> ConfirmarRecepcionExpedienteAsync(AsignacionModelo expediente);
        Task<bool> ConfirmarMasivamenteRecepcionExpedientesAsync(AsignacionModelo[] expedientes);
        Task<bool> RechazarRecepcionExpedienteAsync(AsignacionModelo expediente);
        Task<bool> RechazarMasivamenteRecepcionExpedientesAsync(AsignacionModelo[] expedientes);
        Task<bool> TomarExpedienteAsync(AsignacionModelo expediente);
        Task<bool> PuedeOperarse(int idEntidad, int idExpediente, int idUsuario);
        Task<bool> ExpedienteAsignadoAUsuario(int idEntidad, int idExpediente, int idUsuario);
        Task<bool> VincularExpediente(VinculacionExpedienteModelo expedienteVinculado);
        Task<bool> EliminarVinculacionAsync(int idExpediente, int idExpedienteVinculado);
        Task<List<ListadoExpedientesVinculadosModelo>> ObtenerExpedientesVinculadosAsync(int idExpediente);
        Task<List<ListadoExpedientesVinculadosModelo>> ObtenerExpedientesDisponiblesVincularAsync(int UsuarioAsignado, int idExpediente);
        Task<ExpedienteModelo> ObtenerExpedienteAVincularAsync(int idExpediente);
        Task<bool> CopiarExpediente(CopiaExpedienteModelo copiaExpediente, DateTime Ejercicio);
        Task<bool> UnificarExpedientes(ExpedienteModelo[] expedientes, DateTime Ejercicio);
    }
}
