using Dapper;
using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Datos
{
    public class UnidadAdministrativaDatos : IUnidadAdministrativaDatos
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly IUsuarioDatos _usuarioDatos;

        public UnidadAdministrativaDatos(IConnectionProvider connectionProvider, IUsuarioDatos usuarioDatos)
        {
            _connectionProvider = connectionProvider;
            _usuarioDatos = usuarioDatos;
        }

        public async Task<int> CrearUnidadAdministrativaAsync(UnidadAdministrativaModelo unidadAdministrativa)
        {
            using (var connection = await _connectionProvider.OpenAsync())
            {
                string instruccionSQL = @"                
                INSERT INTO AD_UNIDADES_ADMINISTRATIVAS (
                    ID_ENTIDAD, 
                    ID_UNIDAD_ADMINISTRATIVA, 
                    NOMBRE, 
                    ACTIVA, 
                    SIGLAS,
                    ID_UNIDAD_ADMINISTRATIVA_PADRE, 
                    FECHA_CREACION
                )
                
                VALUES (
                    @IdEntidad, 
                    (SELECT ISNULL(MAX(ID_UNIDAD_ADMINISTRATIVA), 0) FROM AD_UNIDADES_ADMINISTRATIVAS) + 1,                     
                    @Nombre, 
                    @Activa, 
                    @Siglas,
                    @IdUnidadAdministrativaPadre, 
                    @FechaCreacion
                )";

                using (var trx = connection.BeginTransaction())
                {
                    // Obtener información del usuario registro
                    var usuarioRegistro = await _usuarioDatos.ObtenerPorIdAsync(unidadAdministrativa.IdUsuarioRegistro);

                    if(usuarioRegistro != null)
                    unidadAdministrativa.IdEntidad = usuarioRegistro.IdEntidad;

                    // Crear Unidad Administrativa
                    await connection.ExecuteAsync(instruccionSQL, new
                    {
                        unidadAdministrativa.IdEntidad,
                        unidadAdministrativa.Nombre,
                        Activa = 1,
                        unidadAdministrativa.Siglas,
                        unidadAdministrativa.IdUnidadAdministrativaPadre,
                        unidadAdministrativa.FechaCreacion
                    }, trx);
                    
                    trx.Commit();
                }

                return unidadAdministrativa.IdUnidadAdministrativa;
            }
        }

        public async Task<bool> ActualizarUnidadAdministrativaAsync(UnidadAdministrativaModelo unidadAdministrativa)
        {
            using (var connection = await _connectionProvider.OpenAsync())
            {
                int registrosAfectados = 0;

                string instruccionSQL = @"UPDATE AD_UNIDADES_ADMINISTRATIVAS 
                SET ID_ENTIDAD = @IdEntidad,
                NOMBRE = @Nombre, 
                ACTIVA = @Activa, 
                SIGLAS = @Siglas, 
                ID_UNIDAD_ADMINISTRATIVA_PADRE = @IdUnidadAdministrativaPadre
                WHERE ID_UNIDAD_ADMINISTRATIVA = @IdUnidadAdministrativa";

                //DBNull.Value
                using (var trx = connection.BeginTransaction())
                {
                    registrosAfectados += await connection.ExecuteAsync(instruccionSQL, new 
                    {
                        unidadAdministrativa.IdUnidadAdministrativa,
                        unidadAdministrativa.IdEntidad,
                        unidadAdministrativa.Nombre,
                        Activa = unidadAdministrativa.Activa ? 1 : 0,
                        unidadAdministrativa.Siglas,
                        IdUnidadAdministrativaPadre = unidadAdministrativa.IdUnidadAdministrativaPadre == null? (int?)null: unidadAdministrativa.IdUnidadAdministrativaPadre
                    }, trx);
                    trx.Commit();
                }
                return registrosAfectados > 0;
            }
        }
 
        public async Task<bool> EliminarUnidadAdministrativaAsync(int idUnidadAdministrativa)
        {
            using (var connection = await _connectionProvider.OpenAsync())
            {
                int registrosAfectados = 0;

                string instruccionSQL = @"DELETE AD_UNIDADES_ADMINISTRATIVAS 
                    WHERE ID_UNIDAD_ADMINISTRATIVA = @idUnidadAdministrativa";

                using (var trx = connection.BeginTransaction())
                {
                    registrosAfectados += await connection.ExecuteAsync(instruccionSQL, new { idUnidadAdministrativa }, trx);
                    trx.Commit();
                }
                return registrosAfectados > 0;
            }
        }
        public async Task<List<UnidadAdministrativaModelo>> ObtenerUnidadesAdministrativasAsync()
        {
            using (var connection = await _connectionProvider.OpenAsync())
            {
                string instruccionSQL = @"SELECT 
                                            UA.ID_ENTIDAD IdEntidad, 
                                            UA.ID_UNIDAD_ADMINISTRATIVA IdUnidadAdministrativa, 
                                            UA.NOMBRE, 
                                            UA.ACTIVA, 
                                            UA.SIGLAS,
                                            UA.ID_UNIDAD_ADMINISTRATIVA_PADRE IdUnidadAdministrativaPadre, 
                                            UAP.NOMBRE NombreUnidadAdministrativaPadre,
                                            UA.FECHA_CREACION FechaCreacion
                                        FROM AD_UNIDADES_ADMINISTRATIVAS UA LEFT OUTER JOIN AD_UNIDADES_ADMINISTRATIVAS UAP
                                        ON UA.ID_UNIDAD_ADMINISTRATIVA_PADRE = UAP.ID_UNIDAD_ADMINISTRATIVA
                                        ORDER BY UA.ID_UNIDAD_ADMINISTRATIVA";

                var result = await connection.QueryAsync<UnidadAdministrativaModelo>(instruccionSQL);
                return result.ToList();
            }
        }
        public async Task<UnidadAdministrativaModelo> ObtenerPorIdAsync(int idUnidadAdministrativa)
        {
            using (var connection = await _connectionProvider.OpenAsync())
            {
                string sqlQuery = @"SELECT 
                    ID_ENTIDAD IdEntidad, 
                    ID_UNIDAD_ADMINISTRATIVA IdUnidadAdministrativa, 
                    NOMBRE, 
                    ACTIVA, 
                    SIGLAS,
                    ID_UNIDAD_ADMINISTRATIVA_PADRE IdUnidadAdministrativaPadre, 
                    FECHA_CREACION FechaCreacion
                    FROM AD_UNIDADES_ADMINISTRATIVAS 
                    WHERE ID_UNIDAD_ADMINISTRATIVA = @idUnidadAdministrativa";

                var result = await connection.QueryAsync<UnidadAdministrativaModelo>(sqlQuery, new { idUnidadAdministrativa });
                return result.FirstOrDefault();
            }
        }

        public async Task<UnidadAdministrativaModelo> ObtenerPorNombreAsync(string nombreUnidadAdministrativa)
        {
            using (var connection = await _connectionProvider.OpenAsync())
            {
                string sqlQuery = @"SELECT 
                    ID_ENTIDAD IdEntidad, 
                    ID_UNIDAD_ADMINISTRATIVA IdUnidadAdministrativa, 
                    NOMBRE, 
                    ACTIVA, 
                    SIGLAS,
                    ID_UNIDAD_ADMINISTRATIVA_PADRE IdUnidadAdministrativaPadre, 
                    FECHA_CREACION FechaCreacion
                    FROM AD_UNIDADES_ADMINISTRATIVAS 
                    WHERE NOMBRE = @nombreUnidadAdministrativa";

                var result = await connection.QueryAsync<UnidadAdministrativaModelo>(sqlQuery, new { nombreUnidadAdministrativa });
                return result.FirstOrDefault();
            }
        }

        
    }
}
