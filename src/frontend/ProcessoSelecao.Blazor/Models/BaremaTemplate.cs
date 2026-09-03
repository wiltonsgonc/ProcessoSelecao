namespace ProcessoSelecao.Blazor.Models;

public class BaremaTemplate
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string TipoBarema { get; set; } = "CUSTOM";
    public double PontoMaximo { get; set; } = 100;
    public bool Ativo { get; set; } = true;
    public string? CriadoPor { get; set; }
    public string? DataCriacao { get; set; }
    public int TotalItens { get; set; }
    public List<BaremaTemplateItem> Itens { get; set; } = new();
}

public class BaremaTemplateItem
{
    public long Id { get; set; }
    public long TemplateId { get; set; }
    public string Secao { get; set; } = string.Empty;
    public int SecaoOrdem { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public double NotaMinima { get; set; } = 0;
    public double NotaMaxima { get; set; } = 10;
    public double Passo { get; set; } = 1;
    public bool Obrigatorio { get; set; } = true;
}

public class CreateBaremaTemplate
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string TipoBarema { get; set; } = "CUSTOM";
    public double PontoMaximo { get; set; } = 100;
    public string? CriadoPor { get; set; }
    public List<CreateBaremaTemplateItem> Itens { get; set; } = new();
}

public class CreateBaremaTemplateItem
{
    public string Secao { get; set; } = string.Empty;
    public int SecaoOrdem { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public double NotaMinima { get; set; } = 0;
    public double NotaMaxima { get; set; } = 10;
    public double Passo { get; set; } = 1;
    public bool Obrigatorio { get; set; } = true;
}

public class UpdateBaremaTemplate
{
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public bool? Ativo { get; set; }
    public List<CreateBaremaTemplateItem>? Itens { get; set; }
}

public class FinalizarBaremaComTemplate
{
    public List<BaremaItemAvaliacaoInput> Itens { get; set; } = new();
    public string? Observacoes { get; set; }
}

public class BaremaItemAvaliacaoInput
{
    public long TemplateItemId { get; set; }
    public double Nota { get; set; }
}
