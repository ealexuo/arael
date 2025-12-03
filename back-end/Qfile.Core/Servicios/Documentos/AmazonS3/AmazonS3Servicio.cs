using Microsoft.AspNetCore.Http;
using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios.Documentos
{
    public class AmazonS3Servicio : IDocumentoIntegracionServicio
    {
        public AmazonS3Servicio()
        {

        }

        public Task<MemoryStream> DescagarDocumento(string rutaDocumento)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EliminarDocumento(string rutaDocumento, string nombreDocumento)
        {
            throw new NotImplementedException();
        }

        public Task<string> GuardarDocumento(IFormFile documento, int idExpediente)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MoverArchivo(DocumentoModelo documentoActual, string rutaActual, string nuevaRuta)
        {
            throw new NotImplementedException();
        }

        public Task<string> ReemplazarDocumento(IFormFile documentoNuevo, DocumentoModelo documentoActual)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CopiarDocumentosExpediente(int idExpedienteOrigen, int idExpedienteDestino)
        {
            throw new NotImplementedException();
        }
    }
}
