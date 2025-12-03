using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using Dapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Data;
using System.ComponentModel.Design;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using Qfile.Core.Constantes;

namespace Qfile.Datos
{
    public class RequisitoCreacionDatos : IRequisitoCreacionDatos
    {
        private readonly IConnectionProvider connectionProvider;

        public RequisitoCreacionDatos(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }
        public async Task<List<RequisitoGestionModelo>> ObtenerRequisitosAsync(int idEntidad, int idProceso)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"select id_entidad IdEntidad, id_proceso IdProceso, id_requisito IdRequisito, requisito, Obligatorio, id_Usuario UsuarioRegistro
                                    from Proceso_requisitos
                                    where id_entidad = @IdEntidad
                                    and id_proceso = @IdProceso
                                    order by id_requisito";

                var result = await connection.QueryAsync<RequisitoGestionModelo>(sqlQuery, new { idEntidad, idProceso });
                return result.ToList();
            }
        }
        public async Task<int> CrearRequisitoAsync(RequisitoGestionModelo requisito)
        {
            const string insertarRequisitoSQL = @"                
                INSERT INTO PROCESO_REQUISITOS
                (
                    ID_ENTIDAD, ID_PROCESO, ID_REQUISITO, REQUISITO, OBLIGATORIO, ID_USUARIO, FECHA_REGISTRO
                )
                OUTPUT INSERTED.ID_PROCESO
                VALUES(
                   @IdEntidad, @IdProceso, (Select ISNULL(max(id_requisito),0) + 1 from proceso_requisitos where id_entidad = @IdEntidad and id_proceso = @IdProceso), @Requisito, @Obligatorio, @IdUsuario, @FechaRegistro
                )
               ";
            
            using (var connection = await connectionProvider.OpenAsync())
            {                
                var param = new DynamicParameters();
                param.Add("@IdProceso", requisito.IdProceso);
                param.Add("@IdEntidad", requisito.IdEntidad);
                param.Add("@IdRequisito", requisito.IdRequisito);
                param.Add("@Requisito", requisito.Requisito);
                param.Add("@Obligatorio", requisito.Obligatorio ? 1 : 0);
                param.Add("@IdUsuario", requisito.UsuarioRegistro);
                param.Add("@FechaRegistro", requisito.FechaRegistro);

                await connection.ExecuteAsync(insertarRequisitoSQL, param);

                requisito.IdRequisito = param.Get<int>("IdRequisito");                   
            }

            return requisito.IdRequisito;
        }

        public async Task<int> ActualizarRequisitoAsync(RequisitoGestionModelo requisito)
        {
            const string actualizarSQL = @"                
                UPDATE PROCESO_REQUISITOS SET 
                    REQUISITO = @Requisito,
                    OBLIGATORIO = @Obligatorio
                WHERE
                    ID_ENTIDAD = @IdEntidad AND
                    ID_PROCESO = @IdProceso AND
                    ID_REQUISITO = @IdRequisito";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var param = new DynamicParameters();
                param.Add("@IdProceso", requisito.IdProceso);
                param.Add("@IdEntidad", requisito.IdEntidad);
                param.Add("@IdRequisito", requisito.IdRequisito);
                param.Add("@Requisito", requisito.Requisito);
                param.Add("@Obligatorio", requisito.Obligatorio ? 1 : 0);
                param.Add("@Id_Usuario", requisito.UsuarioRegistro);

                var response = await connection.ExecuteAsync(actualizarSQL, param);

                return response;
            }
        }

        public async Task<int> EliminarRequisitoAsync(int idEntidad, int idProceso, int idRequisito)
        {
            const string insertarProcesoSQL = @"                
                DELETE PROCESO_REQUISITOS WHERE ID_ENTIDAD = @IdEntidad AND ID_PROCESO = @IdProceso AND ID_REQUISITO = @IdRequisito";

            using (var connection = await connectionProvider.OpenAsync())
            {

                var response = await connection.ExecuteAsync(insertarProcesoSQL, new { idEntidad, idProceso, idRequisito });

                return response;
            }

        }

    }
}