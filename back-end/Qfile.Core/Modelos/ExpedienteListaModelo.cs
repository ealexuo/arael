using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class ExpedienteListaModelo
    {
        public int IdExpediente { get; set; }
        public int idProceso { get; set; }
        public string Descripcion { get; set; }
        public string NombreOrigen { get; set; }
        public string Emisor { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public DateTime? FechaTraslado { get; set; }
        public string NombreProceso { get; set; }
        public int IdFaseActual { get; set; }
        public int idTipoFase { get; set; }
        public string FaseActualProceso { get; set; }
        public int CantidadTotal { get; set; }
        public string ColorProceso { get; set; }
        public bool ActivoProceso { get; set; }
        public decimal PorcentajeTiempoTranscurrido { get; set; }
        public decimal PorcentajeTiempoInterno { get; set; }
        public int UltimaIdTipoOperacion { get; set; }
        public int IdTipoOperacion { get; set; }
        public int IdUsuarioConsulta { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public int IdUsuarioAsignado { get; set; }
    }
}
