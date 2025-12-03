using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class HistoricoInhabilitacionModelo
    {
        public int IdHistoricoInhabilitacion { get; set; }
        public int IdUsuario { get; set; }
        public int IdEntidad { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int UsuarioRegistro { get; set; }
    }
}
