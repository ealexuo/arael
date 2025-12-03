using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Datos
{
    public interface IProcesoDatos
    {
        Task<List<ProcesoModelo>> ObtenerTodosAsync(int idEntidad);
        Task<int> CrearProcesoAsync(ProcesoModelo proceso, int idUsuario, DateTime fechaRegistro);
        Task<int> ActualizarProcesoAsync(ProcesoModelo proceso);
        Task<int> EliminarProcesoAsync(int idEntidad, int idProceso);
        Task<ProcesoModelo> ObtenerProcesoAsync(int idEntidad, int idProceso);
        Task<bool> ProcesoCompletoAsync (ProcesoModelo proceso);
        Task<List<ProcesoModelo>> ObtenerProcesosActivosAsync(int idEntidad, int idUsuario);
    }
}
