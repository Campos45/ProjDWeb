using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appMonumentos.Models;

public class Imagem
{
    public int Id { get; set; }

    [Required]
    public string NomeImagem { get; set; }

    [Required]
    public int MonumentoId { get; set; }
    public Monumento Monumento { get; set; }

    [Required]
    public int UtilizadorId { get; set; }
    public Utilizador Utilizador { get; set; }

    public bool IsPrincipal { get; set; } = false;
}