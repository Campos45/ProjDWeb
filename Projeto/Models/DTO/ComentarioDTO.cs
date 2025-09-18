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
        /// <summary>
        /// data da submissao do comentario
        /// </summary>
        public DateTime Data { get; set; }
        /// <summary>
        /// ID da imagem associada ao comentario
        /// </summary>
        public int ImagemId { get; set; }
        /// <summary>
        /// ID do utilizador associado ao comentario
        /// </summary>
        public int UtilizadorId { get; set; }
        /// <summary>
        /// nome do autor do comentario
        /// </summary>
        public string NomeAutor { get; set; } // extraído de Utilizador.Nome
    }
    
    
    /// <summary>
    /// DTO para criar comentarios
    /// </summary>
    public class ComentarioCreateDto
    {
        public string ComentarioTexto { get; set; }
        public DateTime Data { get; set; } = DateTime.UtcNow;
        public int ImagemId { get; set; }
        public int UtilizadorId { get; set; }
    }

    
    /// <summary>
    /// DTO para atualizar comentarios
    /// </summary>
    public class ComentarioUpdateDto
    {
        public int Id { get; set; }
        public string ComentarioTexto { get; set; }
        public DateTime Data { get; set; } = DateTime.UtcNow;
    }
}
