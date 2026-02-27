namespace ProcessoSelecao.Application.DTOs;

/// <summary>
/// DTO para leitura de dados da Opção de Curso
/// </summary>
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

/// <summary>
/// DTO para criação de nova Opção de Curso
/// </summary>
public class OpcaoCursoCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int Vagas { get; set; }
    public string? Campus { get; set; }
    public string? LocalProva { get; set; }
}
