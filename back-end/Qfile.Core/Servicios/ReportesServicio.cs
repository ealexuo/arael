using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Qfile.Core.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Qfile.Core.Modelos;
using System.IO;
using iText.Kernel.Events;
using iText.Kernel.Geom;

namespace Qfile.Core.Servicios
{
    public class ReportesServicio : IReportesServicio
    {
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IEntidadServicio _entidadServicio;

        public ReportesServicio(IUsuarioServicio usuarioServicio, IEntidadServicio entidadServicio) 
        {
            _usuarioServicio = usuarioServicio;
            _entidadServicio = entidadServicio;
        }


        public Task<byte[]> ImprimirRequisitosGestion(int idExpediente, List<ExpedienteRequisitosModelo> requisitos)
        {
            // Must have write permissions to the path folder
            var stream = new MemoryStream();
            PdfWriter writer = new PdfWriter(stream);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);

            // Header
            Paragraph header = new Paragraph("REQUISITOS DE GESTIÓN")
               .SetTextAlignment(TextAlignment.CENTER)
               .SetFontSize(20);

            // New line
            Paragraph newline = new Paragraph(new Text("\n"));

            document.Add(newline);
            document.Add(header);

            // Add sub-header
            Paragraph subheader = new Paragraph("Expediente No. " + idExpediente.ToString())
               .SetTextAlignment(TextAlignment.CENTER)
               .SetFontSize(15);
            document.Add(subheader);

            // Line separator
            LineSeparator ls = new LineSeparator(new SolidLine());
            document.Add(ls);

            // Add paragraph1
            Paragraph paragraph1 = new Paragraph(@"Los siguientes requisitos son necesarios para poder darle gestión a su brindarle expediente.");
            document.Add(paragraph1);

            // Table
            Table table = new Table(2, false);
            Cell cellEncabezado1 = new Cell(1, 1)
               .SetBackgroundColor(ColorConstants.GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph("Requisito")); 

            Cell cellEncabezado2 = new Cell(1, 1)
               .SetBackgroundColor(ColorConstants.GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph("Estado"));

            table.AddCell(cellEncabezado1);
            table.AddCell(cellEncabezado2);

            foreach (var requisito in requisitos)
            {
                Cell cellR = new Cell(1, 1)
                .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph(requisito.Requisito));

                Cell cellE = new Cell(1, 1)
               .SetTextAlignment(TextAlignment.CENTER)
              .Add(new Paragraph(requisito.Presentado ? " Presentado ":" Pendiente "));

                table.AddCell(cellR);
                table.AddCell(cellE);
            }

            document.Add(newline);
            document.Add(table);

            //// Hyper link
            //Link link = new Link("click here",
            //   PdfAction.CreateURI("https://www.google.com"));
            //Paragraph hyperLink = new Paragraph("Please ")
            //   .Add(link.SetBold().SetUnderline()
            //   .SetItalic().SetFontColor(ColorConstants.BLUE))
            //   .Add(" to go www.google.com.");

            //document.Add(newline);
            //document.Add(hyperLink);

            // Page numbers
            int n = pdf.GetNumberOfPages();
            for (int i = 1; i <= n; i++)
            {
                document.ShowTextAligned(new Paragraph(String
                   .Format("page" + i + " of " + n)),
                   559, 806, i, TextAlignment.RIGHT,
                   VerticalAlignment.TOP, 0);
            }
            

            var docEvent = document.GetPdfDocument();
            Rectangle pageSize = docEvent.GetPage(1).GetPageSize();

            float coordX = ((pageSize.GetLeft() + document.GetLeftMargin())
                                 + (pageSize.GetRight() - document.GetRightMargin())) / 2;

            float footerY = document.GetBottomMargin();

            Canvas canvas = new Canvas(docEvent.GetPage(1), pageSize);
            canvas
                .SetFontSize(14)
                .ShowTextAligned(requisitos[0].Entidad, coordX, footerY, TextAlignment.CENTER)
                .SetFontSize(10)
                .ShowTextAligned(requisitos[0].Direccion, coordX, footerY-12, TextAlignment.CENTER)
                .Close();


            // Close document
            document.Close();

