using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class ExpedienteAnotacionesModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdExpediente { get; set; }
        public int IdFase { get; set; }
        public DateTime FechaTraslado { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public int idPrivacidad { get; set; }
        public string Privacidad { get; set; }
        public int IdAnotacion { get; set; }
        public string Anotacion { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public string UsuarioRegistro { get; set; }  
        public DateTime FechaRegistro { get; set; }
    }
}
