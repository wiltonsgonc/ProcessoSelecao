namespace ProcessoSelecao.Domain.Entities;

/// <summary>
/// Template de barema — define a estrutura de avaliação (seções, itens, ranges)
/// </summary>
public class BaremaTemplate : BaseEntity
{
    /// <summary>Nome do template (ex: "PIBIC 2026")</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Descrição opcional</summary>
    public string? Descricao { get; set; }

    /// <summary>Tipo do barema (PIBIC, PIBIT, CUSTOM, etc.)</summary>
    public string TipoBarema { get; set; } = "CUSTOM";

    /// <summary>Ponto máximo total da avaliação</summary>
    public float PontoMaximo { get; set; } = 100;

    /// <summary>Se o template está disponível para uso</summary>
    public bool Ativo { get; set; } = true;

    /// <summary>Quem criou o template</summary>
    public string? CriadoPor { get; set; }

    /// <summary>Itens de avaliação vinculados ao template</summary>
    public virtual ICollection<BaremaTemplateItem> Itens { get; set; } = new List<BaremaTemplateItem>();

    /// <summary>Baremas que usam este template</summary>
    public virtual ICollection<Barema> Baremas { get; set; } = new List<Barema>();
}
