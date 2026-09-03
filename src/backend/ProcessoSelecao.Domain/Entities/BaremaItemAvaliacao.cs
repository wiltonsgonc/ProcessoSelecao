namespace ProcessoSelecao.Domain.Entities;

/// <summary>
/// Resposta do avaliador para um item de avaliação (template preenchido)
/// </summary>
public class BaremaItemAvaliacao : BaseEntity
{
    /// <summary>ID do barema (avaliação)</summary>
    public long BaremaId { get; set; }

    /// <summary>Barema relacionado</summary>
    public virtual Barema? Barema { get; set; }

    /// <summary>ID do item do template</summary>
    public long TemplateItemId { get; set; }

    /// <summary>Item do template relacionado</summary>
    public virtual BaremaTemplateItem? TemplateItem { get; set; }

    /// <summary>Nota atribuída pelo avaliador</summary>
    public float Nota { get; set; }
}
