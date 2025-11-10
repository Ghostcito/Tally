using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models.ViewModels
{
    public class UsuarioDetallesViewModel
    {
        public Usuario Usuario { get; set; }
        public List<Evaluaciones> Evaluaciones { get; set; }
    }
}