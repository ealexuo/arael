using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class AsignacionModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdExpediente { get; set; }
        public int IdFaseDestino { get; set; }
        public DateTime FechaTraslado { get; set; }
        public string Observacion { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public DateTime FechaLimiteAtencion { get; set; }
        public int IdUsuarioAsignado { get; set; }
        public int IdTipoOperacion { get; set; }
        public DateTime FechaOperacion { get; set; }
    }
}
