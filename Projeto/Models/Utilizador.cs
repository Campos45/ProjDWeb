using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models;

/// <summary>
/// Representa um utilizador que regista monumentos
/// </summary>
public class Utilizador
{
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Display(Name = "Username")]
    [Required]
    public string Username { get; set; }

    [Display(Name = "Password")]
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Nome do utilizador")]
    [Required]
    public string Nome { get; set; }

    [Display(Name = "Localidade do Utilizador")]
    public string LocalidadeUtilizador { get; set; }

    [Display(Name = "Email do Utilizador")]
    [EmailAddress]
    public string Email { get; set; }

    public ICollection<Monumento> Monumentos { get; set; } = new List<Monumento>();
}