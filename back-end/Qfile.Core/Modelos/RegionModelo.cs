using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class RegionModelo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int? RegionPadre { get; set; }
    }
}
