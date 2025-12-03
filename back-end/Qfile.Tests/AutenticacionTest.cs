using Moq;
using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using Qfile.Core.Servicios;
using System;
using Xunit;


namespace Qfile.Tests
{
    public class AutenticacionTest
    {
        Mock<IAutenticacionDatos> datos;
        Mock<IUsuarioDatos> usuarioDatos;
        AutenticacionServicio servicio;

        public AutenticacionTest()
        {
            datos = new Mock<IAutenticacionDatos>();
            usuarioDatos = new Mock<IUsuarioDatos>();
            servicio = new AutenticacionServicio(datos.Object, usuarioDatos.Object, null, null);
        }

        [Fact]
        public void RegistrarLogeo()
        {
            // Arrange
            int idEntidad = 1;
            int idUsuario = 1;

            datos.Setup(ud => ud.RegistrarLogeo(idEntidad, idUsuario, It.IsAny<DateTime>())).ReturnsAsync(1);

            // Act
            int resultado = servicio.RegistrarLogeo(idEntidad, idUsuario).Result;

            // Assert
            Assert.True(resultado == 1);
        }
    }
}
