using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qfile.Core.Datos
{
    public interface IProcesoPermisoDatos
    {
        Task<List<ProcesoPermisoModelo>> ObtenerPermisosAsync();
        Task<int> GuardarPermisosAsync(ProcesosPermisosUsuarioModelo procesosPermisosUsuario);
        Task<List<ProcesosPermisosUsuariosModelo>> ObtenerPermisosPorUsuarioAsync(int idEntidad, int idUsuario);
    }
}
