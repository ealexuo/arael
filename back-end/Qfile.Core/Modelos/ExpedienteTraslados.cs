using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class ExpedienteTraslados
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdExpediente { get; set; }
        public int IdFaseOrigen { get; set; }
        public string FaseOrigen { get; set; }
        public int IdFaseDestino { get; set; }
        public string FaseDestino { get; set; }
        public DateTime FechaTraslado { get; set; }
        public string ObservacionTraslado { get; set; }
        public int IdUsuarioTraslado { get; set; }
        public string usuarioTraslado { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Tiempo { get; set; }
    }
}
