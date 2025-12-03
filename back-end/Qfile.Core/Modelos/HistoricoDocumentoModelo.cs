using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class HistoricoDocumentoModelo
    {
        public int IdHistoricoDocumento { get; set; }
        public int IdDocumentoReemplazado { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string Extension { get; set; }
        public int IdIntegracion { get; set; }
        public int IdUsuarioCarga { get; set; }
        public int IdEntidadCarga { get; set; }
        public int IdUsuarioModificacion { get; set; }
        public int IdEntidadModificacion { get; set; }
        public string NombreUsuarioCarga { get; set; }
        public string NombreUsuarioModificacion { get; set; }
        public DateTime FechaCarga { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string Observaciones { get; set; }
        public int IdExpediente { get; set; }
    }
}
