using Dapper;
using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Datos
{
    public class EntidadDatos : IEntidadDatos
    {
        private readonly IConnectionProvider connectionProvider;

        public EntidadDatos(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }
        
        public async Task<EntidadModelo> ObtenerEntidadAsync(int idEntidad)
        {
            using (var connection = await connectionProvider.OpenAsync())
            {
                string sqlQuery = @"SELECT 
	                                    ID_ENTIDAD Id, 
	                                    LLAVE_PRODUCTO LlaveProducto, 
	                                    NIT, 
	                                    CODIGO_INSTITUCIONAL CodigoInstitucional, 
	                                    NOMBRE_COMERCIAL NombreComercial, 
	                                    DIRECCION, 
	                                    PBX, 
	                                    CORREO_ELECTRONICO CorreoElectronico, 
	                                    PAGINA_WEB PaginaWeb, 
	                                    SLOGAN, 
	                                    ID_REGION IdRegion, 
	                                    ID_IDIOMA IdIdioma
                                    FROM QFILE.AD_ENTIDADES
                                    WHERE ID_ENTIDAD = :IdEntidad";

                var result = await connection.QueryAsync<EntidadModelo>(sqlQuery, new { IdEntidad = idEntidad });

                return result.FirstOrDefault();
            }
        }

    }
}
