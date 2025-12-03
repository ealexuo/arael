
using System;
using System.Collections.Generic;

namespace Qfile.Core.Modelos
{
    public class HistoricoPlantillasModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdPlantilla { get; set; }
        public decimal Version { get; set; }
        public DateTime fechaPublicacion { get; set; }
    }
}
