namespace ProcessoSelecao.Blazor.Models;

public class Candidato
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Rg { get; set; }
    public string? Telefone { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? AreaPesquisa { get; set; }
    public StatusValidacao StatusValidacao { get; set; }
    public string? DataCadastro { get; set; }
    public int ProcessoSelecaoId { get; set; }
    public double PontuacaoMedia { get; set; }
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
    public string? Rg { get; set; }
    public string? Telefone { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? AreaPesquisa { get; set; }
    public int ProcessoSelecaoId { get; set; }
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
