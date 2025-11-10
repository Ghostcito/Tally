using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using SoftWC.Models.ViewModels;
using SoftWC.Service.Interfaces;

namespace SoftWC.Service.Implementation
{
    public class ExcelExportService : IExcelExportService
{
    public byte[] GenerateExcel(List<ResumenPagoVM> data, int año, int mes, int quincena)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Resumen de Pagos");

            // Encabezados
            worksheet.Cell(1, 1).Value = "Empleado";
            worksheet.Cell(1, 2).Value = "DNI";
            worksheet.Cell(1, 3).Value = "Total Horas";
            worksheet.Cell(1, 4).Value = "Pago por Hora";
            worksheet.Cell(1, 5).Value = "Total a Pagar";
            worksheet.Cell(1, 6).Value = "Servicio";

            // Datos
            int row = 2;
            foreach (var item in data)
            {
                worksheet.Cell(row, 1).Value = item.Empleado;
                worksheet.Cell(row, 2).Value = item.DNI;
                worksheet.Cell(row, 3).Value = item.TotalHoras;
                worksheet.Cell(row, 4).Value = item.PrecioBase;
                worksheet.Cell(row, 5).Value = item.TotalPago;
                worksheet.Cell(row, 6).Value = string.Join(", ", item.Servicios);
                row++;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray(); // 🔁 solo devuelve los bytes
            }
        }
    }

}
}