using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Application.DTOs;

public class ProcessoSelecaoDto
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int VagasDisponiveis { get; set; }
    public StatusProcesso Status { get; set; }
    public int TotalCandidatos { get; set; }
    public int TotalAvaliadores { get; set; }
}

public class CreateProcessoSelecaoDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int VagasDisponiveis { get; set; }
}

public class UpdateProcessoSelecaoDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int VagasDisponiveis { get; set; }
}
