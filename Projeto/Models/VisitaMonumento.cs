// Models/VisitaMonumento.cs
using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models
{
    public class VisitaMonumento
    {
        [Key]
        public int Id { get; set; }

        public int MonumentoId { get; set; }
        public Monumento Monumento { get; set; }

        public int UtilizadorId { get; set; }
        public Utilizador Utilizador { get; set; }
    }
}