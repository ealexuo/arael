using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Integraciones.RutaDocumento.RutaDocumento
{
    public class RutaDocumentoAdaptador
    {
        private readonly IConnectionProvider connectionProvider;

        public RutaDocumentoAdaptador(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }        
    }
}
