using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class UsuarioModelo
    {
        public int IdUsuario { get; set; }
        public int IdEntidad { get; set; }
        public string NoIdentificacionPersonal { get; set; }
        public string CorreoElectronico { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string OtrosNombres { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string ApellidoCasada { get; set; }
        public string Titulo { get; set; }
        public string Cargo { get; set; }
        public string Extension { get; set; }
        public string Telefono { get; set; }
        public int Genero { get; set; }
        public int IdUnidadAdministrativa { get; set; }
        public int IdIdioma { get; set; }
        public bool Activo { get; set; }
        public List<int> ListaRoles { get; set; }
    }
}
