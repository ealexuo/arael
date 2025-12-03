using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class OrigenModelo
    {
        public int IdOrigen { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdEntidad { get; set; }
        public int IdUsuarioRegistro { get; set; }

    }
}
