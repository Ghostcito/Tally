using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoftWC.Models.ViewModels;

namespace SoftWC.Service.Implementation
{
    public interface IPdfExportService
    {
        byte[] GeneratePdf(List<ResumenPagoVM> data, int year, int month, int fortnight);
    }
}