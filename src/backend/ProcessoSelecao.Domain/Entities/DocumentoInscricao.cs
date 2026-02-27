using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

/// <summary>
/// Entidade que representa um documento anexado pelo candidato na inscrição
/// </summary>
public class DocumentoInscricao : BaseEntity
{
    /// <summary>ID da inscrição à qual o documento pertence</summary>
    public long InscricaoId { get; set; }
    
    /// <summary>Tipo do documento anexado</summary>
    public TipoDocumentoInscricao Tipo { get; set; }
    
    /// <summary>Nome original do arquivo enviado pelo candidato</summary>
    public string NomeArquivoOriginal { get; set; } = string.Empty;
    
    /// <summary>Nome do arquivo salvo no servidor (gerado automaticamente)</summary>
    public string NomeArquivoSalvo { get; set; } = string.Empty;
    
    /// <summary>Caminho local onde o arquivo está armazenado</summary>
    public string CaminhoLocal { get; set; } = string.Empty;
    
    /// <summary>Tamanho do arquivo em bytes</summary>
    public long TamanhoBytes { get; set; }
    
    /// <summary>Tipo MIME do arquivo (application/pdf, image/png, etc)</summary>
    public string ContentType { get; set; } = string.Empty;
    
    /// <summary>Hash SHA-256 para validação de integridade</summary>
    public string? HashValidacao { get; set; }
    
    /// <summary>Data/hora do upload do documento</summary>
    public DateTime DataUpload { get; set; } = DateTime.UtcNow;
    
    /// <summary>Inscrição relacionada ao documento</summary>
    public virtual Inscricao? Inscricao { get; set; }
}
