using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models.ViewModels
{
    public class ResumenPagoVM
    {
        public string EmpleadoId { get; set; }
        public string Empleado { get; set; }
        public string DNI { get; set; }
        public decimal TotalHoras { get; set; }
        public decimal TotalPago { get; set; }
        public decimal PrecioBase { get; set; }
        public List<string> Servicios { get; set; }
        public List<DetallePagoVM> Detalles { get; set; }
    }
}