using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models;

// Modelo que representa uma localidade onde existem monumentos
public class Localidade
{
    /// Identificador único da localidade
    [Display(Name = "ID")]
    public int Id { get; set; }  

    /// Nome da localidade
    [Display(Name = "Nome da Localidade")]
    public string NomeLocalidade { get; set; }  
    
    /// Id do utilizador associado a localidade
    public int? UtilizadorId { get; set; }
    public Utilizador? Utilizador { get; set; } 

    /// Relação 1:N: uma localidade pode ter vários monumentos
    public ICollection<Monumento> Monumentos { get; set; } = new List<Monumento>();
}