using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class ExpedienteModelo
    {
        public int IdExpediente { get; set; }
        public string Emisor { get; set; }
        public string Descripcion { get; set; }
        public int IdOrigen { get; set; }
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public DateTime? FechaTraslado { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public int? IdUsuarioAsignado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public int IdFaseActual { get; set; }
        public int IdTipoOperacion { get; set; }
        public bool PlantillaHabilitada { get; set; }
    }
}
