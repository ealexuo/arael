
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace Qfile.Core.Modelos
{
    public class FaseTrasladoModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdFase { get; set; }
        public string Fase { get; set; }
        public bool AsignacionObligatoria { get; set; }
        public bool AcuseRecibido { get; set; }
        public int IdTipoFase { get; set; }
        public string TipoFase { get; set; }
        public List<UsuarioFase> ListaUsuarios { get; set; }
    }

    public class UsuarioFase
    {
        public int idUsuario { get; set; }
        public string Usuario { get; set; }
    }
}
