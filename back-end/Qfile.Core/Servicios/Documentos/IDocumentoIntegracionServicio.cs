using Microsoft.AspNetCore.Http;
using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios.Documentos
{
    public interface IDocumentoIntegracionServicio
    {
        Task<string> GuardarDocumento(IFormFile documento, int idExpediente);
        Task<string> ReemplazarDocumento(IFormFile documentoNuevo, DocumentoModelo documentoActual);
        Task<bool> MoverArchivo(DocumentoModelo documentoActual, string rutaActual, string rutaNueva);
        Task<MemoryStream> DescagarDocumento(string rutaDocumento);
        Task<bool> EliminarDocumento(string rutaDocumento, string nombreDocumento);
        Task<bool> CopiarDocumentosExpediente(int idExpedienteOrigen, int idExpedienteDestino);
    }
}
