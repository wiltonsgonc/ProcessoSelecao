using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

public class Documento : BaseEntity
{
    public TipoDocumento Tipo { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public string CaminhoLocal { get; set; } = string.Empty;
    public string? HashValidacao { get; set; }
    public DateTime DataUpload { get; set; } = DateTime.UtcNow;
    public bool Validado { get; set; }
    public string? MotivoRejeicao { get; set; }
    
    public long CandidatoId { get; set; }
    public virtual Candidato? Candidato { get; set; }

    public bool VerificarIntegridade()
    {
        return !string.IsNullOrEmpty(HashValidacao) && File.Exists(CaminhoLocal);
    }

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
