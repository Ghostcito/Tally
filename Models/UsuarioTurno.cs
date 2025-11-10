using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftWC.Models
{
    public class UsuarioTurno
    {
        [Key]
        public int UsuarioTurnoId { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio")]
        [Display(Name = "Usuario")]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [Display(Name = "Turno")]
        public int TurnoId { get; set; }

        [ForeignKey("TurnoId")]
        public Turno Turno { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [Display(Name = "Fecha de inicio")]
        public DateTime? FechaInicio { get; set; }

        [Display(Name = "Fecha de fin")]
        public DateTime? FechaFin { get; set; } // Null si es el turno actual

        public bool Activo { get; set; } = true;
    }
}