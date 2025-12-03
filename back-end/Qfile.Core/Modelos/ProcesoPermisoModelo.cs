using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class ProcesoPermisoModelo // Mapeo con la entidad de base de datos ProcesoPermiso
    {
        public int IdPermiso { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Habilitado { get; set; }
    }
}
