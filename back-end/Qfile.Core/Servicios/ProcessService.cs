using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using Qfile.Core.Tipos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public class ProcessService : IProcessService
    {
        private readonly IProcessData _datos;

        public ProcessService(IProcessData datos)
        {
            _datos = datos;
        }
        public async Task<List<ProcesoModelo>> GetAllAsync(int idEntity)
        {
            try
            {
                return await _datos.GetAllAsync(idEntity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> CrearProcesoAsync(ProcesoModelo proceso, int idUsuario)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.CrearProcesoAsync(proceso, idUsuario, fechaRegistro);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> ActualizarProcesoAsync(ProcesoModelo proceso)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;

                if (proceso.Estado == true) {
                    if (!await _datos.ProcesoCompletoAsync(proceso)) {
                        return -1;
                    }
                }

                return await _datos.ActualizarProcesoAsync(proceso);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> EliminarProcesoAsync(int idEntidad, int idProceso)
        {
            try
            {
                // PENDIETNE REALIZAR VALIDACIONEs

                return await _datos.EliminarProcesoAsync(idEntidad, idProceso);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<ProcesoModelo> ObtenerProcesoAsync(int idEntidad, int idProceso)
        {
            try
            {
                return await _datos.ObtenerProcesoAsync(idEntidad, idProceso);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<bool> existenExpedientes(int idProceso) {
            return false;
        }
        public async Task<bool> existenSecciones(int idProceso) {
            return false;
        }

        public async Task<List<ProcesoModelo>> ObtenerProcesosActivosAsync(int idEntidad, int idUsuario)
        {
            try
            {
                return await _datos.ObtenerProcesosActivosAsync(idEntidad, idUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
