using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models;

// Modelo que representa as características comuns a vários monumentos
public class Caracteristicas
{
    [Display(Name = "ID")]
    public int Id { get; set; }  // Identificador único da característica

    [Display(Name = "Tipo")]
    [Required]  // Campo obrigatório
    public string Tipo { get; set; } // Tipo do monumento: edifício, estátua, escultura, etc.

    [Display(Name = "Estilo")]
    [Required]  // Campo obrigatório
    public string Estilo { get; set; } // Estilo arquitetónico: barroco, gótico, etc.

    [Display(Name = "Classificação Patrimonial")]
    [Required]  // Campo obrigatório
    public string ClassPatrimonial { get; set; } // Classificação do património: nacional, mundial, etc.

    // Relação de um-para-muitos: uma característica pode estar associada a vários monumentos
    public ICollection<Monumento> Monumentos { get; set; } = new List<Monumento>();
}