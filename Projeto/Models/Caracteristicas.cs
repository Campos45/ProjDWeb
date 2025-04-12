using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models;

public class Caracteristicas
{
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Display(Name = "Tipo")]
    [Required]
    public string Tipo { get; set; } // edifício, estátua, escultura, etc.

    [Display(Name = "Estilo")]
    [Required]
    public string Estilo { get; set; } // barroco, gótico, etc.

    [Display(Name = "Classificação Patrimonial")]
    [Required]
    public string ClassPatrimonial { get; set; } // nacional, mundial

    public ICollection<Monumento> Monumentos { get; set; } = new List<Monumento>();
}