using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoftWC.Models;

namespace SoftWC.ViewModel
{
    public class MarcaViewModel
    {
        public string? NombreSede { get; set; }
        public string? horaActual { get; set; }
        public string? fechaActual { get; set; }
        public string? NombreEmpleado { get; set; }


        public string? HoraEntradaEsperada { get; set; }
        public string? HoraEntrada { get; set; }


        public string? HoraSalidaEsperada { get; set; }
        public decimal? HorasDescontadas { get; set; }
        public decimal? HorasTrabajadas { get; set; }
        public bool? localizacionExitosa { get; set; }
    }
}