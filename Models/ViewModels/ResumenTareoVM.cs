using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models.ViewModels
{
    public class ResumenTareoVM
    {
    public Usuario Empleado { get; set; }
    public decimal TotalHoras { get; set; }
    public decimal TotalPago { get; set; }
    public List<DetalleTareoVM> Detalles { get; set; } = new List<DetalleTareoVM>();
    public string EstadoResumen { get; set; } // "Aprobado", "Rechazado", "Pendiente"
    public string ObservacionResumen { get; set; }
    }
}