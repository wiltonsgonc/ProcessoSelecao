namespace ProcessoSelecao.Application.DTOs;

public class OpcaoCursoDto
{
    public int Id { get; set; }
    public int EditalId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int Vagas { get; set; }
    public string? Campus { get; set; }
    public string? LocalProva { get; set; }
}

public class OpcaoCursoCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int Vagas { get; set; }
    public string? Campus { get; set; }
    public string? LocalProva { get; set; }
}
