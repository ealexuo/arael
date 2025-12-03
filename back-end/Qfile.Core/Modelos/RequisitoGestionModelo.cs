using System;

namespace Qfile.Core.Modelos
{
    public class RequisitoGestionModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdRequisito { get; set;  }
        public string Requisito { get; set; }
        public bool Obligatorio { get; set; }
        public int UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
