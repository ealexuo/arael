using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class ProcesosPermisosUsuarioModelo
    {
        public List<ProcesoPermisosModelo> ListaProcesosPermisos { get; set; }
        public UsuarioModelo Usuario { get; set; }
    }
}
