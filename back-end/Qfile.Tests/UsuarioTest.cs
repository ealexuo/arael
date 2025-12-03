using Moq;
using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using Qfile.Core.Servicios;
using System;
using Xunit;

namespace Qfile.Tests
{
    public class UsuarioTest
    {
        Mock<IUsuarioDatos> datos;
        UsuarioServicio servicio;

        public UsuarioTest()
        {
            datos = new Mock<IUsuarioDatos>();
            servicio = new UsuarioServicio(datos.Object, null, null, null);
        }

        [Fact]
        public void EliminarUsuario()
        {
            // Arrange
            int idEntidad = 1;
            int idUsuario = 1;

            datos.Setup(ud => ud.EliminarUsuarioAsync(idEntidad, idUsuario)).ReturnsAsync(true);

            // Act
            bool resultado = servicio.EliminarUsuarioAsync(idEntidad, idUsuario).Result;

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public void ObtenerUsuario()
        {
            // Arrange
            int idUsuario = 1;
            
            datos.Setup(ud => ud.ObtenerUsuarioAsync(idUsuario)).ReturnsAsync(new UsuarioModelo() { 
                IdUsuario = idUsuario
            });

            // Act
            UsuarioModelo resultado = servicio.ObtenerUsuarioAsync(idUsuario).Result;

            // Assert
            Assert.True(resultado.IdUsuario == idUsuario);
        }
    }
}
