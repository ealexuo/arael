using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Datos
{
    public interface IEntidadDatos
    {
        Task<EntidadModelo> ObtenerEntidadAsync(int idEntidad);
    }
}
