namespace ProcessoSelecao.Blazor.Models;

public class Barema
{
    public int Id { get; set; }
    public int CandidatoId { get; set; }
    public string? CandidatoNome { get; set; }
    public int AvaliadorId { get; set; }
    public string? AvaliadorNome { get; set; }
    public string TipoBarema { get; set; } = "PIBIC";
    public long? TemplateId { get; set; }
    public string? TemplateNome { get; set; }
    public Dictionary<string, double>? Criterios { get; set; }
    public List<BaremaItemAvaliacaoInput>? ItensAvaliacao { get; set; }
    public double NotaFinal { get; set; }
    public string? Observacoes { get; set; }
    public string? DataPreenchimento { get; set; }
    public StatusBarema Status { get; set; }
}

public class CreateBarema
{
    public int CandidatoId { get; set; }
    public int AvaliadorId { get; set; }
    public string TipoBarema { get; set; } = "PIBIC";
    public long? TemplateId { get; set; }
}

public class UpdateBarema
{
    public Dictionary<string, double> Criterios { get; set; } = new();
    public string? Observacoes { get; set; }
}

public class FinalizarBarema
{
    public Dictionary<string, double> Criterios { get; set; } = new();
    public string? Observacoes { get; set; }
}

public class ProgressoCandidato
{
    public long CandidatoId { get; set; }
    public string CandidatoNome { get; set; } = string.Empty;
    public string? NumeroInscricao { get; set; }
    public int AvaliadoresAtribuidos { get; set; }
    public int AvaliadoresConcluidos { get; set; }
    public int AvaliadoresNecessarios { get; set; } = 2;
    public double NotaFinal { get; set; }
    public List<BaremaProgresso> Baremas { get; set; } = new();
}

public class BaremaProgresso
{
    public long BaremaId { get; set; }
    public long AvaliadorId { get; set; }
    public string? AvaliadorNome { get; set; }
    public double NotaFinal { get; set; }
    public StatusBarema Status { get; set; }
    public string? DataPreenchimento { get; set; }
}
