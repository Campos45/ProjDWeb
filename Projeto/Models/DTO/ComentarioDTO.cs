namespace appMonumentos.Models.Dto
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
}