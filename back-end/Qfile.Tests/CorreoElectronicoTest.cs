using Moq;
using Qfile.Core.Datos;
using Qfile.Core.Modelos;
using Qfile.Core.Servicios;
using System;
using Xunit;

namespace Qfile.Tests
{
    public class CorreoElectronicoTest
    {
        ICorreoElectronicoServicio servicio;

        public CorreoElectronicoTest()
        {
            servicio = new CorreoElectronicoGmailServicio();
        }

        [Fact]
        public void Enviar()
        {
            // Arrange
            string para = "ealexander.uo@gmail.com";
            string asunto = "Prueba de correo";
            string mensaje = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.";

            // Act
            bool resultado = servicio.Enviar(para, asunto, mensaje).Result;

            // Assert
            Assert.True(resultado);
        }

    }
}
