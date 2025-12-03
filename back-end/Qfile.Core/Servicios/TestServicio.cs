using Qfile.Core.Datos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Qfile.Core.Servicios
{
    public class TestServicio : ITestServicio
    {
        private readonly ITestDatos _datos;
        public TestServicio(ITestDatos datos)
        {
            _datos = datos;
        }

        public Task<string> GetTestAsync()
        {
            return _datos.GetTestAsync();
        }

        public Task<string> ReadinessProbe()
        {
            return _datos.ReadinessProbe();
        }


    }

}