            return Task.FromResult(stream.ToArray());
        }

        public Task<byte[]> ImprimirCedula(int idExpediente, CedulaExpedienteModelo cedulaExpediente, int idUsuario, int idEntidad)
        {
            UsuarioModelo usuario = _usuarioServicio.ObtenerUsuarioAsync(idUsuario).Result;
            EntidadModelo entidad = _entidadServicio.ObtenerEntidadAsync(idEntidad).Result;

            // Must have write permissions to the path folder
            var stream = new MemoryStream();
            PdfWriter writer = new PdfWriter(stream);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);

            // Header
            Paragraph header = new Paragraph("DATOS GENERALES")
               .SetTextAlignment(TextAlignment.CENTER)
               .SetFontSize(20);

            // New line
            Paragraph newline = new Paragraph(new Text("\n"));
            document.Add(newline);
            document.Add(header);

            // Add sub-header
            Paragraph subheader = new Paragraph("Expediente No. " + idExpediente.ToString())
               .SetTextAlignment(TextAlignment.CENTER)
               .SetFontSize(15);
            document.Add(subheader);

            // Add sub-header
            Paragraph impresion = new Paragraph("Usuario Imprime: " + usuario.PrimerNombre + " " + usuario.SegundoApellido + ", Fecha Impresión: " + UtilidadesServicio.FechaActualServidor)
               .SetTextAlignment(TextAlignment.CENTER)
               .SetFontSize(10);
            document.Add(impresion);

            // Line separator
            LineSeparator ls = new LineSeparator(new SolidLine());
            document.Add(ls);

            // Table
            Table table = new Table(2, false);

            // Proceso
            Cell cellProcesoEtiqueta = new Cell(1, 1)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.LEFT)
               .Add(new Paragraph("Proceso"));

            Cell cellProcesoValor = new Cell(1, 1)
               .SetTextAlignment(TextAlignment.LEFT)
               .Add(new Paragraph(cedulaExpediente.Proceso));

            table.AddCell(cellProcesoEtiqueta);
            table.AddCell(cellProcesoValor);

            // Origen
            Cell cellOrigenEtiqueta = new Cell(1, 1)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.LEFT)
               .Add(new Paragraph("Origen"));

            Cell cellOrigenValor = new Cell(1, 1)
               .SetTextAlignment(TextAlignment.LEFT)
               .Add(new Paragraph(cedulaExpediente.Origen));

            table.AddCell(cellOrigenEtiqueta);
            table.AddCell(cellOrigenValor);

            // Emisor
            Cell cellEmisorEtiqueta = new Cell(1, 1)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.LEFT)
               .Add(new Paragraph("Emisor"));

            Cell cellEmisorValor = new Cell(1, 1)
               .SetTextAlignment(TextAlignment.LEFT)
               .Add(new Paragraph(cedulaExpediente.Emisor));

            table.AddCell(cellEmisorEtiqueta);
            table.AddCell(cellEmisorValor);

            // Descripción
            Cell cellDescripcionEtiqueta = new Cell(1, 1)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.LEFT)
               .Add(new Paragraph("Descripción"));

            Cell cellDescripcionValor = new Cell(1, 1)
               .SetTextAlignment(TextAlignment.LEFT)
               .Add(new Paragraph(cedulaExpediente.Descripcion));

            table.AddCell(cellDescripcionEtiqueta);
            table.AddCell(cellDescripcionValor);

            document.Add(newline);
            document.Add(table);

            // Page numbers
            int n = pdf.GetNumberOfPages();
            for (int i = 1; i <= n; i++)
            {
                document.ShowTextAligned(new Paragraph(String
                   .Format("page" + i + " of " + n)),
                   559, 806, i, TextAlignment.RIGHT,
                   VerticalAlignment.TOP, 0);
            }

            var docEvent = document.GetPdfDocument();
            Rectangle pageSize = docEvent.GetPage(1).GetPageSize();

            float coordX = ((pageSize.GetLeft() + document.GetLeftMargin())
                                 + (pageSize.GetRight() - document.GetRightMargin())) / 2;

            float footerY = document.GetBottomMargin();

            Canvas canvas = new Canvas(docEvent.GetPage(1), pageSize);
            canvas
                .SetFontSize(14)
                .ShowTextAligned(String.IsNullOrEmpty(entidad.NombreComercial) ? " " : entidad.NombreComercial, coordX, footerY, TextAlignment.CENTER)
                .SetFontSize(10)
                .ShowTextAligned(String.IsNullOrEmpty(entidad.Direccion) ? " " : entidad.Direccion, coordX, footerY - 12, TextAlignment.CENTER)
                .Close();


            // Close document
            document.Close();

            return Task.FromResult(stream.ToArray());
        }
    }



}
