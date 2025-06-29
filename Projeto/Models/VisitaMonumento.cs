using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models
{
    // Modelo que representa a visita de um utilizador a um monumento
    public class VisitaMonumento
    {
        [Key]
        public int Id { get; set; }  // Identificador único da visita

        // Chave estrangeira para o monumento visitado
        public int MonumentoId { get; set; }
        public Monumento Monumento { get; set; }  // Propriedade de navegação para o monumento

        // Chave estrangeira para o utilizador que fez a visita
        public int UtilizadorId { get; set; }
        public Utilizador Utilizador { get; set; }  // Propriedade de navegação para o utilizador
    }
}