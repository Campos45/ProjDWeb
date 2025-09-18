namespace appMonumentos.Models.Dtos;

/// DTO para criar imagem
public class ImagemCreateDto
{
    
    public string NomeImagem { get; set; }
    public int MonumentoId { get; set; }
    public int UtilizadorId { get; set; }
    public bool IsPrincipal { get; set; }
}

/// DTO para atualizar imagem
public class ImagemUpdateDto : ImagemCreateDto
{
    public int Id { get; set; }
}
///
