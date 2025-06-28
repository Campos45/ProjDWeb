using System;
using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models
{
    public class Comentario
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Comentário")]
        [Required]
        public string ComentarioTexto { get; set; }

        [Display(Name = "Data")]
        [DataType(DataType.DateTime)]
        public DateTime Data { get; set; }

        [Display(Name = "Imagem")]
        public int ImagemId { get; set; }
        public Imagem Imagem { get; set; }

        // ✅ Associação ao Utilizador
        [Display(Name = "Utilizador")]
        public int UtilizadorId { get; set; }
        public Utilizador Utilizador { get; set; }
    }
}