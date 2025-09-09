namespace appMonumentos.Models.Dtos
{
    
    public class ComentarioDto
    {
        public int Id { get; set; }
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
