using Microsoft.AspNetCore.Http;
using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios.Documentos
{
    public interface IDocumentoServicio
    {
        Task<int> GuardarDocumentoAsync(IFormFile documento, int idUsuario, int idEntidad, int idExpediente);
        Task<DocumentoModelo> ObtenerDocumentoAsync(int idExpediente, string nombreDocumento);
        Task<DocumentoModelo> ObtenerDocumentoPorIdAsync(int idDocumento);
        Task<List<DocumentoModelo>> ObtenerDocumentosAsync(int idExpediente);
        Task<int> ReemplazarDocumentoAsync(IFormFile documento, int idUsuario, int idEntidad, int idExpediente, int idDocumento, string observaciones);
        Task<List<HistoricoDocumentoModelo>> ObtenerHistoricoDocumentoAsync(int idDocumentoReemplazado);
        Task<MemoryStream> DescargarDocumento(int idDocumento, bool esDocumentoActual);
        Task<bool> EliminarDocumentoAsync(int idDocumento);
        Task<bool> CopiarDocumentosExpediente(int idExpedienteOrigen, int idExpedienteDestino);
    }
}
