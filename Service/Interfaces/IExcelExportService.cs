using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoftWC.Models.ViewModels;

namespace SoftWC.Service.Interfaces
{
    public interface IExcelExportService
    {
        byte[] GenerateExcel(List<ResumenPagoVM> data, int year, int month, int fortnight);
    }
}