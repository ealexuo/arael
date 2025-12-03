using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class CambiarPasswordModelo
    {
        public string NombreUsuario { get; set; }
        public string PasswordActual { get; set; }
        public string PasswordNuevo { get; set; }
    }
}
