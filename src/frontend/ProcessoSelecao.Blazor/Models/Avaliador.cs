namespace ProcessoSelecao.Blazor.Models;

public class Avaliador
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TipoAvaliador Tipo { get; set; }
    public string? AreaEspecializacao { get; set; }
    public string? Instituicao { get; set; }
    public bool Ativo { get; set; }
    public int AvaliacoesPendentes { get; set; }
}

public class CreateAvaliador
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TipoAvaliador Tipo { get; set; }
    public string? AreaEspecializacao { get; set; }
    public string? Instituicao { get; set; }
    public int? ProcessoSelecaoId { get; set; }
}

public class UpdateAvaliador
{
    public string Nome { get; set; } = string.Empty;
    public string? AreaEspecializacao { get; set; }
    public string? Instituicao { get; set; }
    public bool Ativo { get; set; }
}
