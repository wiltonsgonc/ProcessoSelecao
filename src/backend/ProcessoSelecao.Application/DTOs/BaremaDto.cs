using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Application.DTOs;

public class BaremaDto
{
    public long Id { get; set; }
    public long CandidatoId { get; set; }
    public string? CandidatoNome { get; set; }
    public long AvaliadorId { get; set; }
    public string? AvaliadorNome { get; set; }
    public Dictionary<string, float>? Criterios { get; set; }
    public float NotaFinal { get; set; }
    public string? Observacoes { get; set; }
    public DateTime? DataPreenchimento { get; set; }
    public StatusBarema Status { get; set; }
}

public class CreateBaremaDto
{
    public long CandidatoId { get; set; }
    public long AvaliadorId { get; set; }
}

public class UpdateBaremaDto
{
    public Dictionary<string, float> Criterios { get; set; } = new();
    public string? Observacoes { get; set; }
}

public class FinalizarBaremaDto
{
    public Dictionary<string, float> Criterios { get; set; } = new();
    public string? Observacoes { get; set; }
}
