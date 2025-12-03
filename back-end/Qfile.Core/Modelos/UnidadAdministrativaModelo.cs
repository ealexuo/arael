using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class UnidadAdministrativaModelo
    {
        public int IdEntidad { get; set; }
        public int IdUnidadAdministrativa { get; set; }
        public string Nombre { get; set; }
        public bool Activa { get; set; }
        public string Siglas { get; set; }       
        public int? IdUnidadAdministrativaPadre { get; set; }
        public string NombreUnidadAdministrativaPadre { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int IdUsuarioRegistro { get; set; }
    }
}
