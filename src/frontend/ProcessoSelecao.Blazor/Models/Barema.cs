namespace ProcessoSelecao.Blazor.Models;

public class Barema
{
    public int Id { get; set; }
    public int CandidatoId { get; set; }
    public string? CandidatoNome { get; set; }
    public int AvaliadorId { get; set; }
    public string? AvaliadorNome { get; set; }
    public Dictionary<string, double>? Criterios { get; set; }
    public double NotaFinal { get; set; }
    public string? Observacoes { get; set; }
    public string? DataPreenchimento { get; set; }
    public StatusBarema Status { get; set; }
}

public class CreateBarema
{
    public int CandidatoId { get; set; }
    public int AvaliadorId { get; set; }
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
