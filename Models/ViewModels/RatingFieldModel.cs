using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftWC.Models.ViewModels
{
    public class RatingFieldModel
    {
        [Required(ErrorMessage = "Este campo es requerido")]
        public Calificacion CurrentValue { get; set; } = Calificacion.Bueno; // Valor por defecto

        [Required]
        public string PropertyName { get; set; } // Nombre de la propiedad en el modelo principal

        [Required]
        public string DisplayName { get; set; } // Etiqueta a mostrar

        // Opcional: Puedes añadir más propiedades según necesites
        public string Tooltip { get; set; } = "Seleccione una calificación";
        public bool Required { get; set; } = true;
    }
}