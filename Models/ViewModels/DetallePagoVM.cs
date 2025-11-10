using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models.ViewModels
{
    public class DetallePagoVM
    {
        public DateTime Fecha { get; set; }
        public string Servicio { get; set; }
        public decimal Horas { get; set; }
        public decimal PagoHora { get; set; }
        public decimal TotalDia { get; set; }
    }
}