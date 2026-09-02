using System.Text.Json;
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
    
    /// <summary>Tipo do barema (PIBIC, PIBIT, etc.)</summary>
    public string TipoBarema { get; set; } = "PIBIC";
    
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
    /// Calcula a nota final para barema PIBIC
    /// </summary>
    public float CalcularNotaFinalPibic(string criteriosJson)
    {
        if (string.IsNullOrEmpty(criteriosJson)) return 0;

        var dados = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(criteriosJson);
        if (dados == null) return 0;

        float soma1 = 0, soma2 = 0, soma3 = 0;

        if (dados.TryGetValue("projeto", out var projetoObj) && projetoObj is JsonElement projetoEl)
        {
            var projeto = JsonSerializer.Deserialize<Dictionary<string, float>>(projetoEl.GetRawText());
            if (projeto != null) soma1 = projeto.Values.Sum();
        }

        if (dados.TryGetValue("orientador", out var orientadorObj) && orientadorObj is JsonElement orientadorEl)
        {
            var orientador = JsonSerializer.Deserialize<Dictionary<string, float>>(orientadorEl.GetRawText());
            if (orientador != null) soma2 = orientador.Values.Sum();
        }

        if (dados.TryGetValue("candidato", out var candidatoObj) && candidatoObj is JsonElement candidatoEl)
        {
            var candidato = JsonSerializer.Deserialize<Dictionary<string, float>>(candidatoEl.GetRawText());
            if (candidato != null) soma3 = candidato.Values.Sum();
        }

        return soma1 + soma2 + soma3;
    }

    /// <summary>
    /// Verifica se a avaliação está completa
    /// </summary>
    public bool ValidarCompletude()
    {
        return !string.IsNullOrEmpty(CriteriosJson) && DataPreenchimento.HasValue;
    }
}
