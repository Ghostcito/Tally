using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models
{
    public class Supervision
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Supervisor")]
        [Required(ErrorMessage = "Se debe seleccionar un supervisor es obligatorio")]
        public string SupervisorId { get; set; }
        [ForeignKey("SupervisorId")]
        public Usuario Supervisor { get; set; }

        [Display(Name = "Empleado asignado")]
        [Required(ErrorMessage = "Se debe seleccionar un empleado asignado es obligatorio")]
        public string EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        
        [Display(Name = "Empleados asignados")]
        [Required(ErrorMessage = "Se debe seleccionar un empleado asignado es obligatorio")]
        public Usuario Empleado { get; set; }

        [Display(Name = "Fecha Inicio actividades")]
        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fecha de fin actividades")]
        public DateTime? FechaFin { get; set; } // NULL si sigue activo
    }
}