/// DTO para criar visita
/// 
// Models/Dtos/VisitaCreateDto.cs
namespace appMonumentos.Models.Dtos
{
    public class VisitaCreateDto
    {
        ///id do monumento a associar a visita
        public int MonumentoId { get; set; }
        ///id do utilizador a associar a visita
        public int UtilizadorId { get; set; }
    }
}

// Models/Dtos/VisitaUpdateDto.cs
/// DTO para atualizar visita
/// 
namespace appMonumentos.Models.Dtos
{
    public class VisitaUpdateDto
    {
        ///id do monumento a associar a visita
        public int Id { get; set; }
        ///numero de visitas a inserir
        public int NumeroVisitas { get; set; }
    }
}