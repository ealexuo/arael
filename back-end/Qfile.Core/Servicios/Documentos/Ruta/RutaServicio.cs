using Microsoft.AspNetCore.Http;
using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios.Documentos
{
    public class RutaServicio : IDocumentoIntegracionServicio
    {
        private readonly string AppDirectory = Path.Combine(Path.GetPathRoot(Directory.GetCurrentDirectory()),"QFile","DocumentosTemporales"); // C:\QFile\DocumentosTemporales

        public RutaServicio() { 
        
        }

        public async Task<string> GuardarDocumento(IFormFile documento, int idExpediente)
        {
            try
            {
                string directorioPorExpediente = Path.Combine(AppDirectory, idExpediente.ToString());

                if (!Directory.Exists(directorioPorExpediente))
                    Directory.CreateDirectory(directorioPorExpediente);

                var path = Path.Combine(directorioPorExpediente, documento.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await documento.CopyToAsync(stream);
                }

                return directorioPorExpediente;
            }
            catch
            {
                throw new Exception("Ocurrió un error al guardar el documento.");
            }            
        }

        public Task<string> ReemplazarDocumento(IFormFile documentoNuevo, DocumentoModelo documentoActual)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> MoverArchivo(DocumentoModelo documentoActual, string rutaActual, string rutaNueva)
        {
            try
            {
                rutaActual = rutaActual + documentoActual.IdExpediente;

                if (File.Exists(rutaActual + "\\" + documentoActual.Nombre))
                {
                    if (!Directory.Exists(rutaNueva))
                        Directory.CreateDirectory(rutaNueva);

                    File.Move(rutaActual + "\\" + documentoActual.Nombre, rutaNueva + "\\" + documentoActual.Nombre);
                    return true;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return false;
        }

        public async Task<MemoryStream> DescagarDocumento(string rutaDocumento)
        {
            if (!System.IO.File.Exists(rutaDocumento))
                return null;

            MemoryStream memory = new MemoryStream();
            
            using (var stream = new FileStream(rutaDocumento, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            return memory;
        }

        public async Task<bool> EliminarDocumento(string rutaDocumento, string nombreDocumento)
        {
            try
            {               
                File.Delete(System.IO.Path.Combine(rutaDocumento, nombreDocumento));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CopiarDocumentosExpediente(int idExpedienteOrigen, int idExpedienteDestino)
        {
            try
            {
                string directorioExpedienteOrigen = Path.Combine(AppDirectory, idExpedienteOrigen.ToString());
                string directorioExpedienteDestino = Path.Combine(AppDirectory, idExpedienteDestino.ToString());

                Copiar(directorioExpedienteOrigen, directorioExpedienteDestino);                                 

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al copiar los documentos.", ex);
            }
        }

        private void Copiar(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

            foreach (var directory in Directory.GetDirectories(sourceDir))
                Copiar(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }
    }
}
