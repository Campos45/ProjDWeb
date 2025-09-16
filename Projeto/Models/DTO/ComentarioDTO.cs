namespace appMonumentos.Models.Dtos
{
    
    /// <summary>
    /// DTO PARA A API COMENTARIOS
    /// </summary>
    public class ComentarioDto
    {
        /// <summary>
        /// identificador do comentario
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// texto associado ao comentario
        /// </summary>
        public string ComentarioTexto { get; set; }
        public DateTime Data { get; set; }
        public int ImagemId { get; set; }
        public int UtilizadorId { get; set; }
        public string NomeAutor { get; set; } // extraído de Utilizador.Nome
    }
    public class ComentarioCreateDto
    {
        public string ComentarioTexto { get; set; }
        public DateTime Data { get; set; } = DateTime.UtcNow;
        public int ImagemId { get; set; }
        public int UtilizadorId { get; set; }
    }

    public class ComentarioUpdateDto
    {
        public int Id { get; set; }
        public string ComentarioTexto { get; set; }
        public DateTime Data { get; set; } = DateTime.UtcNow;
    }
}
