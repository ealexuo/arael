using System;
using System.Collections.Generic;
using System.Text;

namespace Qfile.Core.Modelos
{
    public class ExpedienteSeccionDatosModelo
    {
        public int IdExpediente { get; set; }
        public int IdPlantilla { get; set; }
        public int IdSeccion { get; set; }
        public string Nombre { get; set; }
        public bool Activa { get; set; }
        public List<ExpedienteCampoDatosModelo> ListaCampos { get; set; }
    }

    public class ExpedienteCampoDatosModelo
    {
        public int IdCampo { get; set; }
        public int Orden { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Longitud { get; set; }
        public bool Obligatorio { get; set; }
        public int NoColumnas { get; set; }
        public int IdCampoPadre { get; set; }
        public string Valor { get; set; }
        public int IdTipoCampo { get; set; }
        public bool Activo { get; set; }
    }
}
