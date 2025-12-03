
using System.Collections.Generic;

namespace Qfile.Core.Modelos
{
    public class SeccionModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdPlantilla { get; set; }
        public int IdSeccion { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
        public bool Activa { get; set; }
        public List<CampoModelo> ListaCampos { get; set; }

    }
}
