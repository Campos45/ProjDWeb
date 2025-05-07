using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models;

public class Monumento
{
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Display(Name = "Designação")]
    [Required]
    public string Designacao { get; set; }

    [Display(Name = "Endereço")]
    public string Endereco { get; set; }

    [Display(Name = "Coordenadas")]
    public string Coordenadas { get; set; }

    [Display(Name = "Época de Construção")]
    public string EpocaConstrucao { get; set; }

    [Display(Name = "Descrição")]
    public string Descricao { get; set; }

    // Relações
    [Required(ErrorMessage = "O campo Utilizador é obrigatório.")]
    [Display(Name = "Utilizador")]
    public int UtilizadorId { get; set; }
    public Utilizador? Utilizador { get; set; }

    [Required(ErrorMessage = "O campo Localidade é obrigatório.")]
    [Display(Name = "Localidade")]
    public int LocalidadeId { get; set; }
    public Localidade? Localidade { get; set; }

    public ICollection<Imagem> Imagens { get; set; } = new List<Imagem>();

    public ICollection<Caracteristicas> Caracteristicas { get; set; } = new List<Caracteristicas>();
}