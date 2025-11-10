using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models
{
    public class Servicio
    {
        [Key]
        public int ServicioId { get; set; }

        [Required(ErrorMessage = "El nombre del servicio es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre de servicio")]
        public string NombreServicio { get; set; }

        [Required(ErrorMessage = "Debe ingresar un tipo de servicio")]
        [Display(Name = "Tipo de servicio")]
        public string TipoServicio { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Precio base por hora de trabajo")]
        [Required(ErrorMessage = "El precio base es obligatorio")]
        [Range(0.1, double.MaxValue, ErrorMessage = "El precio debe ser un número válido mayor a 0")]
        public decimal PrecioBase { get; set; }

        // Duración estimada del servicio
        [Display(Name = "Duración estimada")]
        public TimeSpan? DuracionEstimada { get; set; }
        public ICollection<Usuario> Usuarios { get; set; }
}
}