namespace ProcessoSelecao.Blazor.Models;

public class LoginAvaliador
{
    public string Cpf { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class AvaliadorAuthResponse
{
    public string Token { get; set; } = string.Empty;
    public long AvaliadorId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime Expiracao { get; set; }
}

public class AvaliadorPainelBarema
{
    public long Id { get; set; }
    public long CandidatoId { get; set; }
    public string? CandidatoNome { get; set; }
    public string? AvaliadorNome { get; set; }
    public float NotaFinal { get; set; }
    public string? Observacoes { get; set; }
    public string? DataPreenchimento { get; set; }
    public StatusBarema Status { get; set; }
    public Dictionary<string, double>? Criterios { get; set; }
}

public class AvaliadorCandidatoInfo
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? AreaPesquisa { get; set; }
    public string? TituloProjeto { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
}

public class BaremaDadosDto
{
    public long BaremaId { get; set; }
    public string? NomeOrientador { get; set; }
    public string? NomeEstudante { get; set; }
    public string? CursoGraduacao { get; set; }
    public string? NomeAvaliador { get; set; }
    public string TipoBarema { get; set; } = "PIBIC";
    public StatusBarema Status { get; set; }
    public string? CriteriosJson { get; set; }
    public float NotaFinal { get; set; }
    public string? Observacoes { get; set; }
    public List<Models.Documento>? Documentos { get; set; }
}
