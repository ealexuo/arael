using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public interface IFasesTransicionesServicio
    {
        Task<List<FaseModelo>> ObtenerFasesAsync(int idEntidad, int idProceso);
        Task<List<TipoFaseModelo>> ObtenerTiposFasesAsync();
        Task<List<TipoAccesoModelo>> ObtenerTiposAccesosAsync();
        Task<List<UnidadMedidaModelo>> ObtenerUnidadesMedidaAsync();
        Task<int> CrearFaseAsync(FaseModelo fase);
        Task<int> EliminarFaseAsync(int idEntidad, int idProceso, int IdFase);
        Task<int> ActualizarFaseAsync(FaseModelo fase);
        Task<List<UsuarioFaseModelo>> ObtenerUsuariosPorFaseAsync(int idEntidad, int idProceso, int idFase);
        Task<int> CrearUsuarioFaseAsync(UsuarioFaseModelo usuario);
        Task<int> EliminarUsuarioFaseAsync(int idEntidad, int idProceso, int IdFase, int idUsuaio);
        Task<List<UsuarioListaModelo>> ObtenerUsuariosPorUAAsync(int idEntidad, int idProceso, int idFase, int idUnidadAdministrativa);
        Task<int> PermisoRecepcionAsync(UsuarioFaseModelo usuario);
        Task<List<TransicionesModelo>> ObtenerTransicionesPorFaseAsync(int idEntidad, int idProceso, int idFaseOrigen);
        Task<int> CrearTransicioneAsync(TransicionesModelo transicion);
        Task<int> EliminarTransicionAsync(int idEntidad, int idProceso, int IdFaseOrigen, int idFaseDestino);
        Task<List<FaseModelo>> ObtenerFasesPendientesAsync(int idEntidad, int idProceso, int IdFaseOrigen);
        Task<int> ActivarTransicionAsync(TransicionesModelo transicion);
        Task<List<TransicionUsuarioModelo>> ObtenerUsuariosPorTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino);
        Task<int> CrearUsuarioTransicionAsync(TransicionUsuarioModelo usuario);
        Task<int> EliminarUsuarioTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino, int idUsuario);
        Task<List<UsuarioFaseModelo>> ObtenerUsuariosTransicionPendientesAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino);
        Task<int> CrearNotificacionTransicionAsync(TransicionNotificacionModelo notificacion);
        Task<int> EliminarNotificacionTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino, string correo);
        Task<List<TransicionNotificacionModelo>> ObtenerNotificacionesPorTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino);
        Task<List<RequisitoPorTransicionModelo>> ObtenerRequisitosPorTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino);
        Task<int> CrearRequisitoPorTransicionAsync(RequisitoPorTransicionModelo requisito);
        Task<int> EliminarRequisitoPorTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino, int idRequisito);
        Task<int> ActualizarRequisitoPorTransicionAsync(RequisitoPorTransicionModelo requisito);
    }
}
