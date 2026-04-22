using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Application.DTOs;

/// <summary>
/// DTO para leitura de dados do Candidato
/// </summary>
public class CandidatoDto
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? RG { get; set; }
    public string? Telefone { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? AreaPesquisa { get; set; }
    public StatusValidacao StatusValidacao { get; set; }
    public DateTime DataCadastro { get; set; }
    public long ProcessoSelecaoId { get; set; }
    public string NumeroInscricao { get; set; } = string.Empty;
    public float PontuacaoMedia { get; set; }
    public int TotalDocumentos { get; set; }
    public int DocumentosValidados { get; set; }
    
    public DateTime? DataNascimento { get; set; }
    public string? PaisNatal { get; set; }
    public string? EstadoNatal { get; set; }
    public string? Naturalidade { get; set; }
    public string? NomeSocial { get; set; }
    public string? EstadoCivil { get; set; }
    public string? Nacionalidade { get; set; }
    public string? Sexo { get; set; }
    public string? Telefone2 { get; set; }
    public string? CorRaca { get; set; }
    public DateTime? DataVencimentoRG { get; set; }
    public string? TipoVisto { get; set; }
    public string? FormaInscricao { get; set; }
    public string? LocalProva { get; set; }
    public string? CampusProva { get; set; }
    public decimal? ValorInscricao { get; set; }
    public bool DeficienciaFisica { get; set; }
    public bool DeficienciaAuditiva { get; set; }
    public bool DeficienciaFala { get; set; }
    public bool DeficienciaVisual { get; set; }
    public bool DeficienciaMental { get; set; }
    public bool DeficienciaIntelectual { get; set; }
    public bool DeficienciaReabilitado { get; set; }
    public bool DeficienciaMultipla { get; set; }
    public string? MotivoOutrasNecessidades { get; set; }
}

/// <summary>
/// DTO para criação de novo Candidato
/// </summary>
public class CreateCandidatoDto
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? RG { get; set; }
    public string? Telefone { get; set; }
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
