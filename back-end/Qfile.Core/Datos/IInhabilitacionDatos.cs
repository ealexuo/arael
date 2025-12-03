using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Datos
{
    public interface IInhabilitacionDatos
    {
        Task<int> GuardarInhabilitacionAsync(HistoricoInhabilitacionModelo inhabilitacion);
        Task<int> ActualizarInhabilitacionAsync(HistoricoInhabilitacionModelo inhabilitacion);
        Task<FechasInhabilitacionUsuarioModelo> ObtenerFechasInhabilitacionUsuarioAsync(int IdUsuario);
        Task<List<HistoricoInhabilitacionModelo>> ObtenerInhabilitacionesUsuarioAsync(int idEntidad, int IdUsuario);
        Task<bool> EliminarInhabilitacionAsync(int idEntidad, int idUsuario, int idInhabilitacion);
        Task<bool> VerificarTraslapeDeFechaAsync(int idEntidad, int IdUsuario, DateTime Fecha, int idHistoricoInhabilitacion);

    }
}
