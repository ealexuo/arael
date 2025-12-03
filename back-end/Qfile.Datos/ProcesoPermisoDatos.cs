using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using Dapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Data;

namespace Qfile.Datos
{
    public class ProcesoPermisoDatos : IProcesoPermisoDatos
    {
        private readonly IConnectionProvider connectionProvider;

        public ProcesoPermisoDatos(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }

        public async Task<List<ProcesoPermisoModelo>> ObtenerPermisosAsync()
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"SELECT ID_PERMISO AS IdPermiso, NOMBRE as Nombre, DESCRIPCION as Descripcion FROM AD_PROCESOS_PERMISOS ORDER BY ID_PERMISO";
                var result = await connection.QueryAsync<ProcesoPermisoModelo>(sqlQuery, new { });
                return result.ToList();
            }
        }

        public async Task<List<ProcesosPermisosUsuariosModelo>> ObtenerPermisosPorUsuarioAsync(int idEntidad, int idUsuario)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"SELECT 
	                                    ID_PROCESO_ENTIDAD AS IdProcesoEntidad,
	                                    ID_PROCESO AS IdProceso,
	                                    ID_PERMISO AS IdPermiso,
	                                    ID_USUARIO_ENTIDAD AS IdUsuarioEntidad,
	                                    ID_USUARIO AS IdUsuario
                                    FROM AD_PROCESOS_PERMISOS_USUARIOS 
                                    WHERE ID_PROCESO_ENTIDAD = @IdEntidad 
	                                    AND ID_USUARIO_ENTIDAD = @IdEntidad 
	                                    AND ID_USUARIO = @IdUsuario";

                var result = await connection.QueryAsync<ProcesosPermisosUsuariosModelo>(sqlQuery, new { 
                    IdEntidad = idEntidad,
                    idUsuario = idUsuario
                });
                return result.ToList();
            }
        }

        public async Task<int> GuardarPermisosAsync(ProcesosPermisosUsuarioModelo procesosPermisosUsuario)
        {
            int resultado = 0;

            using (var connection = await connectionProvider.OpenAsync())
            {
                string eliminarPermisoSQL = @"DELETE AD_PROCESOS_PERMISOS_USUARIOS WHERE ID_USUARIO_ENTIDAD = @IdUsuarioEntidad AND ID_USUARIO = @IdUsuario";

                string insertarPermisoSQL = @"INSERT INTO AD_PROCESOS_PERMISOS_USUARIOS
                                            (ID_PROCESO_ENTIDAD, ID_PROCESO, ID_PERMISO, ID_USUARIO_ENTIDAD, ID_USUARIO)
                                            VALUES(@IdProcesoEntidad, @IdProceso, @IdPermiso, @IdUsuarioEntidad, @IdUsuario)";
                
                using (var trx = connection.BeginTransaction())
                {
                    await connection.ExecuteAsync(eliminarPermisoSQL, new { 
                        IdUsuarioEntidad = procesosPermisosUsuario.Usuario.IdEntidad,
                        IdUsuario = procesosPermisosUsuario.Usuario.IdUsuario
                    }, trx);

                    foreach (var procesoPermisosUsuario in procesosPermisosUsuario.ListaProcesosPermisos)
                    {
                        foreach (var permiso in procesoPermisosUsuario.ListaPermisos)
                        {
                            if(permiso.Habilitado)
                            {
                                resultado += await connection.ExecuteAsync(insertarPermisoSQL, new
                                {
                                    IdProcesoEntidad = procesoPermisosUsuario.Proceso.IdEntidad,
                                    IdProceso = procesoPermisosUsuario.Proceso.IdProceso,
                                    IdPermiso = permiso.IdPermiso,
                                    IdUsuarioEntidad = procesosPermisosUsuario.Usuario.IdEntidad,
                                    IdUsuario = procesosPermisosUsuario.Usuario.IdUsuario
                                }, trx);
                            }                            
                        }
                    }

                    trx.Commit();
                }
            }

            return resultado;
        }
    }
}
