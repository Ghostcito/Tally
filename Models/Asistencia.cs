using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models
{
    public class Asistencia
    {
        [Key]
        public int IdAsistencia { get; set; }

        [Display(Name = "Empleado")]
        [Required(ErrorMessage = "El ID del empleado es obligatorio")]
        public string? IdEmpleado { get; set; } // Tipo string porque IdentityUser usa string para el Id

        [Display(Name = "Empleado")]
        [ForeignKey("IdEmpleado")]
        public Usuario Empleado { get; set; } //para la navegacion
        
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; }

        [Display(Name = "Hora de entrada")]
        public DateTime? HoraEntrada { get; set; }

        [Display(Name = "Hora de salida")]
        public DateTime? HoraSalida { get; set; }

        [Display(Name = "Horas trabajadas")]
        public decimal HorasTrabajadas { get; set; }
        //Podris tambien acotar las horas descontadas
        //public decimal Horas Descontadas
        [Display(Name = "Presente")]
        public bool Presente { get; set; } // bool para validar y dar una representacion grafica

        //observacion para ser aumentada en el dashboard por parte de un supervisor o jefe de area
        public string? Observacion { get; set; }
    }
}