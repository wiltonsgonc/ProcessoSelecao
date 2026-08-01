using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Application.DTOs;

/// <summary>
/// DTO para leitura de dados do Avaliador
/// </summary>
public class AvaliadorDto
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TipoAvaliador Tipo { get; set; }
    public string? AreaEspecializacao { get; set; }
    public string? Instituicao { get; set; }
    public bool Ativo { get; set; }
    public int AvaliacoesPendentes { get; set; }
    public string? LinkLattes { get; set; }
    public string? UltimaFormacao { get; set; }
    public string? Cargo { get; set; }
    public NivelCnpq NivelCnpq { get; set; }
}

/// <summary>
/// DTO para criar novo Avaliador
/// </summary>
public class CreateAvaliadorDto
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TipoAvaliador Tipo { get; set; }
    public string? AreaEspecializacao { get; set; }
    public string? Instituicao { get; set; }
    public long? ProcessoSelecaoId { get; set; }
    public string? LinkLattes { get; set; }
    public string? UltimaFormacao { get; set; }
    public string? Cargo { get; set; }
    public NivelCnpq NivelCnpq { get; set; }
}

/// <summary>
/// DTO para atualizacao de Avaliador
/// </summary>
public class UpdateAvaliadorDto
{
    public string Nome { get; set; } = string.Empty;
    public string? AreaEspecializacao { get; set; }
    public string? Instituicao { get; set; }
    public bool Ativo { get; set; }
    public string? LinkLattes { get; set; }
    public string? UltimaFormacao { get; set; }
    public string? Cargo { get; set; }
    public NivelCnpq NivelCnpq { get; set; }
}