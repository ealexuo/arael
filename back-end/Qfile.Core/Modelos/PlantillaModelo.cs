
using System.Collections.Generic;

namespace Qfile.Core.Modelos
{
    public class PlantillaModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdPlantilla { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Version { get; set; }
        public decimal VersionPropuesta { get; set; }
        public bool Activa { get; set; }
        public List<SeccionModelo> ListaSecciones { get; set; }
    }
}
