using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models.ViewModels
{
    public class DetalleTareoVM
    {
    public DateTime Fecha { get; set; }
    public string Servicio { get; set; }
    public decimal HorasTrabajadas { get; set; }
    public decimal PagoPorHora { get; set; }
    public decimal TotalDia { get; set; }
    public string Estado { get; set; }
    public string Observacion { get; set; }
    }
}