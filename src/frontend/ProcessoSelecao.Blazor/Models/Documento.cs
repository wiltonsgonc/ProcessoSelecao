namespace ProcessoSelecao.Blazor.Models;

public class Documento
{
    public int Id { get; set; }
    public TipoDocumento Tipo { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public string? DataUpload { get; set; }
    public bool Validado { get; set; }
    public string? MotivoRejeicao { get; set; }
    public int CandidatoId { get; set; }
    public string? CandidatoNome { get; set; }
}

public class CreateDocumento
{
    public TipoDocumento Tipo { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public int CandidatoId { get; set; }
}

public class CreateDocumentoWithUrl
{
    public TipoDocumento Tipo { get; set; }
    public string LinkUrl { get; set; } = string.Empty;
    public int CandidatoId { get; set; }
}

public class ValidateDocumento
{
    public bool Validado { get; set; }
    public string? MotivoRejeicao { get; set; }
}
