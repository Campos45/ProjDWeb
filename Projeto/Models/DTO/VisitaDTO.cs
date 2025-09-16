// Models/Dtos/VisitaCreateDto.cs
namespace appMonumentos.Models.Dtos
{
    public class VisitaCreateDto
    {
        public int MonumentoId { get; set; }
        public int UtilizadorId { get; set; }
    }
}

// Models/Dtos/VisitaUpdateDto.cs
namespace appMonumentos.Models.Dtos
{
    public class VisitaUpdateDto
    {
        public int Id { get; set; }
        public int NumeroVisitas { get; set; }
    }
}