using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SoftWC.Models
{
    public enum SedeEnum
    {
        [Display(Name = "Activa (operativa)")]
        ACTIVA,
        
        [Display(Name = "Inactiva (no operativa)")]
        INACTIVA
    }
}