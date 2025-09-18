using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appMonumentos.Models;

// Modelo que representa uma imagem associada a um monumento
public class Imagem
{
    /// Identificador único da imagem
    public int Id { get; set; }  

    /// Nome do ficheiro da imagem
    [Required]  // Campo obrigatório
    public string NomeImagem { get; set; }  

    /// Propriedade de navegação para o monumento associado
    [Required]  // Campo obrigatório, chave estrangeira para Monumento
    public int MonumentoId { get; set; }
    public Monumento Monumento { get; set; }  

    /// Propriedade de navegação para o utilizador que adicionou a imagem
    [Required]  // Campo obrigatório, chave estrangeira para Utilizador
    public int UtilizadorId { get; set; }
    public Utilizador Utilizador { get; set; }  

    /// Indica se esta imagem é a imagem principal do monumento (por defeito é falso)
    public bool IsPrincipal { get; set; } = false;

    /// Relação 1:N: uma imagem pode ter vários comentários associados
    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
}