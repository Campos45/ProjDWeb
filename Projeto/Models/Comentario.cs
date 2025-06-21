using System;
using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models
{
    public class Comentario
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Comentário")]
        [Required]
        public string ComentarioTexto { get; set; }

        [Display(Name = "Data")]
        [DataType(DataType.DateTime)]
        public DateTime Data { get; set; }

        public int ImagemId { get; set; }

        public Imagem Imagem { get; set; }

        public Utilizador Utilizador { get; set; } // Adiciona esta propriedade
    }
}