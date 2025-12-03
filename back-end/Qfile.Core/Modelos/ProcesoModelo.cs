
namespace Qfile.Core.Modelos
{
    public class ProcesoModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Color { get; set; }
        public int IdTipoProceso { get; set; }
        public string TipoProceso { get; set; }
        public int IdUnidadAdministrativa { get; set; }
        public string UnidadAdministrativa { get; set; }
        public string SiglasUA { get; set; }
        public bool Estado { get; set; }
        public int expedientesActivos { get; set; }
        public int expedientesFinalizados { get; set; }
        public int totalExpedientes { get; set; }
    }
}
