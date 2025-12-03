using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Qfile.Core.Servicios
{
    public class UnidadAdministrativaServicio : IUnidadAdministrativaServicio
    {
        public readonly IUnidadAdministrativaDatos _datos;

        public UnidadAdministrativaServicio(IUnidadAdministrativaDatos datos)
        {
            _datos = datos;
        }
        
        public async Task<int> CrearUnidadAdministrativaAsync(UnidadAdministrativaModelo unidadAdministrativa)
        {
            try
            {
                if (unidadAdministrativa != null)
                    unidadAdministrativa.FechaCreacion = UtilidadesServicio.FechaActualUtc;

                return await _datos.CrearUnidadAdministrativaAsync(unidadAdministrativa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }            
        }
        public async Task<bool> ActualizarUnidadAdministrativaAsync(UnidadAdministrativaModelo unidadAdministrativa)
        {
            try
            {
                return await _datos.ActualizarUnidadAdministrativaAsync(unidadAdministrativa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            
        }
        public async Task<bool> EliminarUnidadAdministrativaAsync(int idUnidadAdministrativa)
        {
            try
            {
                return await _datos.EliminarUnidadAdministrativaAsync(idUnidadAdministrativa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }           
        }
        public async Task<List<UnidadAdministrativaModelo>> ObtenerUnidadesAdministrativasAsync()
        {           
            try
            {
                return await _datos.ObtenerUnidadesAdministrativasAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<UnidadAdministrativaModelo> ObtenerPorIdAsync(int idUnidadAdministrativa)
        {
            try
            {
                return await _datos.ObtenerPorIdAsync(idUnidadAdministrativa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public async Task<UnidadAdministrativaModelo> ObtenerPorNombreAsync(string nombreUnidadAdministrativa)
        {            
            try
            {
                return await _datos.ObtenerPorNombreAsync(nombreUnidadAdministrativa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }      
    }
}
