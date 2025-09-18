namespace appMonumentos.Models.Dtos;

/// DTO para criar utilizador
public class UtilizadorCreateDto
{
    ///username do utilizador 
    public string Username { get; set; }
    ///nome do utilizador
    public string Nome { get; set; }
    ///localidade de onde e o utilizador
    public string LocalidadeUtilizador { get; set; }
    ///email do utilizador
    public string Email { get; set; }
    
    
    public string Password { get; set; } // 🔑 apenas usado no Identity

}
/// DTO para atualizar utilizador
public class UtilizadorUpdateDto : UtilizadorCreateDto
{
    ///id do utilizador a atualizar
    public int Id { get; set; }
}
