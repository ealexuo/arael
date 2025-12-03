using Microsoft.AspNetCore.Http;
using Qfile.Core.Constantes;
using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using Qfile.Core.Servicios.Documentos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios.Documentos
{
    public class DocumentoServicio : IDocumentoServicio
    {
        private readonly IDocumentoDatos _datos;
        private IDictionary<int, string> Integraciones;
        
        public DocumentoServicio(IDocumentoDatos datos)
        {
            _datos = datos;
            cargarDiccionarioIntegraciones();
        }
        
        public async Task<int> GuardarDocumentoAsync(IFormFile documento, int idUsuario, int IdEntidad, int idExpediente)
        {
            if(documento != null)
            {
                int idDocumento = 0;
                int idIntegracionSeleccionada = this.ObtenerIntegracionSeleccionada();
                string integracionSeleccionada = Integraciones[idIntegracionSeleccionada];

                var IntegracionServicio = this.ActivarIntegracion(integracionSeleccionada);
                               
                string resultado = await IntegracionServicio.GuardarDocumento(documento, idExpediente);

                var arrayRuta = resultado.Split('\\');
                var ruta="";
                for (int i = 0; i < arrayRuta.Length -1; i++)
                {
                    ruta = ruta + arrayRuta[i] + "\\";
                }
;
                if (!String.IsNullOrEmpty(resultado))
                {
                    DocumentoModelo documentoModelo = new DocumentoModelo
                    {
                        IdDocumento = idDocumento,  
                        IdExpediente = idExpediente,
                        Nombre = documento.FileName,
                        Ruta = ruta,
                        Extension = Path.GetExtension(documento.FileName).Replace(".",""),
                        IdIntegracion = idIntegracionSeleccionada,
                        IdUsuarioCarga = idUsuario,
                        IdEntidadCarga = IdEntidad,
                        IdUsuarioModificacion = idUsuario,
                        IdEntidadModificacion = IdEntidad,
                        FechaCarga = UtilidadesServicio.FechaActualUtc,
                        FechaModificacion = UtilidadesServicio.FechaActualUtc,
                        Observaciones = "",
                        IdAlmacenamiento = 1, // ****** debe ser acorde al almacenamiento definido por el usuario *****
                        IdDocumentoGuid = null
                    };

                    idDocumento = await _datos.GuardarDocumentoAsync(documentoModelo);
                }

                return idDocumento;
            }
            else
            {
                throw new Exception("No existe documento para guardar.");
            }            
        }

        public async Task<int> ReemplazarDocumentoAsync(IFormFile documento, int idUsuario, int idEntidad, int idExpediente, int idDocumento, string observaciones)
        {
            // activar integración
            int idIntegracionSeleccionada = this.ObtenerIntegracionSeleccionada();
            string integracionSeleccionada = Integraciones[idIntegracionSeleccionada];
            var IntegracionServicio = this.ActivarIntegracion(integracionSeleccionada);

            // obtener documento actual
            DocumentoModelo documentoActual = await this._datos.ObtenerDocumentoPorIdAsync(idDocumento);

            // guardar histórico
            documentoActual.Observaciones = observaciones;
            string nuevaRutaHistorico = await this._datos.GuardarHistoricoDocumentoAsync(documentoActual);
            
            // mover documento actual a nueva ruta de histórico
            bool moverArchivoExito = await IntegracionServicio.MoverArchivo(documentoActual, documentoActual.Ruta, nuevaRutaHistorico);

            // actualizar el nuevo documento
            await IntegracionServicio.GuardarDocumento(documento, idExpediente);

            if (moverArchivoExito)
            {
                documentoActual.Nombre = documento.FileName;
                documentoActual.Extension = Path.GetExtension(documento.FileName).Replace(".", "");
                documentoActual.IdIntegracion = idIntegracionSeleccionada;
                documentoActual.IdUsuarioCarga = idUsuario;
                documentoActual.IdEntidadCarga = idEntidad;
                documentoActual.IdUsuarioModificacion = idUsuario;
                documentoActual.IdEntidadModificacion = idEntidad;
                documentoActual.FechaCarga = UtilidadesServicio.FechaActualUtc;
                documentoActual.FechaModificacion = UtilidadesServicio.FechaActualUtc;
                documentoActual.Observaciones = "";
                documentoActual.IdDocumentoGuid = null;

                idDocumento = await _datos.ActualizarDocumentoAsync(documentoActual);
            }

            return idDocumento;
        }

        public async Task<DocumentoModelo> ObtenerDocumentoAsync(int idExpediente, string nombreDocumento)
        {
            return await this._datos.ObtenerDocumentoAsync(idExpediente, nombreDocumento);
        }

        public async Task<DocumentoModelo> ObtenerDocumentoPorIdAsync(int idDocumento)
        {
            return await this._datos.ObtenerDocumentoPorIdAsync(idDocumento);
        }

        public async Task<List<DocumentoModelo>> ObtenerDocumentosAsync(int idExpediente)
        {
            return await this._datos.ObtenerDocumentosAsync(idExpediente);
        }

        public async Task<List<HistoricoDocumentoModelo>> ObtenerHistoricoDocumentoAsync(int idDocumentoReemplazado)
        {
            return await this._datos.ObtenerHistoricoDocumentoAsync(idDocumentoReemplazado);
        }

        public async Task<MemoryStream> DescargarDocumento(int idDocumento, bool esDocumentoActual)
        {
            // activar integración
            int idIntegracionSeleccionada = this.ObtenerIntegracionSeleccionada();
            string integracionSeleccionada = Integraciones[idIntegracionSeleccionada];
            var IntegracionServicio = this.ActivarIntegracion(integracionSeleccionada);


            // obtener rutaDocumento
            string ruta, nombre;
            
            if (esDocumentoActual)
            {
                DocumentoModelo documentoModelo = await this._datos.ObtenerDocumentoPorIdAsync(idDocumento);
                ruta = documentoModelo.Ruta + documentoModelo.IdExpediente;
                nombre = documentoModelo.Nombre;
            }
            else
            {
                HistoricoDocumentoModelo documentoModelo = await this._datos.ObtenerDocumentoHistoricoPorIdAsync(idDocumento);
                ruta = documentoModelo.Ruta;
                nombre = documentoModelo.Nombre;
            }

            // obtener documento
            return await IntegracionServicio.DescagarDocumento(System.IO.Path.Combine(ruta, nombre));
        }

        public async Task<bool> EliminarDocumentoAsync(int idDocumento)
        {
            int idIntegracionSeleccionada = this.ObtenerIntegracionSeleccionada();
            string integracionSeleccionada = Integraciones[idIntegracionSeleccionada];
            var IntegracionServicio = this.ActivarIntegracion(integracionSeleccionada);

            DocumentoModelo documentoModelo = await this._datos.ObtenerDocumentoPorIdAsync(idDocumento);

            bool resultado = await IntegracionServicio.EliminarDocumento(documentoModelo.Ruta, documentoModelo.Nombre);

            if (resultado)
                return await _datos.EliminarDocumentoAsync(idDocumento);
            else
                return false;
        }

        public async Task<bool> CopiarDocumentosExpediente(int idExpedienteOrigen, int idExpedienteDestino)
        {
            int idIntegracionSeleccionada = this.ObtenerIntegracionSeleccionada();
            string integracionSeleccionada = Integraciones[idIntegracionSeleccionada];

            var IntegracionServicio = this.ActivarIntegracion(integracionSeleccionada);

            return await IntegracionServicio.CopiarDocumentosExpediente(idExpedienteOrigen, idExpedienteDestino);            
        }

        private IDocumentoIntegracionServicio ActivarIntegracion(string integracionSeleccionada)
        {
            try
            {
                IDocumentoIntegracionServicio docService;
                docService = (IDocumentoIntegracionServicio)Activator.CreateInstance(Type.GetType("Qfile.Core.Servicios.Documentos." + integracionSeleccionada));
                return docService;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al tratar de activar la integración de documentos.");
            }
        }

        private void cargarDiccionarioIntegraciones()
        {
            Integraciones = new Dictionary<int, string>();
            Integraciones.Add(0, "RutaServicio");
            Integraciones.Add(1, "AmazonS3Servicio");
            Integraciones.Add(2, "DropboxServicio");
        }

        private int ObtenerIntegracionSeleccionada()
        {
            // TODO: obtener integración seleccionada, por el momento default Ruta
            return 0;
        }
    }
}
