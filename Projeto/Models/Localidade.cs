using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models;

// Modelo que representa uma localidade onde existem monumentos
public class Localidade
{
    [Display(Name = "ID")]
    public int Id { get; set; }  // Identificador único da localidade

    [Display(Name = "Nome da Localidade")]
    public string NomeLocalidade { get; set; }  // Nome da localidade

    // Relação 1:N: uma localidade pode ter vários monumentos
    public ICollection<Monumento> Monumentos { get; set; } = new List<Monumento>();
}