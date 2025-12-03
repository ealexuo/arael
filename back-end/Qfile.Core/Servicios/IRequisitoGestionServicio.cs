using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public interface IRequisitoGestionServicio
    {
        Task<List<RequisitoGestionModelo>> ObtenerRequisitosAsync(int idEntidad, int idProceso);
        Task<int> CrearRequisitoAsync(RequisitoGestionModelo requisito);
        Task<int> ActualizarRequisitoAsync(RequisitoGestionModelo requisito);
        Task<int> EliminarRequisitoAsync(int idEntidad, int idProceso, int idRequisito);
    }
}
