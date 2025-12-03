using Microsoft.AspNetCore.Http;
using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Datos
{
    public interface IDocumentoDatos
    {
        Task<int> GuardarDocumentoAsync(DocumentoModelo documento);
        Task<int> ActualizarDocumentoAsync(DocumentoModelo documento);
        Task<List<DocumentoModelo>> ObtenerDocumentosAsync(int idExpediente);
        Task<DocumentoModelo> ObtenerDocumentoAsync(int idExpediente, string nombreDocumento);
        Task<DocumentoModelo> ObtenerDocumentoPorIdAsync(int idDocumento);
        Task<HistoricoDocumentoModelo> ObtenerDocumentoHistoricoPorIdAsync(int idDocumento);
        Task<string> GuardarHistoricoDocumentoAsync(DocumentoModelo documento);
        Task<List<HistoricoDocumentoModelo>> ObtenerHistoricoDocumentoAsync(int idDocumentoReemplazado);
        Task<bool> EliminarDocumentoAsync(int idDocumento);
    }
}
