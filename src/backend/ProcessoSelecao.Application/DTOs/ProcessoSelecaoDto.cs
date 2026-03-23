using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Application.DTOs;

/// <summary>
/// DTO para leitura de dados do Processo de Seleção
/// </summary>
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

/// <summary>
/// DTO para criação de novo Processo de Seleção
/// </summary>
public class CreateProcessoSelecaoDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int VagasDisponiveis { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}

/// <summary>
/// DTO para atualização de Processo de Seleção
/// </summary>
public class UpdateProcessoSelecaoDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int VagasDisponiveis { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}
