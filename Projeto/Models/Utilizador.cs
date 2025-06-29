using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models;

/// <summary>
/// Representa um utilizador que regista monumentos e interage com a aplicação
/// </summary>
public class Utilizador
{
    [Display(Name = "ID")]
    public int Id { get; set; }  // Identificador único do utilizador

    [Display(Name = "Username")]
    [Required]  // Campo obrigatório
    public string Username { get; set; }  // Nome de utilizador (login)

    [Display(Name = "Password")]
    [Required]  // Campo obrigatório
    [DataType(DataType.Password)]  // Indica que é um campo de password (para UI)
    public string Password { get; set; }  // Palavra-passe do utilizador

    [Display(Name = "Nome do utilizador")]
    [Required]  // Campo obrigatório
    [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Utilize apenas letras maiúsculas e minúsculas.")] 
    public string Nome { get; set; }  // Nome real do utilizador (só letras)

    [Display(Name = "Localidade do Utilizador")]
    [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Utilize apenas letras maiúsculas e minúsculas.")]
    public string LocalidadeUtilizador { get; set; }  // Localidade do utilizador (opcional)

    [Display(Name = "Email do Utilizador")]
    [EmailAddress]  // Validação de email
    public string Email { get; set; }  // Email do utilizador (opcional)

    // Relação 1:N: um utilizador pode criar vários monumentos
    public ICollection<Monumento> Monumentos { get; set; } = new List<Monumento>();

    // Relação 1:N: um utilizador pode escrever vários comentários
    public ICollection<Comentario> Comentario { get; set; } = new List<Comentario>();

    // Relação 1:N: um utilizador pode ter várias visitas marcadas a monumentos
    public List<VisitaMonumento> VisitasAosMonumentos { get; set; } = new List<VisitaMonumento>();
}