namespace ProcessoSelecao.Application.DTOs;

/// <summary>
/// DTO para leitura de template de barema
/// </summary>
public class BaremaTemplateDto
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string TipoBarema { get; set; } = "CUSTOM";
    public float PontoMaximo { get; set; } = 100;
    public bool Ativo { get; set; } = true;
    public string? CriadoPor { get; set; }
    public DateTime DataCriacao { get; set; }
    public int TotalItens { get; set; }
    public List<BaremaTemplateItemDto> Itens { get; set; } = new();
}

/// <summary>
/// DTO para item de template
/// </summary>
public class BaremaTemplateItemDto
{
    public long Id { get; set; }
    public long TemplateId { get; set; }
    public string Secao { get; set; } = string.Empty;
    public int SecaoOrdem { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public float NotaMinima { get; set; } = 0;
    public float NotaMaxima { get; set; } = 10;
    public float Passo { get; set; } = 1;
    public bool Obrigatorio { get; set; } = true;
}

/// <summary>
/// DTO para criação de template
/// </summary>
public class CreateBaremaTemplateDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string TipoBarema { get; set; } = "CUSTOM";
    public float PontoMaximo { get; set; } = 100;
    public string? CriadoPor { get; set; }
    public List<CreateBaremaTemplateItemDto> Itens { get; set; } = new();
}

/// <summary>
/// DTO para criação de item de template
/// </summary>
public class CreateBaremaTemplateItemDto
{
    public string Secao { get; set; } = string.Empty;
    public int SecaoOrdem { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public float NotaMinima { get; set; } = 0;
    public float NotaMaxima { get; set; } = 10;
    public float Passo { get; set; } = 1;
    public bool Obrigatorio { get; set; } = true;
}

/// <summary>
/// DTO para atualização de template
/// </summary>
public class UpdateBaremaTemplateDto
{
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public bool? Ativo { get; set; }
    public List<CreateBaremaTemplateItemDto>? Itens { get; set; }
}

/// <summary>
/// DTO para resposta de finalização de barema com template
/// </summary>
public class FinalizarBaremaTemplateDto
{
    public List<BaremaItemAvaliacaoDto> Itens { get; set; } = new();
    public string? Observacoes { get; set; }
}

/// <summary>
/// DTO para item de avaliação preenchido
/// </summary>
public class BaremaItemAvaliacaoDto
{
    public long TemplateItemId { get; set; }
    public float Nota { get; set; }
}
