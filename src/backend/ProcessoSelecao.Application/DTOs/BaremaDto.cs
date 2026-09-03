using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Application.DTOs;

/// <summary>
/// DTO para leitura de dados do Barema
/// </summary>
public class BaremaDto
{
    public long Id { get; set; }
    public long CandidatoId { get; set; }
    public string? CandidatoNome { get; set; }
    public long AvaliadorId { get; set; }
    public string? AvaliadorNome { get; set; }
    public string TipoBarema { get; set; } = "PIBIC";
    public long? TemplateId { get; set; }
    public string? TemplateNome { get; set; }
    public Dictionary<string, float>? Criterios { get; set; }
    public List<BaremaItemAvaliacaoDto>? ItensAvaliacao { get; set; }
    public float NotaFinal { get; set; }
    public string? Observacoes { get; set; }
    public DateTime? DataPreenchimento { get; set; }
    public StatusBarema Status { get; set; }
}

/// <summary>
/// DTO para criação de novo Barema
/// </summary>
public class CreateBaremaDto
{
    public long CandidatoId { get; set; }
    public long AvaliadorId { get; set; }
    public string TipoBarema { get; set; } = "PIBIC";
    public long? TemplateId { get; set; }
}

/// <summary>
/// DTO para atualização de Barema
/// </summary>
public class UpdateBaremaDto
{
    public Dictionary<string, float> Criterios { get; set; } = new();
    public string? Observacoes { get; set; }
}

/// <summary>
/// DTO para finalização de Barema
/// </summary>
public class FinalizarBaremaDto
{
    public Dictionary<string, float> Criterios { get; set; } = new();
    public string? Observacoes { get; set; }
}

/// <summary>
/// DTO com dados para preenchimento automático do barema
/// </summary>
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
    public IEnumerable<DocumentoDto>? Documentos { get; set; }
}

/// <summary>
/// DTO com progresso de avaliação por candidato
/// </summary>
public class ProgressoCandidatoDto
{
    public long CandidatoId { get; set; }
    public string CandidatoNome { get; set; } = string.Empty;
    public string? NumeroInscricao { get; set; }
    public int AvaliadoresAtribuidos { get; set; }
    public int AvaliadoresConcluidos { get; set; }
    public int AvaliadoresNecessarios { get; set; } = 2;
    public float NotaFinal { get; set; }
    public List<BaremaProgressoDto> Baremas { get; set; } = new();
}

/// <summary>
/// DTO com dados de um barema para o progresso
/// </summary>
public class BaremaProgressoDto
{
    public long BaremaId { get; set; }
    public long AvaliadorId { get; set; }
    public string? AvaliadorNome { get; set; }
    public float NotaFinal { get; set; }
    public StatusBarema Status { get; set; }
    public DateTime? DataPreenchimento { get; set; }
}
