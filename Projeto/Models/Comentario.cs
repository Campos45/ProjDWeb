using System;
using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models
{
    // Modelo que representa um comentário feito por um utilizador numa imagem
    public class Comentario
    {
        [Key]
        /// Identificador único do comentário
        public int Id { get; set; }  

        /// Texto do comentário
        [Display(Name = "Comentário")]
        [Required]  // O texto do comentário é obrigatório
        public string ComentarioTexto { get; set; }  

        /// Data e hora em que o comentário foi feito
        [Display(Name = "Data")]
        [DataType(DataType.DateTime)]  // Define que o campo é do tipo Data e Hora
        public DateTime Data { get; set; }  

        /// Propriedade de navegação para a imagem
        [Display(Name = "Imagem")]
        public int ImagemId { get; set; }  // Chave estrangeira para a imagem associada
        public Imagem Imagem { get; set; } 

        /// Propriedade de navegação para o utilizador
        /// Associação ao utilizador que fez o comentário
        [Display(Name = "Utilizador")]
        public int UtilizadorId { get; set; }  // Chave estrangeira para o utilizador autor
        public Utilizador Utilizador { get; set; } 
    }
}