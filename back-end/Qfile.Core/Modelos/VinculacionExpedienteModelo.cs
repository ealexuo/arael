using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class VinculacionExpedienteModelo
    {
        public int IdExpediente { get; set; }
        public int IdExpedienteVinculado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdEntidad { get; set; }
        public int IdUsuarioRegistro { get; set; }
    }
}
