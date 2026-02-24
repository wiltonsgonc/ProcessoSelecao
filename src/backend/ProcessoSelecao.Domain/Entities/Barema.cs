using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

public class Barema : BaseEntity
{
    public long CandidatoId { get; set; }
    public virtual Candidato? Candidato { get; set; }
    
    public long AvaliadorId { get; set; }
    public virtual Avaliador? Avaliador { get; set; }
    
    public string? CriteriosJson { get; set; }
    public float NotaFinal { get; set; }
    public string? Observacoes { get; set; }
    public DateTime? DataPreenchimento { get; set; }
    public StatusBarema Status { get; set; } = StatusBarema.Pendente;

    public float CalcularNotaFinal(Dictionary<string, float> criterios)
    {
        if (criterios == null || !criterios.Any()) return 0;
        return criterios.Values.Average();
    }

    public bool ValidarCompletude()
    {
        return !string.IsNullOrEmpty(CriteriosJson) && DataPreenchimento.HasValue;
    }
}
