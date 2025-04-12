using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models;

public class Imagem
{
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Nome da Imagem")]
    public string NomeImagem { get; set; }

    [Display(Name = "ID do Monumento")]
    public int MonumentoId { get; set; }
    
    [Display(Name = "Monumento")]
    public Monumento Monumento { get; set; }

    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
}