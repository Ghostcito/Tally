using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SoftWC.Models
{
    //ya combinamos empleados y administrativos en un solo modelo
    public class Usuario : IdentityUser
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? DNI { get; set; }

        [Display(Name = "Fecha de ingreso")]
        public DateTime? FechaIngreso { get; set; } // fecha de ingreso a la empresa

        [Display(Name = "Fecha de nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [Display(Name = "Nivel de acceso")]
        public string? NivelAcceso { get; set; }
        public string? Estado { get; set; } //activo o inactivo
        public decimal? Salario { get; set; }

        //un usuario puede tener varias sedes
        [Display(Name = "Sedes asignadas")]
        public ICollection<Sede> Sedes { get; set; } = new List<Sede>();

        public int? ServicioId { get; set; }

        [ForeignKey("ServicioId")]
        public Servicio? Servicio { get; set; }


        //RELACIONES EMPLEADO - SUPERVISOR
        public ICollection<Supervision>? EmpleadosSupervisados { get; set; } // Si es Supervisor
        public ICollection<Supervision>? SupervisoresAsignados { get; set; } // Si es Empleado

    }   
}