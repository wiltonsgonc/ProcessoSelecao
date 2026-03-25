using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Application.DTOs;

/// <summary>
/// DTO para leitura de dados do Documento
/// </summary>
public class DocumentoDto
{
    public long Id { get; set; }
    public TipoDocumento Tipo { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public DateTime DataUpload { get; set; }
    public bool Validado { get; set; }
    public string? MotivoRejeicao { get; set; }
    public long CandidatoId { get; set; }
    public string? CandidatoNome { get; set; }
}

/// <summary>
/// DTO para criação de novo Documento
/// </summary>
public class CreateDocumentoDto
{
    public TipoDocumento Tipo { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public long CandidatoId { get; set; }
}

/// <summary>
/// DTO para validação de Documento
/// </summary>
public class ValidateDocumentoDto
{
    public bool Validado { get; set; }
    public string? MotivoRejeicao { get; set; }
}
