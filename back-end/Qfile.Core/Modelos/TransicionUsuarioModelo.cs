using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class TransicionUsuarioModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdFaseOrigen { get; set; }      
        public int IdFaseDestino { get; set; }
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public int UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
