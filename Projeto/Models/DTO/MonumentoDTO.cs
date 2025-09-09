namespace appMonumentos.Models.Dtos;

public class MonumentoCreateDto
{
    public string Designacao { get; set; }
    public string Endereco { get; set; }
    public string Coordenadas { get; set; }
    public string EpocaConstrucao { get; set; }
    public string Descricao { get; set; }
    public int UtilizadorId { get; set; }
    public int LocalidadeId { get; set; }
}

public class MonumentoUpdateDto : MonumentoCreateDto
{
    public int Id { get; set; }
}
