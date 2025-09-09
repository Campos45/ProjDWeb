namespace appMonumentos.Models.Dtos;

public class VisitaCreateDto
{
    public int MonumentoId { get; set; }
    public int UtilizadorId { get; set; }
}

public class VisitaUpdateDto : VisitaCreateDto
{
    public int Id { get; set; }
}