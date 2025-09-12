namespace appMonumentos.Models.Dtos;

public class UtilizadorCreateDto
{
    public string Username { get; set; }
    public string Nome { get; set; }
    public string LocalidadeUtilizador { get; set; }
    public string Email { get; set; }
    
    public string Password { get; set; } // 🔑 apenas usado no Identity

}

public class UtilizadorUpdateDto : UtilizadorCreateDto
{
    public int Id { get; set; }
}
