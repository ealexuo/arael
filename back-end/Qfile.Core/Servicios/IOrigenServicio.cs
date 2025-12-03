using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public interface IOrigenServicio
    {
        Task<List<OrigenModelo>> ObtenerOrigenesAsync();
        Task<OrigenModelo> ObtenerOrigenAsync(int idOrigen);
        Task<int> CrearOrigenAsync(OrigenModelo origen, int idUsuarioRegistro, int idEntidad);
        Task<bool> ActualizarOrigenAsync(OrigenModelo origen, int idUsuarioRegistro, int idEntidad);
        Task<bool> EliminarOrigenAsync(int idOrigen);

    }
}
