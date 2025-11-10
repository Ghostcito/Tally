using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models.Dto
{
    public class UsuarioDTO
    {
        [Required]
        public string? Nombre { get; set; }
        
        [Required]
        public string? Apellido { get; set; }
        
        [Required]
        public string? DNI { get; set; }
        
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        
        [Required]
        public string? UserName { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? NivelAcceso { get; set; }
        public decimal? Salario { get; set; }
        
        // Para asignación a sedes
        public List<int> SedeIds { get; set; } = new List<int>();
    }
}