namespace ProcessoSelecao.Blazor.Models;

public class Candidato
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? AreaPesquisa { get; set; }
    public string? Orientador { get; set; }
    public StatusValidacao StatusValidacao { get; set; }
    public DateTime DataCadastro { get; set; }
    public long ProcessoSelecaoId { get; set; }
    public string NumeroInscricao { get; set; } = string.Empty;
    public float PontuacaoMedia { get; set; }
    public int TotalDocumentos { get; set; }
    public int DocumentosValidados { get; set; }
    public string? TituloProjeto { get; set; }
    public float Nota1 { get; set; }
    public float Nota2 { get; set; }
}

public class CreateCandidato
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? AreaPesquisa { get; set; }
    public long ProcessoSelecaoId { get; set; }
    public string? TituloProjeto { get; set; }
    public float Nota1 { get; set; }
    public float Nota2 { get; set; }
}

public class UpdateCandidato
{
    public string Nome { get; set; } = string.Empty;
    public string? AreaPesquisa { get; set; }
    public string? TituloProjeto { get; set; }
    public float Nota1 { get; set; }
    public float Nota2 { get; set; }
}
