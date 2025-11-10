using System.ComponentModel.DataAnnotations;

namespace SoftWC.Models
{
    public enum TipoClienteEnum
    {
        [Display(Name = "Corporativos")]
        CORPORATIVOS = 1, //REFERIDO A EMPRESAS PRIVADAS, EN DIVERSAS, 
        [Display(Name = "Residenciales")]
        RESIDENCIALES = 2, //PERSONAS NATURALES, FAIMILIAS, 
        [Display(Name = "Institucionales")]
        INSTITUCIONALES = 3 // ENTIDADES DEL ESTADO, GOBIERNO, MUNICIPIOS, ETC
    }
}