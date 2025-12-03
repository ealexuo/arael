
namespace Qfile.Core.Modelos
{
    public class CampoModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdPlantilla { get; set; }
        public int IdSeccion { get; set; }
        public int IdCampo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion{ get; set; }
        public int Orden { get; set; }
        public int Longitud { get; set; }
        public bool Obligatorio { get; set; }
        public int NoColumnas { get; set; }
        public int IdTipoCampo { get; set; }
        public bool Activo { get; set; }
        public int IdCampoPadre { get; set; }
        public string NombreCampoPadre { get; set; }
    }
}
