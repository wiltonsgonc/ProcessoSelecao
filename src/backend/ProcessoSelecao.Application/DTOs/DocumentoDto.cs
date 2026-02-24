using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Application.DTOs;

public class DocumentoDto
{
    public long Id { get; set; }
    public TipoDocumento Tipo { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public DateTime DataUpload { get; set; }
    public bool Validado { get; set; }
    public string? MotivoRejeicao { get; set; }
    public long CandidatoId { get; set; }
}

public class CreateDocumentoDto
{
    public TipoDocumento Tipo { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public long CandidatoId { get; set; }
}

public class ValidateDocumentoDto
{
    public bool Validado { get; set; }
    public string? MotivoRejeicao { get; set; }
}
