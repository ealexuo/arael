
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace Qfile.Core.Modelos
{
    public class FaseModelo
    {
        public int IdEntidad { get; set; }
        public int IdProceso { get; set; }
        public int IdFase { get; set; }
        public int IdTipoFase { get; set; }
        public string TipoFase { get; set; }
        public int IdUnidadAdministrativa { get; set; }
        public string UnidadAdministrativa { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal TiempoPromedio { get; set; }
        public int IdUnidadMedida { get; set; }
        public string UnidadMedida { get; set; }
        public bool AsignacionObligatoria { get; set; }
        public bool Activa { get; set; }
        public bool AcuseRecibido { get; set; }
        public int IdTipoAcceso { get; set; }
        public string TipoAcceso { get; set; }
        public int UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public List<TransicionesModelo> ListaTransiciones { get; set; }
    }
}
