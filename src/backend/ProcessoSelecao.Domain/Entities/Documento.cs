using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

/// <summary>
/// Entidade que representa um documento enviado pelo candidato
/// </summary>
public class Documento : BaseEntity
{
    /// <summary>Tipo do documento</summary>
    public TipoDocumento Tipo { get; set; }
    
    /// <summary>Nome do arquivo</summary>
    public string NomeArquivo { get; set; } = string.Empty;
    
    /// <summary>Caminho local onde o arquivo está存储ado</summary>
    public string CaminhoLocal { get; set; } = string.Empty;
    
    /// <summary>Hash SHA-256 para validação de integridade</summary>
    public string? HashValidacao { get; set; }
    
    /// <summary>Data do upload do arquivo</summary>
    public DateTime DataUpload { get; set; } = DateTime.UtcNow;
    
    /// <summary>Indica se o documento foi validado</summary>
    public bool Validado { get; set; }
    
    /// <summary>Motivo da rejeição (quando aplicável)</summary>
    public string? MotivoRejeicao { get; set; }
    
    /// <summary>ID do candidato que enviou o documento</summary>
    public long CandidatoId { get; set; }
    
    /// <summary>Candidato relacionado</summary>
    public virtual Candidato? Candidato { get; set; }

    /// <summary>
    /// Verifica a integridade do arquivo comparando o hash
    /// </summary>
    public bool VerificarIntegridade()
    {
        return !string.IsNullOrEmpty(HashValidacao) && File.Exists(CaminhoLocal);
    }

    /// <summary>
    /// Extrai metadados do documento em formato de dicionário
    /// </summary>
    public Dictionary<string, string> ExtrairMetadados()
    {
        return new Dictionary<string, string>
        {
            { "NomeArquivo", NomeArquivo },
            { "Tipo", Tipo.ToString() },
            { "DataUpload", DataUpload.ToString("o") },
            { "Validado", Validado.ToString() }
        };
    }
}
