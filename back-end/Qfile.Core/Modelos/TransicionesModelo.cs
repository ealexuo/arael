using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class TransicionesModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdFaseOrigen { get; set; }
        public string FaseOrigen { get; set; }
        public string UnidadAdministrativaFO { get; set; }
        public int IdFaseDestino { get; set; }
        public string FaseDestino { get; set; }
        public string UnidadAdministrativaFD { get; set; }
        public bool Activa { get; set; }        
        public int UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
