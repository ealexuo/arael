using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Qfile.Core.Datos;

namespace Qfile.Core.Servicios
{
    public class EntidadServicio : IEntidadServicio
    {
        private readonly IEntidadDatos _datos;

        public EntidadServicio(IEntidadDatos datos)
        {
            _datos = datos;
        }

        public async Task<EntidadModelo> ObtenerEntidadAsync(int idEntidad)
        {
            return await _datos.ObtenerEntidadAsync(idEntidad);
        }      

    }
}
