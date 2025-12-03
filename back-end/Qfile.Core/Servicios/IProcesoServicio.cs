using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public interface IProcesoServicio
    {
        Task<List<ProcesoModelo>> ObtenerTodosAsync(int idEntidad);
        Task<int> CrearProcesoAsync(ProcesoModelo proceso, int idUsuario);
        Task<int> ActualizarProcesoAsync(ProcesoModelo proceso);
        Task<int> EliminarProcesoAsync(int idEntidad, int idProceso);
        Task<ProcesoModelo> ObtenerProcesoAsync(int idEntidad, int idProceso);
        Task<bool> existenExpedientes(int idProceso);
        Task<bool> existenSecciones(int idProceso);
        Task<List<ProcesoModelo>> ObtenerProcesosActivosAsync(int idEntidad, int idUsuario);
    }
}
