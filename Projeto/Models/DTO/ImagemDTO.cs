namespace appMonumentos.Models.Dtos;

public class ImagemCreateDto
{
    public string NomeImagem { get; set; }
    public int MonumentoId { get; set; }
    public int UtilizadorId { get; set; }
    public bool IsPrincipal { get; set; }
}

public class ImagemUpdateDto : ImagemCreateDto
{
    public int Id { get; set; }
}
