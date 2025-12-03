using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qfile.Core.Modelos
{
    public class LoginModelo
    {
        public int idUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public bool RequiereCambioPassword { get; set; }
    }
}
