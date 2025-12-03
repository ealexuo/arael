using Qfile.Core.Constantes;
using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using Qfile.Core.Tipos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public class FasesTransicionesServicio : IFasesTransicionesServicio
    {
        private readonly IFasesTransicionesDatos _datos;

        public FasesTransicionesServicio(IFasesTransicionesDatos datos)
        {
            _datos = datos;
        }

        public async Task<List<FaseModelo>> ObtenerFasesAsync(int idEntidad, int idProceso)
        {
            try
            {
                return await _datos.ObtenerFasesAsync(idEntidad, idProceso);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
 
        public async Task<List<TipoFaseModelo>> ObtenerTiposFasesAsync()
        {
            try
            {
                return await _datos.ObtenerTiposFasesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<List<TipoAccesoModelo>> ObtenerTiposAccesosAsync()
        {
            try
            {
                return await _datos.ObtenerTiposAccesosAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<List<UnidadMedidaModelo>> ObtenerUnidadesMedidaAsync()
        {
            try
            {
                return await _datos.ObtenerUnidadesMedidaAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> CrearFaseAsync(FaseModelo fase) 
        {
            try
            {
              
                if (fase.IdTipoFase == TipoFase.Inical && await _datos.ExisteFaseInicial(fase.IdEntidad, fase.IdProceso))
                {
                    throw new Exception("Solo puede existir una fase inicial por proceso");
                }

                fase.FechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.CrearFaseAsync(fase);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> EliminarFaseAsync(int idEntidad, int idProceso, int IdFase)
        {
            try
            {
                if (await _datos.ExistenExpedientesAsigandosALaFase(idEntidad, idProceso, IdFase))
                {
                    throw new Exception("No es posible eliminar la fase debido a que existen Expedientes asignados a la misma");
                }

                return await _datos.EliminarFaseAsync(idEntidad, idProceso, IdFase);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> ActualizarFaseAsync(FaseModelo fase)
        {
            try
            {
                fase.FechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.ActualizarFaseAsync(fase);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #region Fase Usuarios
        public async Task<List<UsuarioFaseModelo>> ObtenerUsuariosPorFaseAsync(int idEntidad, int idProceso, int idFase)
        {
            try
            {
                return await _datos.ObtenerUsuariosPorFaseAsync(idEntidad, idProceso, idFase);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> CrearUsuarioFaseAsync(UsuarioFaseModelo usuario)
        {
            try
            {
                usuario.FechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.CrearUsuarioFaseAsync(usuario);
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> EliminarUsuarioFaseAsync(int idEntidad, int idProceso, int IdFase, int idUsuario)
        {
            try
            {
                return await _datos.EliminarUsuarioFaseAsync(idEntidad, idProceso, IdFase, idUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<List<UsuarioListaModelo>> ObtenerUsuariosPorUAAsync(int idEntidad, int idProceso, int idFase, int idUnidadAdministrativa)
        {
            try
            {
                return await _datos.ObtenerUsuariosPorUAAsync(idEntidad, idProceso, idFase, idUnidadAdministrativa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }
        public async Task<int> PermisoRecepcionAsync(UsuarioFaseModelo usuario)
        {
            try
            {
                return await _datos.PermisoRecepcionAsync(usuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region Transiciones
        public async Task<List<TransicionesModelo>> ObtenerTransicionesPorFaseAsync(int idEntidad, int idProceso, int idFaseOrigen)
        {
            try
            {
                return await _datos.ObtenerTransicionesPorFaseAsync(idEntidad, idProceso, idFaseOrigen);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> CrearTransicioneAsync(TransicionesModelo transicion)
        {
            try
            {
                transicion.FechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.CrearTransicioneAsync(transicion);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> EliminarTransicionAsync(int idEntidad, int idProceso, int IdFaseOrigen, int idFaseDestino)
        {
            try
            {
                return await _datos.EliminarTransicionAsync(idEntidad, idProceso, IdFaseOrigen, idFaseDestino);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<List<FaseModelo>> ObtenerFasesPendientesAsync(int idEntidad, int idProceso, int IdFaseOrigen)
        {
            try
            {
                return await _datos.ObtenerFasesPendientesAsync(idEntidad, idProceso, IdFaseOrigen);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }
        public async Task<int> ActivarTransicionAsync(TransicionesModelo transicion)
        {
            try
            {
                return await _datos.ActivarTransicionAsync(transicion);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region Fase Usuarios
        public async Task<List<TransicionUsuarioModelo>> ObtenerUsuariosPorTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino)
        {
            try
            {
                return await _datos.ObtenerUsuariosPorTransicionAsync(idEntidad, idProceso, idFaseOrigen, idFaseDestino);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> CrearUsuarioTransicionAsync(TransicionUsuarioModelo usuario)
        {
            try
            {
                usuario.FechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.CrearUsuarioTransicionAsync(usuario);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> EliminarUsuarioTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen,int idFaseDestino, int idUsuario)
        {
            try
            {
                return await _datos.EliminarUsuarioTransicionAsync(idEntidad, idProceso, idFaseOrigen, idFaseDestino, idUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<List<UsuarioFaseModelo>> ObtenerUsuariosTransicionPendientesAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino)
        {
            try
            {
                return await _datos.ObtenerUsuariosTransicionPendientesAsync(idEntidad, idProceso, idFaseOrigen, idFaseDestino);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }
        #endregion

        #region Transicion Notificaciones

        public async Task<int> CrearNotificacionTransicionAsync(TransicionNotificacionModelo notificacion)
        {
            try
            {
                notificacion.FechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.CrearNotificacionTransicionAsync(notificacion);
                }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion

        #region Requisito por Transición

        public async Task<List<RequisitoPorTransicionModelo>> ObtenerRequisitosPorTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino)
        {
            try
            {
                return await _datos.ObtenerRequisitosPorTransicionAsync(idEntidad, idProceso, idFaseOrigen, idFaseDestino);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> CrearRequisitoPorTransicionAsync(RequisitoPorTransicionModelo requisito)
        {
            try
            {
                requisito.FechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.CrearRequisitoPorTransicionAsync(requisito);
                }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
     
        public async Task<int> EliminarNotificacionTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino, string correo)
        {
            try
            {
                return await _datos.EliminarNotificacionTransicionAsync(idEntidad, idProceso, idFaseOrigen, idFaseDestino, correo);
                }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> EliminarRequisitoPorTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino, int idRequisito)
        {
            try
            {
                return await _datos.EliminarRequisitoPorTransicionAsync(idEntidad, idProceso, idFaseOrigen, idFaseDestino, idRequisito);
                }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
           
        public async Task<List<TransicionNotificacionModelo>> ObtenerNotificacionesPorTransicionAsync(int idEntidad, int idProceso, int idFaseOrigen, int idFaseDestino)
        {
            try
            {
                return await _datos.ObtenerNotificacionesPorTransicionAsync(idEntidad, idProceso, idFaseOrigen, idFaseDestino);
                }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> ActualizarRequisitoPorTransicionAsync(RequisitoPorTransicionModelo requisito)
        {
            try
            {
                requisito.FechaRegistro = UtilidadesServicio.FechaActualUtc;
                return await _datos.ActualizarRequisitoPorTransicionAsync(requisito);
                }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
            
        #endregion
    }
}
