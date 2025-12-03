using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public interface IReportesServicio
    {
        Task<byte[]> ImprimirRequisitosGestion(int idExpediente, List<ExpedienteRequisitosModelo> requisitos);
        Task<byte[]> ImprimirCedula(int idExpedientem, CedulaExpedienteModelo cedulaExpediente, int idUsuario, int idEntidad);
    }
}
