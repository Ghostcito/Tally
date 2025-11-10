using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using DinkToPdf;
using DinkToPdf.Contracts;
using SoftWC.Models.ViewModels;
using SoftWC.Service.Interfaces;

namespace SoftWC.Service.Implementation
{
    public class PdfExportService : IPdfExportService
    {
        private readonly IConverter _pdfConverter;
        
        public PdfExportService(IConverter pdfConverter)
        {
            _pdfConverter = pdfConverter;
        }
        
        public byte[] GeneratePdf(List<ResumenPagoVM> data, int year, int month, int fortnight)
        {
            var htmlContent = BuildHtmlContent(data, year, month, fortnight);
            
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Landscape
                },
                Objects = {
                    new ObjectSettings() {
                        HtmlContent = htmlContent
                    }
                }
            };
            
            return _pdfConverter.Convert(doc);
            }
            
            private string BuildHtmlContent(List<ResumenPagoVM> datos, int mes, int año, int quincena)
            {
            var sb = new StringBuilder();
            
            sb.Append($@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; }}
                    h1 {{ color: #333366; text-align: center; }}
                    table {{ width: 100%; border-collapse: collapse; }}
                    th {{ background-color: #333366; color: white; padding: 8px; text-align: left; }}
                    td {{ padding: 6px; border-bottom: 1px solid #ddd; }}
                    .total {{ font-weight: bold; }}
                </style>
            </head>
            <body>
                <h1>Resumen de Pagos - {mes}/{año} (Quincena {quincena})</h1>
                <table>
                    <tr>
                        <th>Empleado</th>
                        <th>DNI</th>
                        <th>Total Horas</th>
                        <th>Pago por Hora</th>
                        <th>Total a Pagar</th>
                        <th>Servicio</th>
                    </tr>");
            
            foreach (var item in datos)
            {
                sb.Append($@"
                    <tr>
                        <td>{item.Empleado}</td>
                        <td>{item.DNI}</td>
                        <td>{item.TotalHoras:N2}</td>
                        <td>{item.PrecioBase:N2}</td>
                        <td>{item.TotalPago:N2}</td>
                        <td>{string.Join(", ", item.Servicios)}</td>
                    </tr>");
            }
            
            sb.Append($@"
                    <tr class='total'>
                        <td colspan='2'>TOTAL GENERAL</td>
                        <td>{datos.Sum(d => d.TotalHoras):N2}</td>
                        <td></td>
                        <td>{datos.Sum(d => d.TotalPago):N2}</td>
                        <td></td>
                    </tr>
                </table>
            </body>
            </html>");
            
            return sb.ToString();
        
            }
    }
}