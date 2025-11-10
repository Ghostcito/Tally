using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models
{
    //cliente para manejarlo como sede
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder {1} caracteres")]
        public string? Nombre { get; set; }

        [StringLength(50, ErrorMessage = "El apellido no puede exceder {1} caracteres")]
        public string? Apellido { get; set; }
        public string? Telefono { get; set; }
        
        [Display(Name = "Correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        public string? Correo { get; set; }

        [Display(Name = "Tipo de cliente")]
        public TipoClienteEnum TipoCliente { get; set; }

        //Estado para que seria? podria ser Activo, Inactivo, Bloqueado, etc. pero no lo veo muy significativo
        public bool Estado { get; set; } // activo o inactivo    
    }
}