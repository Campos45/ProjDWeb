namespace appMonumentos.Models.Dtos;

/// DTO para criar monumento
public class MonumentoCreateDto
{
    ///designaçao do monumento
    public string Designacao { get; set; }
    ///endereço do monumento
    public string Endereco { get; set; }
    ///coordenadas do monumento
    public string Coordenadas { get; set; }
    ///epoca de construçao do monumento
    public string EpocaConstrucao { get; set; }
    ///descriçao do monumento
    public string Descricao { get; set; }
    ///id do utilizador associado ao monumento
    public int UtilizadorId { get; set; }
    ///id da localidade do monumento
    public int LocalidadeId { get; set; }
}

/// DTO para atualizar monumento
public class MonumentoUpdateDto : MonumentoCreateDto
{
    ///id do monumento a atualizar
    public int Id { get; set; }
}
