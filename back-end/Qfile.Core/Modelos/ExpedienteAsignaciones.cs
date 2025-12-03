using System;

namespace Qfile.Core.Modelos
{
    public class ExpedienteAsignaciones
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdExpediente { get; set; }
        public int IdFase { get; set; }
        public DateTime FechaTraslado { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public string Observacion { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public int IdUsuarioAsignado { get; set; }
        public string UsuarioAsignado { get; set; }
        public DateTime FechaLimiteAtencion { get; set; }
        public int idTipoOperacion { get; set; }
        public string TipoOperacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
