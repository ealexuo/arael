using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public interface ICorreoElectronicoServicio
    {
        Task<bool> Enviar(string para, string asunto, string mensaje);
    }
}
