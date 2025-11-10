using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models
{

    //PARA PERMISOS EN CASO DE QUE UN EMPLEADO TENGA QUE SALIR POR UN TIEMPO Y LUEGO RETORNAR
    //FUNCIONALIDAD PARA FUTURO
    public class Permisos
    {
        [Key]
        public int IdPermiso { get; set; }

        [Display(Name = "Empleado")]
        [Required(ErrorMessage = "El ID del empleado es obligatorio")]
        public string? IdEmpleado { get; set; }

        [ForeignKey("IdEmpleado")]
        public Usuario? Empleado { get; set; }

        // public Asistencia IdAsistencia { get; set; }
        [Display(Name = "Hora de salida")]
        [Required(ErrorMessage = "La hora de salida es obligatoria")]
        public TimeSpan HoraSalida { get; set; }

        [Display(Name = "Hora de retorno")]
        [Required(ErrorMessage = "La hora de retorno es obligatoria")]
        public TimeSpan HoraRetorno { get; set; }

        public string? Motivo { get; set; }
        public string? Estado { get; set; } // Aprobada, Rechazada, Pendiente

    }
}