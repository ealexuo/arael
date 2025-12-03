using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public interface IEntidadServicio
    {
        Task<EntidadModelo> ObtenerEntidadAsync(int idEntidad);
    }
}
