using Qfile.Core.Modelos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public interface IProcesoPermisoServicio
    {
        Task<List<ProcesoPermisosModelo>> ObtenerProcesosPermisosAsync(int idEntidad);
        Task<List<ProcesoPermisosModelo>> ObtenerProcesosPermisosPorUsuarioAsync(int idEntidad, int idUsuario);
        Task<int> GuardarPermisosAsync(ProcesosPermisosUsuarioModelo procesosPermisosUsuario);
        Task<bool> UsuarioTienePermiso(string permiso, int idUsuario, int idEntidad, int idProceso);
    }
}
