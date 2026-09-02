namespace ProcessoSelecao.Blazor.Models;

public class ProcessoSelecao
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? DataInicio { get; set; }
    public string? DataFim { get; set; }
    public int VagasDisponiveis { get; set; }
    public string? AgenciaFomento { get; set; }
    public string? NivelBolsa { get; set; }
    public string TipoProcesso { get; set; } = "PIBIC";
    public StatusProcesso Status { get; set; }
    public int TotalCandidatos { get; set; }
    public int TotalAvaliadores { get; set; }
}

public class CreateProcessoSelecao
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int VagasDisponiveis { get; set; }
    public string? AgenciaFomento { get; set; }
    public string? NivelBolsa { get; set; }
    public string TipoProcesso { get; set; } = "PIBIC";
    public string? DataInicio { get; set; }
    public string? DataFim { get; set; }
}

public class UpdateProcessoSelecao
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int VagasDisponiveis { get; set; }
    public string? AgenciaFomento { get; set; }
    public string? NivelBolsa { get; set; }
    public string TipoProcesso { get; set; } = "PIBIC";
    public string? DataInicio { get; set; }
    public string? DataFim { get; set; }
}
