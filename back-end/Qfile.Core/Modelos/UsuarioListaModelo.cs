using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class UsuarioListaModelo
    {
        public int IdEntidad { get; set; }
        public int IdUsuario { get; set; }
        public string NoIdentificacionPersonal { get; set; }
        public string CorreoElectronico { get; set; }
        public string Nombre { get; set; }
        public string NombreUnidadAdministrativa { get; set; }
        public bool Estado { get; set; }
        public int CantidadTotal { get; set; }
        public FechasInhabilitacionUsuarioModelo FechasInhabilitacion { get; set; }
    }
}
