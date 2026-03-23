using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Application.DTOs;

/// <summary>
/// DTO para leitura de dados do Candidato
/// </summary>
public class CandidatoDto
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AreaPesquisa { get; set; }
    public StatusValidacao StatusValidacao { get; set; }
    public DateTime DataCadastro { get; set; }
    public long ProcessoSelecaoId { get; set; }
    public float PontuacaoMedia { get; set; }
    public int TotalDocumentos { get; set; }
    public int DocumentosValidados { get; set; }
}

/// <summary>
/// DTO para criação de novo Candidato
/// </summary>
public class CreateCandidatoDto
{
    public string Nome { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AreaPesquisa { get; set; }
    public long ProcessoSelecaoId { get; set; }
}

/// <summary>
/// DTO para atualização de Candidato
/// </summary>
public class UpdateCandidatoDto
{
    public string Nome { get; set; } = string.Empty;
    public string? AreaPesquisa { get; set; }
}
