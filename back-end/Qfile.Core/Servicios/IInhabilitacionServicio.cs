using Qfile.Core.Modelos;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public interface IInhabilitacionServicio
    {
        Task<Result<string>> GuardarInhabilitacionAsync(HistoricoInhabilitacionModelo inhabilitacion);
        Task <Result<string>> ActualizarInhabilitacionAsync(HistoricoInhabilitacionModelo inhabilitacion);
        Task<FechasInhabilitacionUsuarioModelo> ObtenerFechasInhabilitacionUsuarioAsync(int IdUsuario);
        Task<bool> EsUsuarioInhabilitadoAsync(int IdUsuario);
        Task<List<HistoricoInhabilitacionModelo>> ObtenerInhabilitacionesUsuarioAsync(int idEntidad, int idUsuario);
        Task<bool> EliminarInhabilitacionAsync(int idEntidad, int idUsuario, int idInhabilitacion);

    }
}
