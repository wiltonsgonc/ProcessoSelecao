using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

/// <summary>
/// Entidade que representa a avaliação/barema de um candidato
/// </summary>
public class Barema : BaseEntity
{
    /// <summary>ID do candidato avaliado</summary>
    public long CandidatoId { get; set; }
    
    /// <summary>Candidato relacionado</summary>
    public virtual Candidato? Candidato { get; set; }
    
    /// <summary>ID do avaliador</summary>
    public long AvaliadorId { get; set; }
    
    /// <summary>Avaliador relacionado</summary>
    public virtual Avaliador? Avaliador { get; set; }
    
    /// <summary>Critérios de avaliação em formato JSON</summary>
    public string? CriteriosJson { get; set; }
    
    /// <summary>Nota final calculada</summary>
    public float NotaFinal { get; set; }
    
    /// <summary>Observações do avaliador</summary>
    public string? Observacoes { get; set; }
    
    /// <summary>Data do preenchimento da avaliação</summary>
    public DateTime? DataPreenchimento { get; set; }
    
    /// <summary>Status atual da avaliação</summary>
    public StatusBarema Status { get; set; } = StatusBarema.Pendente;

    /// <summary>
    /// Calcula a nota final com base nos critérios
    /// </summary>
    public float CalcularNotaFinal(Dictionary<string, float> criterios)
    {
        if (criterios == null || !criterios.Any()) return 0;
        return criterios.Values.Average();
    }

    /// <summary>
    /// Verifica se a avaliação está completa
    /// </summary>
    public bool ValidarCompletude()
    {
        return !string.IsNullOrEmpty(CriteriosJson) && DataPreenchimento.HasValue;
    }
}
