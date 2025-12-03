using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Qfile.Core.Datos;
using Qfile.Core.Constantes;

namespace Qfile.Core.Servicios
{
    public class ExpedienteServicio : IExpedienteServicio
    {
        private readonly IExpedienteDatos _datos;
        private readonly IFasesTransicionesDatos _datosFase;
        private readonly IReportesServicio _servicioReporte;
        private readonly IUsuarioServicio _servicioUsuario;
        private readonly ICorreoElectronicoServicio _servicioCorreoElectronico;
        private readonly IProcesoPermisoServicio _servicioProcesoPermiso;

        public ExpedienteServicio(
            IExpedienteDatos datos,
            IFasesTransicionesDatos datosFase,
            IReportesServicio servicioReporte,
            IUsuarioServicio servicioUsuario,
            ICorreoElectronicoServicio servicioCorreoElectronico,
            IProcesoPermisoServicio servicioProcesoPermiso)
        {
            _datos = datos;
            _datosFase = datosFase;
            _servicioReporte = servicioReporte;
            _servicioUsuario = servicioUsuario;
            _servicioCorreoElectronico = servicioCorreoElectronico;
            _servicioProcesoPermiso = servicioProcesoPermiso;
        }

        public async Task<List<ExpedienteListaModelo>> ObtenerExpedientesAsync(int idEntidad, int idUsuario, int pagina, int cantidad, string buscarTexto)
        {
            return await _datos.ObtenerExpedientesAsync(idEntidad, idUsuario, pagina, cantidad, buscarTexto);
        }
        public async Task<int> GuardarExpedienteAsync(ExpedienteModelo expediente, int idUsuarioRegistro)
        {
            if (expediente == null || expediente.IdExpediente == 0)
            {
                return await _datos.CrearExpedienteAsync(expediente, idUsuarioRegistro, UtilidadesServicio.FechaActualUtc);
            }
            else
            {
                return await _datos.ActualizarExpedienteAsync(expediente);
            }
        }

        public async Task<ExpedienteModelo> ObtenerExpedienteAsync(int idExpediente, int idUsuario, int idEntidad)
        {
            var expediente = await _datos.ObtenerExpedienteAsync(idExpediente);
            expediente.PlantillaHabilitada = await _servicioProcesoPermiso.UsuarioTienePermiso("Edición de campos de plantilla", idUsuario, idEntidad, expediente.IdProceso);

            return expediente;
        }

