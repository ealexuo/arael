using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class TransicionNotificacionModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdFaseOrigen { get; set; }      
        public string FaseOrigen { get; set; }
        public int IdFaseDestino { get; set; }
        public string FaseDestino { get; set; }
        public string Correo { get; set; }
        public int UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
