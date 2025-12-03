using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class RequisitoPorTransicionModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdFaseOrigen { get; set; }
        public int IdFaseDestino { get; set; }
        public int IdRequisito { get; set; }
        public int IdTipoCampo { get; set; }
        public string NombreTipoCampo { get; set; }
        public string Campo { get; set; }
        public bool Obligatorio { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int UsuarioRegistro { get; set; }
    }
}
