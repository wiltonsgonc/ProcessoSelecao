namespace ProcessoSelecao.Domain.Entities;

/// <summary>
/// Item de avaliação dentro de um template de barema
/// </summary>
public class BaremaTemplateItem : BaseEntity
{
    /// <summary>ID do template pai</summary>
    public long TemplateId { get; set; }

    /// <summary>Template relacionado</summary>
    public virtual BaremaTemplate? Template { get; set; }

    /// <summary>Nome da seção (ex: "PROJETO", "ORIENTADOR", "CANDIDATO")</summary>
    public string Secao { get; set; } = string.Empty;

    /// <summary>Ordem da seção (1, 2, 3)</summary>
    public int SecaoOrdem { get; set; }

    /// <summary>Nome do item (ex: "1.1 Relação com projeto SENAI")</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Ordem do item dentro da seção</summary>
    public int Ordem { get; set; }

    /// <summary>Nota mínima possível</summary>
    public float NotaMinima { get; set; } = 0;

    /// <summary>Nota máxima possível</summary>
    public float NotaMaxima { get; set; } = 10;

    /// <summary>Passo do incremento (1, 0.5, etc.)</summary>
    public float Passo { get; set; } = 1;

    /// <summary>Se o item é obrigatório preencher</summary>
    public bool Obrigatorio { get; set; } = true;
}
