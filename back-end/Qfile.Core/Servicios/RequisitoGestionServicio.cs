using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using Qfile.Core.Tipos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public class RequisitoGestionServicio : IRequisitoGestionServicio
    {
        private readonly IRequisitoCreacionDatos _datos;

        public RequisitoGestionServicio(IRequisitoCreacionDatos datos)
        {
            _datos = datos;
        }
        public async Task<List<RequisitoGestionModelo>> ObtenerRequisitosAsync(int idEntidad, int idProceso)
        {
            try
            {
                return await _datos.ObtenerRequisitosAsync(idEntidad, idProceso);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> CrearRequisitoAsync(RequisitoGestionModelo requisito)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;
                requisito.FechaRegistro = fechaRegistro;

                return await _datos.CrearRequisitoAsync(requisito);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<int> ActualizarRequisitoAsync(RequisitoGestionModelo requisito)
        {
            try
            {
                DateTime fechaRegistro = UtilidadesServicio.FechaActualUtc;

                return await _datos.ActualizarRequisitoAsync(requisito);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<int> EliminarRequisitoAsync(int idEntidad, int idProceso, int idRequisito)
        {
            try
            {
                // PENDIETNE REALIZAR VALIDACIONEs

                return await _datos.EliminarRequisitoAsync(idEntidad, idProceso, idRequisito);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
