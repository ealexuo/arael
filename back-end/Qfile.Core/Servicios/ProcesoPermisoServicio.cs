using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Qfile.Core.Datos;

namespace Qfile.Core.Servicios
{
    public class ProcesoPermisoServicio : IProcesoPermisoServicio
    {
        private readonly IProcesoPermisoDatos _datos;
        private readonly IProcesoDatos _procesoDatos;

        public ProcesoPermisoServicio(IProcesoPermisoDatos datos, IProcesoDatos procesoDatos)
        {
            _datos = datos;
            _procesoDatos = procesoDatos;
        }

        public async Task<List<ProcesoPermisosModelo>> ObtenerProcesosPermisosAsync(int idEntidad)
        {
            List<ProcesoPermisosModelo> procesosPermisos = new List<ProcesoPermisosModelo>();

            var listaProcesos = await _procesoDatos.ObtenerTodosAsync(idEntidad);
            var listaPermisos = await _datos.ObtenerPermisosAsync();

            foreach(var proceso in listaProcesos)
            {
                ProcesoPermisosModelo procesoPermisos = new ProcesoPermisosModelo();
                procesoPermisos.Proceso = proceso;
                procesoPermisos.ListaPermisos = new List<ProcesoPermisoModelo>();

                foreach(var permiso in listaPermisos)
                {
                    procesoPermisos.ListaPermisos.Add(permiso);
                }

                procesosPermisos.Add(procesoPermisos);
            }

            return procesosPermisos;
        }

        public async Task<List<ProcesoPermisosModelo>> ObtenerProcesosPermisosPorUsuarioAsync(int idEntidad, int idUsuario)
        {
            List<ProcesoPermisosModelo> procesosPermisos = new List<ProcesoPermisosModelo>();

            var listaProcesos = await _procesoDatos.ObtenerTodosAsync(idEntidad);
            var listaPermisos = await _datos.ObtenerPermisosAsync();
            var listaPermisosPorUsuario = await _datos.ObtenerPermisosPorUsuarioAsync(idEntidad, idUsuario);

            // Se recorre la lista de procesos existentes
            foreach (var proceso in listaProcesos)
            {
                ProcesoPermisosModelo procesoPermisos = new ProcesoPermisosModelo();
                procesoPermisos.Proceso = proceso;
                procesoPermisos.ListaPermisos = new List<ProcesoPermisoModelo>();

                // Se recorre la lista de permisos existentes 
                foreach (var permiso in listaPermisos)
                {
                    var indicePermisoPorUsuario = listaPermisosPorUsuario.FindIndex(ppu => ppu.IdPermiso == permiso.IdPermiso);

                    if (indicePermisoPorUsuario >= 0)
                        permiso.Habilitado = true;

                    procesoPermisos.ListaPermisos.Add(permiso);
                }

                procesosPermisos.Add(procesoPermisos);
            }

            return procesosPermisos;
        }

        public async Task<int> GuardarPermisosAsync(ProcesosPermisosUsuarioModelo procesosPermisosUsuario)
        {
            return await _datos.GuardarPermisosAsync(procesosPermisosUsuario);
        }

        public async Task<bool> UsuarioTienePermiso(string permiso, int idUsuario, int idEntidad, int idProceso)
        {
            var permisosUsuario = await this.ObtenerProcesosPermisosPorUsuarioAsync(idEntidad, idUsuario);

            foreach(var pu in permisosUsuario)
            {
                if(pu.Proceso.IdEntidad == idEntidad && pu.Proceso.IdProceso == idProceso)
                {
                    foreach(var pp in pu.ListaPermisos)
                    {
                        if(pp.Nombre == permiso && pp.Habilitado)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
