using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models
{
    // basada en indicadores para la evaluacion de los empleados
   public class Evaluaciones  // ✔ Nombre en singular
{
    [Key]
    public int IdEvaluacion { get; set; }

    [Display(Name = "ID Empleado")]
    [Required(ErrorMessage = "El ID del empleado es obligatorio")]
    public string IdEmpleado { get; set; }

    [Display(Name = "Empleado")]
    [ForeignKey("IdEmpleado")]
    public Usuario Empleado { get; set; }

    [Display(Name = "Tipo de empleado")]
    [Required(ErrorMessage = "El tipo de empleado es obligatorio")]
    public string TipoEmpleado { get; set; } // Ej: "Limpieza", "Supervisor"

    [Display(Name = "Fecha de evaluación")]
    [Required(ErrorMessage = "La fecha de evaluación es obligatoria")]
    public DateTime FechaEvaluacion { get; set; }

    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    // --- Indicadores de evaluación --- //
    [Display(Name = "Responsabilidad")]
    public Calificacion Responsabilidad { get; set; }

    [Display(Name = "Puntualidad")]
    public Calificacion Puntualidad { get; set; }

    [Display(Name = "Calidad de trabajo")]
    public Calificacion CalidadTrabajo { get; set; }

    [Display(Name = "Uso de materiales")]
    public Calificacion UsoMateriales { get; set; }

    [Display(Name = "Actitud")]
    public Calificacion Actitud { get; set; }

    // Calificación global calculada (no se almacena en DB)
    [Display(Name = "Puntaje global")]
    [NotMapped]
    public decimal PuntajeGlobal => 
        ((int)Responsabilidad + (int)Puntualidad + (int)CalidadTrabajo + 
        (int)UsoMateriales + (int)Actitud) / 5.0m;

    // Relación con el supervisor que evaluó
    [Display(Name = "Evaluador")]
    public string? EvaluadorId { get; set; }

    [Display(Name = "Evaluado por")]
    [ForeignKey("EvaluadorId")]
    public Usuario? Evaluador { get; set; }
}

public enum Calificacion
{
    [Display(Name = "Deficiente")] Deficiente = 1,
    [Display(Name = "Regular")] Regular = 2,
    [Display(Name = "Bueno")] Bueno = 3,
    [Display(Name = "Excelente")] Excelente = 4
}
}