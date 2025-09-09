namespace appMonumentos.Models.Dtos;

public class LocalidadeCreateDto
{
    public string NomeLocalidade { get; set; }
}

public class LocalidadeUpdateDto : LocalidadeCreateDto
{
    public int Id { get; set; }
}
