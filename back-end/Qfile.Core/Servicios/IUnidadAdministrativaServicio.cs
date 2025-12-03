using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public interface IUnidadAdministrativaServicio
    {
        Task<int> CrearUnidadAdministrativaAsync(UnidadAdministrativaModelo unidadAdministrativa);
        Task<bool> EliminarUnidadAdministrativaAsync(int idUnidadAdministrativa);
        Task<bool> ActualizarUnidadAdministrativaAsync(UnidadAdministrativaModelo unidadAdministrativa);
        Task<List<UnidadAdministrativaModelo>> ObtenerUnidadesAdministrativasAsync();
        Task<UnidadAdministrativaModelo> ObtenerPorIdAsync(int idUnidadAdministrativa);
        Task<UnidadAdministrativaModelo> ObtenerPorNombreAsync(string nombreUnidadAdministrativa);
    }
}