        public async Task<List<ExpedienteSeccionDatosModelo>> ObtenerExpedienteDatosAsync(int idExpediente)
        {
            return await _datos.ObtenerExpedienteDatosAsync(idExpediente);
        }
        public async Task<int> GuardarExpedienteDatosAsync(List<ExpedienteSeccionDatosModelo> datosExpediente)
        {

            foreach (var seccion in datosExpediente)
            {
                foreach (var campo in seccion.ListaCampos)
                {
                    if (campo.Obligatorio && (campo.Valor == null || campo.Valor == ""))
                    {
                        throw new Exception("Todos los campos obligatorios son requeridos.");
                    }
                };
            };

            return await _datos.GuardarExpedienteDatosAsync(datosExpediente);
        }
        public async Task<int> ActualizarExpedienteAsync(ExpedienteModelo expediente)
        {
            return await _datos.ActualizarExpedienteAsync(expediente);
        }
        public async Task<List<ExpedienteRequisitosModelo>> ListaRequisitosGestion(int idExpediente)
        {
            return await _datos.ListaRequisitosGestion(idExpediente);
        }
        public async Task<int> GuardarRequisitoAsync(ExpedienteRequisitosModelo requisito)
        {
            requisito.FechaRegistro = DateTime.Now;
            return await _datos.GuardarRequisitoAsync(requisito);
        }
        public async Task<byte[]> ImprimirRequisitosGestion(int idExpediente, List<ExpedienteRequisitosModelo> requisitos)
        {

            return await _servicioReporte.ImprimirRequisitosGestion(idExpediente, requisitos);
        }
        public async Task<byte[]> ImprimirCedula(int idExpediente, CedulaExpedienteModelo cedulaExpediente, int idUsuario, int idEntidad)
        {
            return await _servicioReporte.ImprimirCedula(idExpediente, cedulaExpediente, idUsuario, idEntidad);
        }
        public async Task<List<ExpedienteTraslados>> ObtenerTraslados(int idExpediente)
        {
            return await _datos.ObtenerTraslados(idExpediente);
        }
        public async Task<List<ExpedienteAsignaciones>> ObtenerAsignaciones(int idEntidad, int idProceso, int idExpediente, DateTime fechaTraslado)
        {
            return await _datos.ObtenerAsignaciones(idEntidad, idProceso, idExpediente, fechaTraslado);
        }
        public async Task<List<ExpedienteAnotacionesModelo>> ObtenerAnotaciones(int idEntidad, int idProceso, int idExpediente, DateTime fechaTraslado, DateTime? fechaAsignacion)
        {
            return await _datos.ObtenerAnotaciones(idEntidad, idProceso, idExpediente, fechaTraslado, fechaAsignacion);
        }
        public async Task<ExpedienteAsignacionInternaModelo> ObtenerAsignacionInterna(int idEntidad, int idProceso, int idExpediente)
        {
            return await _datos.ObtenerAsignacionInterna(idEntidad, idProceso, idExpediente);
        }
        public async Task<List<FaseTrasladoModelo>> ObtenerFasesUsuariosTraslado(int idExpediente)
        {
            return await _datos.ObtenerFasesUsuariosTraslado(idExpediente);
        }
        public async Task<bool> TrasladarExpediente(TrasladoModelo expediente)
        {

            expediente.FechaTraslado = UtilidadesServicio.FechaActualUtc;

            FaseModelo fase = await _datosFase.ObtenerFaseAsync(expediente.IdEntidad, expediente.IdProceso, expediente.IdFaseDestino);

            if (expediente.IdUsuarioAsignado == -1)
                expediente.IdTipoOperacion = TipoOperacion.TrasnaladoAFaseSinAsignacion;
            else
            {
                if (fase != null && fase.AcuseRecibido)
                    expediente.IdTipoOperacion = TipoOperacion.TrasladoPendienteDeRecibir;
                else
                    expediente.IdTipoOperacion = TipoOperacion.TrasladoConAsignacionObligatoria;
            }

            bool resultado = await _datos.TrasladarExpediente(expediente);

            if (resultado && expediente.IdUsuarioAsignado != -1)
            {
                var usuarioAsignado = await _servicioUsuario.ObtenerPorIdAsync(expediente.IdUsuarioAsignado);
                await _servicioCorreoElectronico.Enviar(
                    usuarioAsignado.CorreoElectronico,
                    "Asignación de Expediente " + expediente.IdExpediente,
                    "Se ha asignado el expediente: " + expediente.IdExpediente
                );
            }

            return resultado;
        }
        public async Task<bool> TrasladarMasivamenteExpedientes(TrasladoModelo[] expedientes)
        {
            int verificacion = 0;

            foreach (var expediente in expedientes)
            {
                expediente.FechaTraslado = UtilidadesServicio.FechaActualUtc;

                FaseModelo fase = await _datosFase.ObtenerFaseAsync(expediente.IdEntidad, expediente.IdProceso, expediente.IdFaseDestino);

                if (expediente.IdUsuarioAsignado == -1)
                    expediente.IdTipoOperacion = TipoOperacion.TrasnaladoAFaseSinAsignacion;
                else
                {
                    if (fase != null && fase.AcuseRecibido)
                        expediente.IdTipoOperacion = TipoOperacion.TrasladoPendienteDeRecibir;
                    else
                        expediente.IdTipoOperacion = TipoOperacion.TrasladoConAsignacionObligatoria;
                }

                if (!await PuedeOperarse(expediente.IdEntidad, expediente.IdExpediente, expediente.IdUsuarioRegistro))
                    verificacion++;
            }

            if (verificacion > 0)
            {
                return false;
            }
            else
            {
                bool resultado = await _datos.TrasladarMasivamenteExpedientes(expedientes);

                if (resultado)
                {
                    foreach (var expediente in expedientes)
                    {
                        var usuarioAsignado = await _servicioUsuario.ObtenerPorIdAsync(expediente.IdUsuarioAsignado);
                        await _servicioCorreoElectronico.Enviar(
                            usuarioAsignado.CorreoElectronico,
                            "Asignación de Expediente " + expediente.IdExpediente,
                            "Se ha asignado el expediente: " + expediente.IdExpediente
                        );
                    }

                }

                return resultado;
            }
        }
        public async Task<List<UsuarioFase>> ObtenerUsuariosAsignacionIntera(int idEntidad, int idProceso, int idFase, int idUsuario)
        {
            return await _datos.ObtenerUsuariosAsignacionIntera(idEntidad, idProceso, idFase, idUsuario);
        }
        public async Task<bool> AsignarExpediente(AsignacionModelo expediente)
        {

            expediente.FechaAsignacion = UtilidadesServicio.FechaActualUtc;

            expediente.IdTipoOperacion = TipoOperacion.AsignacionInterna;

            bool resultado = await _datos.AsignarExpediente(expediente);

            if (resultado)
            {
                var usuarioAsignado = await _servicioUsuario.ObtenerPorIdAsync(expediente.IdUsuarioAsignado);
                await _servicioCorreoElectronico.Enviar(
                    usuarioAsignado.CorreoElectronico,
                    "Asignación de Expediente " + expediente.IdExpediente,
                    "Se ha asignado el expediente: " + expediente.IdExpediente
                );
            }

            return resultado;

        }
        public async Task<bool> AsignarMasivamenteExpedientes(AsignacionModelo[] expedientes)
        {
            int verificacion = 0;

            foreach (var expediente in expedientes)
            {
                expediente.FechaAsignacion = UtilidadesServicio.FechaActualUtc;

                expediente.IdTipoOperacion = TipoOperacion.AsignacionInterna;

                if (!await PuedeOperarse(expediente.IdEntidad, expediente.IdExpediente, expediente.IdUsuarioRegistro))
                    verificacion++;
            }


            if (verificacion > 0)
            {
                return false;
            }
            else
            {
                bool resultado = await _datos.AsignarMasivamenteExpedientes(expedientes);

                if (resultado)
                {
                    foreach (var expediente in expedientes)
                    {
                        var usuarioAsignado = await _servicioUsuario.ObtenerPorIdAsync(expediente.IdUsuarioAsignado);
                        await _servicioCorreoElectronico.Enviar(
                            usuarioAsignado.CorreoElectronico,
                            "Asignación de Expediente " + expediente.IdExpediente,
                            "Se ha asignado el expediente: " + expediente.IdExpediente
                        );
                    }

                }

                return resultado;
            }
        }
        public async Task<List<ExpedienteAnotacionesModelo>> ObtenerAnotacionesEnFasePorUsuarioAsync(ExpedienteModelo expediente)
        {

            var resp = await _datos.ObtenerAnotacionesEnFasePorUsuarioAsync(expediente);

            //foreach (var item in resp)
            //{
            //    item.FechaRegistro  = UtilidadesServicio.FechaActualUtc()
            //}

            return resp;

        }
        public async Task<bool> CrearAnotacionAsync(ExpedienteAnotacionesModelo anotacion)
        {

            anotacion.FechaRegistro = UtilidadesServicio.FechaActualUtc;

            return await _datos.CrearAnotacionAsync(anotacion);
        }
        public async Task<bool> EliminarAnotacionAsync(ExpedienteAnotacionesModelo anotacion)
        {

            return await _datos.EliminarAnotacionAsync(anotacion);
        }
        public async Task<bool> ConfirmarRecepcionExpedienteAsync(AsignacionModelo expediente)
        {
            expediente.IdTipoOperacion = TipoOperacion.Recibido;
            expediente.FechaOperacion = UtilidadesServicio.FechaActualUtc;

            return await _datos.ConfirmarRecepcionExpedienteAsync(expediente);
        }
        public async Task<bool> ConfirmarMasivamenteRecepcionExpedientesAsync(AsignacionModelo[] expedientes)
        {
            int verificacion = 0;

            foreach (var expediente in expedientes)
            {
                expediente.IdTipoOperacion = TipoOperacion.Recibido;
                expediente.FechaOperacion = UtilidadesServicio.FechaActualUtc;

                if (!await ExpedienteAsignadoAUsuario(expediente.IdEntidad, expediente.IdExpediente, expediente.IdUsuarioRegistro))
                    verificacion++;
            }

            if (verificacion > 0)
                return false;
            else
                return await _datos.ConfirmarMasivamenteRecepcionExpedientesAsync(expedientes);

        }
        public async Task<bool> RechazarRecepcionExpedienteAsync(AsignacionModelo expediente)
        {
            expediente.FechaOperacion = UtilidadesServicio.FechaActualUtc;
            expediente.IdTipoOperacion = TipoOperacion.Rechazado;

            return await _datos.RechazarRecepcionExpedienteAsync(expediente);
        }
        public async Task<bool> RechazarMasivamenteRecepcionExpedientesAsync(AsignacionModelo[] expedientes)
        {
            int verificacion = 0;

            foreach (var expediente in expedientes)
            {
                expediente.FechaOperacion = UtilidadesServicio.FechaActualUtc;
                expediente.IdTipoOperacion = TipoOperacion.Rechazado;

                if (!await ExpedienteAsignadoAUsuario(expediente.IdEntidad, expediente.IdExpediente, expediente.IdUsuarioRegistro))
                    verificacion++;
            }

            if (verificacion > 0)
                return false;
            else
                return await _datos.RechazarMasivamenteRecepcionExpedientesAsync(expedientes);
        }
        public async Task<bool> TomarExpedienteAsync(AsignacionModelo expediente)
        {
            expediente.IdTipoOperacion = TipoOperacion.AsignarmeExpediente;
            expediente.FechaOperacion = UtilidadesServicio.FechaActualUtc;

            return await _datos.TomarExpedienteAsync(expediente);
        }
        public async Task<bool> PuedeOperarse(int idEntidad, int idExpediente, int idUsuario)
        {
            return await _datos.PuedeOperarse(idEntidad, idExpediente, idUsuario);
        }
        public async Task<bool> ExpedienteAsignadoAUsuario(int idEntidad, int idExpediente, int idUsuario)
        {
            return await _datos.ExpedienteAsignadoAUsuario(idEntidad, idExpediente, idUsuario);
        }
        public async Task<bool> VincularExpediente(VinculacionExpedienteModelo expedienteVinculado)
        {

            expedienteVinculado.FechaRegistro = UtilidadesServicio.FechaActualUtc;

            bool resultado = await _datos.VincularExpediente(expedienteVinculado);

            return resultado;
        }
        public async Task<bool> EliminarVinculacionAsync(int idExpediente, int idExpedienteVinculado)
        {
            return await _datos.EliminarVinculacionAsync(idExpediente, idExpedienteVinculado);
        }
        public async Task<List<ListadoExpedientesVinculadosModelo>> ObtenerExpedientesVinculadosAsync(int idExpediente)
        {
            return await _datos.ObtenerExpedientesVinculadosAsync(idExpediente);
        }
        public async Task<List<ListadoExpedientesVinculadosModelo>> ObtenerExpedientesDisponiblesVincularAsync(int UsuarioAsignado, int idExpediente)
        {
            return await _datos.ObtenerExpedientesDisponiblesVincularAsync(UsuarioAsignado, idExpediente);
        }
        public async Task<ExpedienteModelo> ObtenerExpedienteAVincularAsync(int idExpediente)
        {
            return await _datos.ObtenerExpedienteAVincularAsync(idExpediente);
        }
        public async Task<bool> CopiarExpediente(CopiaExpedienteModelo expedienteVinculado) {            

            bool resultado = await _datos.CopiarExpediente(expedienteVinculado, UtilidadesServicio.FechaActualUtc);

            return resultado;
        }
        public async Task<bool> UnificarExpedientes(ExpedienteModelo[] expedientes)
        {
            bool resultado = await _datos.UnificarExpedientes(expedientes, UtilidadesServicio.FechaActualUtc);

            return resultado;
        }
    }
}