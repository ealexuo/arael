using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Qfile.Core.Datos;

namespace Qfile.Core.Servicios
{
    public class OrigenServicio : IOrigenServicio
    {
        private readonly IOrigenDatos _datos;

        public OrigenServicio(IOrigenDatos datos)
        {
            _datos = datos;
        }

        public async Task<List<OrigenModelo>> ObtenerOrigenesAsync()
        {
            return await _datos.ObtenerOrigenesAsync();
        }
        public async Task<OrigenModelo> ObtenerOrigenAsync(int idOrigen)
        {
            return await _datos.ObtenerOrigenAsync(idOrigen);
        }

        public async Task<int> CrearOrigenAsync(OrigenModelo origen, int idUsuarioRegistro, int idEntidad) 
        {
            return await _datos.CrearOrigenAsync(origen, idUsuarioRegistro, UtilidadesServicio.FechaActualUtc, idEntidad);
        }

        public async Task<bool> ActualizarOrigenAsync(OrigenModelo origen, int idUsuarioRegistro, int idEntidad)
        {
            return await _datos.ActualizarOrigenAsync(origen, idUsuarioRegistro, idEntidad);
        }

        public async Task<bool> EliminarOrigenAsync(int idOrigen)
        {
            return await _datos.EliminarOrigenAsync(idOrigen);
        }

    }
}
