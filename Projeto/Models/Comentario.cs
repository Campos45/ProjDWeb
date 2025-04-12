using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models;

public class Comentario
{
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Display(Name = "Comentario")]
    [Required]
    public string ComentarioTexto { get; set; }

    [Display(Name = "Data")]
    [DataType(DataType.Date)]
    public DateTime Data { get; set; }

    [Display(Name = "ID da imagem")]
    public int ImagemId { get; set; }
    
    [Display(Name = "Imagem")]
    public Imagem Imagem { get; set; }
}