namespace appMonumentos.Models.Dtos;

/// DTO para criar localidade
public class LocalidadeCreateDto
{
    ///nome da localidade a ser criada
    public string NomeLocalidade { get; set; }
    ///ID do utilizador associado a localidade
    public int UtilizadorId { get; set; }

}


/// DTO para atualizar localidade
public class LocalidadeUpdateDto : LocalidadeCreateDto
{
    ///ID da localidade a atualizar
    public int Id { get; set; }
}
