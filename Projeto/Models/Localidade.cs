using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models;

public class Localidade
{
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Display(Name = "Nome da Localidade")]
    public string NomeLocalidade { get; set; }

    public ICollection<Monumento> Monumentos { get; set; } = new List<Monumento>();
}