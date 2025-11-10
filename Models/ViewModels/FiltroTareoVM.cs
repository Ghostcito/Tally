using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models.ViewModels
{
    public class FiltroTareoVM
    {
        public int? Año { get; set; }
        public int? Mes { get; set; }
        public int? Quincena { get; set; } // 1 o 2
        public string IdEmpleado { get; set; }
    }
}