using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Qfile.Core.Datos;
using ServiceResult;

namespace Qfile.Core.Servicios
{
    public class InhabiliacionServicio : IInhabilitacionServicio
    {
        private readonly IInhabilitacionDatos _datos;

        public InhabiliacionServicio(IInhabilitacionDatos datos)
        {
            _datos = datos;
        }
        
        public async Task<Result<string>> GuardarInhabilitacionAsync(HistoricoInhabilitacionModelo inhabilitacion)
        {
            // validacion de fechas
            if (inhabilitacion.FechaInicio > inhabilitacion.FechaFin)
            {
                return new InvalidResult<string>("La fecha fecha final debe ser mayor a la fecha inicial.");
            }

            if (await VerificarTraslapeDeFechaAsync(inhabilitacion.IdEntidad, inhabilitacion.IdUsuario, inhabilitacion.FechaInicio, inhabilitacion.IdHistoricoInhabilitacion))                 
            {
                return new InvalidResult<string>("Existe un traslape entre la fecha inicial y un registro de inhabilitación ingresado anteriormente.");
            }
            
            if (await VerificarTraslapeDeFechaAsync(inhabilitacion.IdEntidad, inhabilitacion.IdUsuario, inhabilitacion.FechaFin, inhabilitacion.IdHistoricoInhabilitacion))
            {
                return new InvalidResult<string>("Existe un traslape entre la fecha final y un registro de inhabilitación ingresado anteriormente.");
            }

            await _datos.GuardarInhabilitacionAsync(inhabilitacion);
            
            return new SuccessResult<string>("");        
        }

        public async Task<Result<string>> ActualizarInhabilitacionAsync(HistoricoInhabilitacionModelo inhabilitacion)
        {
            // validacion de fechas
            if (inhabilitacion.FechaInicio > inhabilitacion.FechaFin)
            {
                return new InvalidResult<string>("La fecha fecha final debe ser mayor a la fecha inicial.");
            }
            
            if(await VerificarTraslapeDeFechaAsync(inhabilitacion.IdEntidad, inhabilitacion.IdUsuario, inhabilitacion.FechaInicio, inhabilitacion.IdHistoricoInhabilitacion))
            {
                return new InvalidResult<string>("Existe un traslape entre la fecha inicial y un registro de inhabilitación ingresado anteriormente.");
            }

            if (await VerificarTraslapeDeFechaAsync(inhabilitacion.IdEntidad, inhabilitacion.IdUsuario, inhabilitacion.FechaFin, inhabilitacion.IdHistoricoInhabilitacion))
            {
                return new InvalidResult<string>("Existe un traslape entre la fecha final y un registro de inhabilitación ingresado anteriormente.");
            }

            var result =  await _datos.ActualizarInhabilitacionAsync(inhabilitacion);

            return new SuccessResult<string>(result.ToString());
        }

        public async Task<bool> VerificarTraslapeDeFechaAsync(int idEntidad, int IdUsuario, DateTime Fecha, int idHistoricoInhabilitacion)
        {
            return  await _datos.VerificarTraslapeDeFechaAsync(idEntidad, IdUsuario, Fecha, idHistoricoInhabilitacion);           
        }

        public async Task<FechasInhabilitacionUsuarioModelo> ObtenerFechasInhabilitacionUsuarioAsync(int IdUsuario)
        {
            FechasInhabilitacionUsuarioModelo inhabilitacion = await _datos.ObtenerFechasInhabilitacionUsuarioAsync(IdUsuario);
            DateTime fechaActual = UtilidadesServicio.FechaActualServidor;

            if (inhabilitacion != null && (fechaActual >= inhabilitacion.FechaInicio && fechaActual < inhabilitacion.FechaFin))
                return inhabilitacion;

            return null;
        }

        public async Task<bool> EsUsuarioInhabilitadoAsync(int IdUsuario)
        {
            FechasInhabilitacionUsuarioModelo inhabilitacion = await this.ObtenerFechasInhabilitacionUsuarioAsync(IdUsuario);
            return inhabilitacion == null;
        }

        public async Task<List<HistoricoInhabilitacionModelo>> ObtenerInhabilitacionesUsuarioAsync(int idEntidad, int idUsuario)
        {
            try
            {
                return await _datos.ObtenerInhabilitacionesUsuarioAsync(idEntidad, idUsuario);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> EliminarInhabilitacionAsync(int idEntidad, int idUsuario, int idInhabilitacion)
        {
            return await _datos.EliminarInhabilitacionAsync(idEntidad, idUsuario, idInhabilitacion);
        }
    }
}
