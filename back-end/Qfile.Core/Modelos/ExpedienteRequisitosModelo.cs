using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class ExpedienteRequisitosModelo
    {
        public int IdExpediente { get; set; }
        public int idRequisito { get; set; }
        public string Requisito { get; set; }
        public bool Obligatorio { get; set; }
        public bool Presentado { get; set; }
        public string Observacion { get; set; }
        public int IdEntidad { get; set; }
        public string Entidad { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdUsuarioRegistro { get; set; }
    }
}
