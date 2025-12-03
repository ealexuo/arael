using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class ListadoExpedientesVinculadosModelo
    {
        public int IdExpediente { get; set; }
        public string Descripcion { get; set; }
        public string Origen { get; set; }
        public string Emisor { get; set; }
        public int Tipo { get; set; }
    }
}
