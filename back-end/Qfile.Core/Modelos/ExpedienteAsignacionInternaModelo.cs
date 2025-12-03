using System;

namespace Qfile.Core.Modelos
{
    public class ExpedienteAsignacionInternaModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdExpediente { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public string Observacion { get; set; }
        public int IdUsuarioRegistro { get; set; }
        public string UsuarioRegistro { get; set; }
        public DateTime FechaLimiteAtencion { get; set; }
    }
}
