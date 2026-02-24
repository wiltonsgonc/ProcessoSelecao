using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Application.DTOs;

public class AvaliadorDto
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TipoAvaliador Tipo { get; set; }
    public string? AreaEspecializacao { get; set; }
    public string? Instituicao { get; set; }
    public bool Ativo { get; set; }
    public int AvaliacoesPendentes { get; set; }
}

public class CreateAvaliadorDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TipoAvaliador Tipo { get; set; }
    public string? AreaEspecializacao { get; set; }
    public string? Instituicao { get; set; }
    public long? ProcessoSelecaoId { get; set; }
}

public class UpdateAvaliadorDto
{
    public string Nome { get; set; } = string.Empty;
    public string? AreaEspecializacao { get; set; }
    public string? Instituicao { get; set; }
    public bool Ativo { get; set; }
}
