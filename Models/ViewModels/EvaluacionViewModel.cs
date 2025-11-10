using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftWC.Models.ViewModels
{
    public class EvaluacionViewModel
    {
        [Required]
        public string IdEmpleado { get; set; }
        
        [Display(Name = "Empleado")]
        public string NombreEmpleado { get; set; }

        [Required]
        [Display(Name = "Tipo de Empleado")]
        public string TipoEmpleado { get; set; }

        [Required]
        [Display(Name = "Fecha de Evaluación")]
        [DataType(DataType.Date)]
        public DateTime FechaEvaluacion { get; set; } = DateTime.Now;

        [Display(Name = "Comentarios")]
        public string? Descripcion { get; set; }

        // Campos de calificación
        [Required]
        [Display(Name = "Responsabilidad")]
        public Calificacion Responsabilidad { get; set; }

        [Required]
        [Display(Name = "Puntualidad")]
        public Calificacion Puntualidad { get; set; }

        [Required]
        [Display(Name = "Calidad de Trabajo")]
        public Calificacion CalidadTrabajo { get; set; }

        [Required]
        [Display(Name = "Uso de Materiales")]
        public Calificacion UsoMateriales { get; set; }

        [Required]
        [Display(Name = "Actitud")]
        public Calificacion Actitud { get; set; }

        public string? EvaluadorId { get; set; }

        [NotMapped]
        public decimal PuntajeGlobal => 
            ((int)Responsabilidad + (int)Puntualidad + (int)CalidadTrabajo + 
            (int)UsoMateriales + (int)Actitud) / 5.0m;
    }
}