using Dapper;
using Microsoft.AspNetCore.Http;
using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Datos
{
    public class DocumentoDatos : IDocumentoDatos
    {
        private readonly IConnectionProvider connectionProvider;

        public DocumentoDatos(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }

        public async Task<int> GuardarDocumentoAsync(DocumentoModelo documento)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            const string insertarDocumentoSQL = @"                
                                     --DECLARE @idDocumento INT;

                                    SELECT @idDocumento = ISNULL(MAX(ID_DOCUMENTO), 0) + 1 
                                    FROM EXP_EXPEDIENTE_DOCUMENTOS;

                                    INSERT INTO EXP_EXPEDIENTE_DOCUMENTOS (
                                        ID_DOCUMENTO, ID_EXPEDIENTE, NOMBRE, RUTA, EXTENSION, ID_INTEGRACION, 
                                        ID_USUARIO_CARGA, ID_ENTIDAD_CARGA, ID_USUARIO_MODIFICACION, 
                                        ID_ENTIDAD_MODIFICACION, FECHA_CARGA, FECHA_MODIFICACION, 
                                        OBSERVACIONES, ID_ALMACENAMIENTO
                                    )
                                    VALUES (
                                        @idDocumento, @idExpediente, @nombreArchivo, @ruta, @extension, @idIntegracion, 
                                        @idUsuarioCarga, @idEntidadCarga, @idUsuarioModificacion, 
                                        @idEntidadModificacion, @fechaCarga, @fechaModificacion, 
                                        @observaciones, @idAlmacenamiento
                                    );";

            using (var connection = await connectionProvider.OpenAsync())
            {

                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@idDocumento", documento.IdDocumento);
                    param.Add("@idExpediente", documento.IdExpediente);
                    param.Add("@nombreArchivo", documento.Nombre);
                    param.Add("@ruta", documento.Ruta);
                    param.Add("@extension", documento.Extension);
                    param.Add("@idIntegracion", documento.IdIntegracion);
                    param.Add("@idUsuarioCarga", documento.IdUsuarioCarga);
                    param.Add("@idEntidadCarga", documento.IdEntidadCarga);
                    param.Add("@idUsuarioModificacion", documento.IdUsuarioModificacion);
                    param.Add("@idEntidadModificacion", documento.IdEntidadModificacion);
                    param.Add("@fechaCarga", documento.FechaCarga);
                    param.Add("@fechaModificacion", documento.FechaModificacion);
                    param.Add("@observaciones", documento.Observaciones);
                    param.Add("@idDocumentoGuid", documento.IdDocumentoGuid);
                    param.Add("@idAlmacenamiento", documento.IdAlmacenamiento);
                    await connection.ExecuteAsync(insertarDocumentoSQL, param, trx);

                    documento.IdDocumento = param.Get<int>("idDocumento");

                    trx.Commit();
                }
            }
                
            return documento.IdDocumento;
        }

        public async Task<int> ActualizarDocumentoAsync(DocumentoModelo documento)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);

            const string actualizarDocumentoSQL = @"                
                UPDATE EXP_EXPEDIENTE_DOCUMENTOS
                SET 
	                ID_EXPEDIENTE= :IdExpediente, 
	                NOMBRE= :NombreArchivo, 
	                RUTA= :Ruta, 
	                EXTENSION= :Extension, 
	                ID_INTEGRACION= :IdIntegracion, 
	                ID_USUARIO_CARGA= :IdUsuarioCarga,
	                ID_ENTIDAD_CARGA= :IdEntidadCarga, 
	                ID_USUARIO_MODIFICACION= :IdUsuarioModificacion, 
	                ID_ENTIDAD_MODIFICACION= :IdEntidadModificacion, 
	                FECHA_CARGA= :FechaCarga, 
	                FECHA_MODIFICACION= :FechaModificacion, 
	                OBSERVACIONES= :Observaciones, 
	                ID_DOCUMENTO_GUID= :IdDocumentoGuid
                WHERE ID_DOCUMENTO= :IdDocumento";

            using (var connection = await connectionProvider.OpenAsync())
            {

                using (var trx = connection.BeginTransaction())
                {
                    var param = new DynamicParameters();
                    param.Add("@IdDocumento", documento.IdDocumento);
                    param.Add("@IdExpediente", documento.IdExpediente);
                    param.Add("@NombreArchivo", documento.Nombre);
                    param.Add("@Ruta", documento.Ruta);
                    param.Add("@Extension", documento.Extension);
                    param.Add("@IdIntegracion", documento.IdIntegracion);
                    param.Add("@IdUsuarioCarga", documento.IdUsuarioCarga);
                    param.Add("@IdEntidadCarga", documento.IdEntidadCarga);
                    param.Add("@IdUsuarioModificacion", documento.IdUsuarioModificacion);
                    param.Add("@IdEntidadModificacion", documento.IdEntidadModificacion);
                    param.Add("@FechaCarga", documento.FechaCarga);
                    param.Add("@FechaModificacion", documento.FechaModificacion);
                    param.Add("@Observaciones", documento.Observaciones);
                    param.Add("@IdDocumentoGuid", documento.IdDocumentoGuid);
                    await connection.ExecuteAsync(actualizarDocumentoSQL, param, trx);
                    trx.Commit();
                }
            }

            return documento.IdDocumento;
        }

        public async Task<DocumentoModelo> ObtenerDocumentoAsync(int idExpediente, string nombreDocumento)
        {
            string sqlQuery = @"SELECT TOP 1
                                doc.ID_DOCUMENTO AS IdDocumento, 
                                doc.ID_EXPEDIENTE AS IdExpediente, 
                                doc.NOMBRE AS Nombre, 
                                doc.RUTA AS Ruta, 
                                doc.EXTENSION AS Extension, 
                                doc.ID_INTEGRACION AS IdIntegracion, 
                                doc.ID_USUARIO_CARGA AS IdUsuarioCarga, 
                                doc.ID_ENTIDAD_CARGA AS IdEntidadCarga, 
                                doc.ID_USUARIO_MODIFICACION AS IdUsuarioModificacion, 
                                doc.ID_ENTIDAD_MODIFICACION AS IdEntidadModificacion, 
                                doc.FECHA_CARGA AS IdFechaCarga, 
                                doc.FECHA_MODIFICACION AS FechaModificacion, 
                                doc.OBSERVACIONES AS Observaciones, 
                                doc.ID_DOCUMENTO_GUID AS IdDocumentoGuid,
                                usrc.PRIMER_NOMBRE + ' ' + usrc.PRIMER_APELLIDO AS NombreUsuarioCarga,
                                usrm.PRIMER_NOMBRE + ' ' + usrm.PRIMER_APELLIDO AS NombreUsuarioModificacion
                            FROM EXP_EXPEDIENTE_DOCUMENTOS doc
                            JOIN AD_USUARIOS usrc
                                ON doc.ID_USUARIO_CARGA = usrc.ID_USUARIO
                            JOIN AD_USUARIOS usrm
                                ON doc.ID_USUARIO_MODIFICACION = usrm.ID_USUARIO
                            WHERE doc.NOMBRE = @nombreDocumento 
                              AND doc.ID_EXPEDIENTE = @idExpediente;";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var result = await connection.QueryFirstOrDefaultAsync<DocumentoModelo>(sqlQuery, new { nombreDocumento, idExpediente });

                return result;
            }
        }

        public async Task<DocumentoModelo> ObtenerDocumentoPorIdAsync(int idDocumento)
        {
            string sqlQuery = @"SELECT 
                                    doc.ID_DOCUMENTO IdDocumento, 
                                    doc.ID_EXPEDIENTE IdExpediente, 
                                    doc.NOMBRE Nombre, 
                                    doc.RUTA Ruta, 
                                    doc.EXTENSION Extension, 
                                    doc.ID_INTEGRACION IdIntegracion, 
                                    doc.ID_USUARIO_CARGA IdUsuarioCarga, 
                                    doc.ID_ENTIDAD_CARGA IdEntidadCarga, 
                                    doc.ID_USUARIO_MODIFICACION IdUsuarioModificacion, 
                                    doc.ID_ENTIDAD_MODIFICACION IdEntidadModificacion, 
                                    doc.FECHA_CARGA FechaCarga, 
                                    doc.FECHA_MODIFICACION FechaModificacion, 
                                    doc.OBSERVACIONES Observaciones, 
                                    doc.ID_DOCUMENTO_GUID IdDocumentoGuid
                                FROM EXP_EXPEDIENTE_DOCUMENTOS doc                                
                                WHERE doc.ID_DOCUMENTO = :idDocumento
                                ";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var result = await connection.QueryFirstOrDefaultAsync<DocumentoModelo>(sqlQuery, new { idDocumento });

                return result;
            }
        }

        public async Task<HistoricoDocumentoModelo> ObtenerDocumentoHistoricoPorIdAsync(int idDocumento)
        {
            string sqlQuery = @"SELECT 
	                                hd.ID_HISTORICO_DOCUMENTO IdHistoricoDocumento, 
	                                hd.ID_DOCUMENTO_REEMPLAZADO IdDocumentoReemplazado, 
	                                hd.NOMBRE Nombre, 
	                                hd.RUTA Ruta, 
	                                hd.EXTENSION Extension, 
	                                hd.ID_INTEGRACION IdIntegracion, 
	                                hd.ID_USUARIO_CARGA IdUsuarioCarga, 
	                                hd.ID_ENTIDAD_CARGA IdEntidadCarga, 
	                                hd.ID_USUARIO_MODIFICACION IdUsuarioModificacion, 
	                                hd.ID_ENTIDAD_MODIFICACION IdEntidadModificacion, 
	                                hd.FECHA_CARGA FechaCarga, 
	                                hd.FECHA_MODIFICACION FechaModificacion, 
	                                hd.OBSERVACIONES Observaciones,
                                    ed.id_expediente
                                FROM HISTORICO_DOCUMENTO hd, EXP_EXPEDIENTE_DOCUMENTOS ed
                                WHERE hd.ID_HISTORICO_DOCUMENTO = :idDocumento
                                AND ed.id_documento = hd.ID_DOCUMENTO_REEMPLAZADO
                                ";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var result = await connection.QueryFirstOrDefaultAsync<HistoricoDocumentoModelo>(sqlQuery, new { idDocumento });

                return result;
            }
        }

        public async Task<List<DocumentoModelo>> ObtenerDocumentosAsync(int idExpediente)
        {
            string sqlQuery = @"
                                SELECT 
                                    doc.ID_DOCUMENTO IdDocumento, 
                                    doc.ID_EXPEDIENTE IdExpediente, 
                                    doc.NOMBRE Nombre, 
                                    doc.RUTA Ruta, 
                                    doc.EXTENSION Extension, 
                                    doc.ID_INTEGRACION IdIntegracion, 
                                    doc.ID_USUARIO_CARGA IdUsuarioCarga, 
                                    doc.ID_ENTIDAD_CARGA IdEntidadCarga, 
                                    doc.ID_USUARIO_MODIFICACION IdUsuarioModificacion, 
                                    doc.ID_ENTIDAD_MODIFICACION IdEntidadModificacion, 
                                    doc.FECHA_CARGA IdFechaCarga, 
                                    doc.FECHA_MODIFICACION FechaModificacion, 
                                    doc.OBSERVACIONES Observaciones, 
                                    doc.ID_DOCUMENTO_GUID IdDocumentoGuid,
                                    usrc.PRIMER_NOMBRE + usrc.PRIMER_APELLIDO NombreUsuarioCarga,
                                    usrm.PRIMER_NOMBRE + usrm.PRIMER_APELLIDO NombreUsuarioModificacion
                                FROM EXP_EXPEDIENTE_DOCUMENTOS doc
                                JOIN AD_USUARIOS usrc
                                    ON doc.ID_USUARIO_CARGA = usrc.ID_USUARIO
                                JOIN AD_USUARIOS usrm
                                    ON doc.ID_USUARIO_MODIFICACION = usrm.ID_USUARIO 
                                WHERE doc.ID_EXPEDIENTE = @idExpediente";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var result = await connection.QueryAsync<DocumentoModelo>(sqlQuery, new { idExpediente });

                return result.ToList();
            }
        }

        public async Task<string> GuardarHistoricoDocumentoAsync(DocumentoModelo documento)
        {
            SqlMapper.AddTypeMap(typeof(bool), DbType.Byte);
            string nuevaRuta = "";

            string SQLInsertarHistorico = @"INSERT INTO HISTORICO_DOCUMENTO
                                                (ID_HISTORICO_DOCUMENTO, ID_DOCUMENTO_REEMPLAZADO, NOMBRE, RUTA, EXTENSION, ID_INTEGRACION, ID_USUARIO_CARGA, ID_ENTIDAD_CARGA, 
                                                ID_USUARIO_MODIFICACION, ID_ENTIDAD_MODIFICACION, FECHA_CARGA, FECHA_MODIFICACION, OBSERVACIONES)
                                            VALUES((SELECT NVL(MAX(ID_HISTORICO_DOCUMENTO), 0) + 1 FROM HISTORICO_DOCUMENTO), :IdDocumento, :Nombre, :Ruta, :Extension, 
                                                :IdIntegracion, :IdUsuarioCarga, :IdEntidadCarga, :IdUsuarioModificacion, :IdEntidadModificacion, :FechaCarga, :FechaModificacion, :Observaciones)
                                            RETURNING ID_HISTORICO_DOCUMENTO INTO :IdHistoricoDocumento";

            string SQLActualizarRutaHistorico = @"UPDATE HISTORICO_DOCUMENTO SET RUTA = :Ruta WHERE ID_HISTORICO_DOCUMENTO = :IdHistoricoDocumento";

            using (var connection = await connectionProvider.OpenAsync())
            {

                using (var trx = connection.BeginTransaction())
                {
                    // Insertar histórico documento
                    var paramInsertar = new DynamicParameters();
                    paramInsertar.Add("@IdHistoricoDocumento", 0);
                    paramInsertar.Add("@IdDocumento", documento.IdDocumento);
                    paramInsertar.Add("@Nombre", documento.Nombre);
                    paramInsertar.Add("@Ruta", documento.Ruta);
                    paramInsertar.Add("@Extension", documento.Extension);
                    paramInsertar.Add("@IdIntegracion", documento.IdIntegracion);
                    paramInsertar.Add("@IdUsuarioCarga", documento.IdUsuarioCarga);
                    paramInsertar.Add("@IdEntidadCarga", documento.IdEntidadCarga);
                    paramInsertar.Add("@IdUsuarioModificacion", documento.IdUsuarioModificacion);
                    paramInsertar.Add("@IdEntidadModificacion", documento.IdEntidadModificacion);
                    paramInsertar.Add("@FechaCarga", documento.FechaCarga);
                    paramInsertar.Add("@FechaModificacion", documento.FechaModificacion);
                    paramInsertar.Add("@Observaciones", documento.Observaciones);
                    await connection.ExecuteAsync(SQLInsertarHistorico, paramInsertar, trx);

                    // Actualizar ruta
                    int idHistoricoDocumento = paramInsertar.Get<int>("IdHistoricoDocumento");
                    nuevaRuta = documento.Ruta + "\\" + documento.IdExpediente + "\\" + idHistoricoDocumento;
                    var paramActualizar = new DynamicParameters();

                    paramActualizar.Add("@IdHistoricoDocumento", idHistoricoDocumento);
                    paramActualizar.Add("@Ruta", nuevaRuta);
                    await connection.ExecuteAsync(SQLActualizarRutaHistorico, paramActualizar, trx);

                    idHistoricoDocumento = paramActualizar.Get<int>("IdHistoricoDocumento");

                    trx.Commit();
                }
            }

            return nuevaRuta;
        }

        public async Task<List<HistoricoDocumentoModelo>> ObtenerHistoricoDocumentoAsync(int idDocumentoReemplazado)
        {
            string sqlQuery = @"
                                SELECT 
	                                hd.ID_HISTORICO_DOCUMENTO IdHistoricoDocumento, 
	                                hd.ID_DOCUMENTO_REEMPLAZADO IdDocumentoReemplazado, 
	                                hd.NOMBRE Nombre, 
	                                hd.RUTA Ruta, 
	                                hd.EXTENSION Extension, 
	                                hd.ID_INTEGRACION IdIntegracion, 
	                                hd.ID_USUARIO_CARGA IdUsuarioCarga,
	                                hd.ID_ENTIDAD_CARGA IdEntidadCarga, 
	                                hd.ID_USUARIO_MODIFICACION IdUsuarioModificacion, 
	                                hd.ID_ENTIDAD_MODIFICACION IdEntidadModificacion, 
	                                hd.FECHA_CARGA FechaCarga, 
	                                hd.FECHA_MODIFICACION FechaModificacion, 
	                                hd.OBSERVACIONES Observaciones,
                                    usrc.PRIMER_NOMBRE || usrc.PRIMER_APELLIDO NombreUsuarioCarga,
                                    usrm.PRIMER_NOMBRE || usrm.PRIMER_APELLIDO NombreUsuarioModificacion
                                FROM HISTORICO_DOCUMENTO hd
                                JOIN AD_USUARIOS usrc
                                    ON hd.ID_USUARIO_CARGA = usrc.ID_USUARIO
                                JOIN AD_USUARIOS usrm
                                    ON hd.ID_USUARIO_MODIFICACION = usrm.ID_USUARIO
                                WHERE hd.ID_DOCUMENTO_REEMPLAZADO = :IdDocumentoReemplazado";

            using (var connection = await connectionProvider.OpenAsync())
            {
                var result = await connection.QueryAsync<HistoricoDocumentoModelo>(sqlQuery, new { idDocumentoReemplazado });

                return result.ToList();
            }
        }

        public async Task<bool> EliminarDocumentoAsync(int idDocumento)
        {
            string eliminarDocumentoSQL = "DELETE FROM EXP_EXPEDIENTE_DOCUMENTOS WHERE ID_DOCUMENTO = :IdDocumento";

            using (var connection = await connectionProvider.OpenAsync())
            {
                int registrosAfectados = 0;

                using (var trx = connection.BeginTransaction())
                {
                    registrosAfectados += await connection.ExecuteAsync(eliminarDocumentoSQL, new { idDocumento }, trx);
                    trx.Commit();
                }
                return registrosAfectados > 0;
            }
        }

    }
}
