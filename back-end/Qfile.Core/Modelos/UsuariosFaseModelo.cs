using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class UsuarioFaseModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdFase { get; set; }        
        public int IdUsuario { get; set; }
        public bool RecepcionTraslado { get; set; }
        public string Nombre { get; set; }
        public int UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
