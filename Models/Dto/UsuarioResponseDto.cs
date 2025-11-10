using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models.Dto
{
    public class UsuarioResponseDto
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? NivelAcceso { get; set; }
        public string? Estado { get; set; }
        public decimal? Salario { get; set; }
        
        // Información de sedes (puedes usar un DTO simple para sedes también)
        public List<SedeInfoDto> Sedes { get; set; } = new List<SedeInfoDto>();
    }
}