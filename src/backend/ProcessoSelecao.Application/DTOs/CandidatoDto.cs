using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Application.DTOs;

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

public class CreateCandidatoDto
{
    public string Nome { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AreaPesquisa { get; set; }
    public long ProcessoSelecaoId { get; set; }
}

public class UpdateCandidatoDto
{
    public string Nome { get; set; } = string.Empty;
    public string? AreaPesquisa { get; set; }
}
