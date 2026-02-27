using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

public class DocumentoInscricao : BaseEntity
{
    public long InscricaoId { get; set; }
    public TipoDocumentoInscricao Tipo { get; set; }
    public string NomeArquivoOriginal { get; set; } = string.Empty;
    public string NomeArquivoSalvo { get; set; } = string.Empty;
    public string CaminhoLocal { get; set; } = string.Empty;
    public long TamanhoBytes { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string? HashValidacao { get; set; }
    public DateTime DataUpload { get; set; } = DateTime.UtcNow;
    
    public virtual Inscricao? Inscricao { get; set; }
}
