using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models.Account
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string LocalidadeUtilizador { get; set; }
    }
}