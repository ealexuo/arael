
namespace Qfile.Core.Modelos
{
    public class ValorListaModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdPlantilla { get; set; }
        public int IdSeccion { get; set; }
        public int IdCampo { get; set; }
        public int IdValor { get; set; }
        public int Orden { get; set; }
        public int IdCampoPadre { get; set; }
        public int IdValorPadre { get; set; }
        public string Nombre { get; set; }
        public bool Predeterminado { get; set; }
    }
}
