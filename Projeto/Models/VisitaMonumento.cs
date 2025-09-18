// Models/VisitaMonumento.cs
using System.ComponentModel.DataAnnotations;

namespace appMonumentos.Models
{
    /// <summary>
    /// Representa a relação entre Utilizador e Monumento (visita).
    /// Contem também o número de vezes que o utilizador visitou o monumento.
    /// </summary>
    public class VisitaMonumento
    {
        [Key]
        public int Id { get; set; }

        /// FK para o monumento
        public int MonumentoId { get; set; }
        public Monumento Monumento { get; set; } = null!;

        /// FK para o utilizador (tabela Utilizador do projecto)
        public int UtilizadorId { get; set; }
        public Utilizador Utilizador { get; set; } = null!;

        /// Número de visitas deste utilizador a este monumento (não nulo, default 1)
        public int NumeroVisitas { get; set; } = 1;
    }
}