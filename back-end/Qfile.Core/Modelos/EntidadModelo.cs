using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class EntidadModelo
    {
        public int Id { get; set; } 
        public string LlaveProducto { get; set; }
        public string NIT { get; set; }
        public int CodigoInstitucional { get; set; }
        public string NombreComercial { get; set; }        
        public string Direccion { get; set; }
        public string PBX { get; set; }
        public string CorreoElectronico { get; set; }
        public string PaginaWeb { get; set; }
        public string Slogan { get; set; }
        public int IdRegion { get; set; }
        public int IdIdioma { get; set; }
    }
}
