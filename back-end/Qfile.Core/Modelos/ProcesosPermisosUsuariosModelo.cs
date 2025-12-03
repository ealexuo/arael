using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class ProcesosPermisosUsuariosModelo // Mapeo de la entidad de base de datos ProcesosPermisosUsuarios
    {
        public int IdProcesoEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdUsuarioEntidad { get; set; }
        public int IdPermiso { get; set; }
        public int IdUsuario { get; set; }
    }
}
