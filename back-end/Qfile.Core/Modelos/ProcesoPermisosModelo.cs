using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class ProcesoPermisosModelo
    {
        public ProcesoModelo Proceso { get; set; }
        public List<ProcesoPermisoModelo> ListaPermisos { get; set; }
    }
}
