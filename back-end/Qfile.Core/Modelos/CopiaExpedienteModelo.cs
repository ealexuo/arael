using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class CopiaExpedienteModelo
    {
        public int IdExpediente { get; set; }
        public int NumeroCopias { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdEntidad { get; set; }
        public int IdUsuarioRegistro { get; set; }
    }
}
