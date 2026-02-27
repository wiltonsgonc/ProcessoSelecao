using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Application.DTOs;

/// <summary>
/// DTO para leitura de dados da Inscrição
/// </summary>
public class InscricaoDto
{
    public int Id { get; set; }
    public int EditalId { get; set; }
    public string? NomeEdital { get; set; }
    public int? OpcaoCursoId { get; set; }
    public string? NomeOpcaoCurso { get; set; }
    
    // Página 1 - Dados Iniciais
    public string Nome { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string TipoDocumento { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone1 { get; set; } = string.Empty;
    public string? Telefone2 { get; set; }
    public bool AceitaPoliticaPrivacidade { get; set; }
    
    // Página 2 - Dados Pessoais
    public string? PaisNatal { get; set; }
    public string? EstadoNatal { get; set; }
    public string? Naturalidade { get; set; }
    public string? NomeSocial { get; set; }
    public string? EstadoCivil { get; set; }
    public string? Nacionalidade { get; set; }
    public string? Sexo { get; set; }
    public string? CorRaca { get; set; }
    public string? AutorizaDadosPessoais { get; set; }
    
    // Dados Estrangeiros
    public string? TipoVisto { get; set; }
    public string? NumeroRegistroGeral { get; set; }
    public DateTime? DataVencimentoRg { get; set; }
    
    // Página 3 - Opção de Interesse
    public FormaInscricao? FormaInscricao { get; set; }
    public string? LocalRealizacaoProva { get; set; }
    public string? CampusRealizacaoProva { get; set; }
    
    // Deficiências
    public bool DefFisica { get; set; }
    public bool DefAuditiva { get; set; }
    public bool DefFala { get; set; }
    public bool DefVisual { get; set; }
    public bool DefMental { get; set; }
    public bool DefIntelectual { get; set; }
    public bool DefReabilitado { get; set; }
    public bool DefMultipla { get; set; }
    public string? DefOutrasNecessidades { get; set; }
    
    public DateTime DataInscricao { get; set; }
    public StatusInscricao Status { get; set; }
    public List<DocumentoInscricaoDto> Documentos { get; set; } = new();
}

/// <summary>
/// DTO para criação de nova Inscrição
/// </summary>
public class InscricaoCreateDto
{
    public int EditalId { get; set; }
    public int? OpcaoCursoId { get; set; }
    
    // Página 1 - Dados Iniciais
    public string Nome { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string TipoDocumento { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone1 { get; set; } = string.Empty;
    public string? Telefone2 { get; set; }
    public bool AceitaPoliticaPrivacidade { get; set; }
    
    // Página 2 - Dados Pessoais
    public string? PaisNatal { get; set; }
    public string? EstadoNatal { get; set; }
    public string? Naturalidade { get; set; }
    public string? NomeSocial { get; set; }
    public string? EstadoCivil { get; set; }
    public string? Nacionalidade { get; set; }
    public string? Sexo { get; set; }
    public string? CorRaca { get; set; }
    public string? AutorizaDadosPessoais { get; set; }
    
    // Dados Estrangeiros
    public string? TipoVisto { get; set; }
    public string? NumeroRegistroGeral { get; set; }
    public DateTime? DataVencimentoRg { get; set; }
    
    // Página 3 - Opção de Interesse
    public FormaInscricao? FormaInscricao { get; set; }
    public string? LocalRealizacaoProva { get; set; }
    public string? CampusRealizacaoProva { get; set; }
    
    // Deficiências
    public bool DefFisica { get; set; }
    public bool DefAuditiva { get; set; }
    public bool DefFala { get; set; }
    public bool DefVisual { get; set; }
    public bool DefMental { get; set; }
    public bool DefIntelectual { get; set; }
    public bool DefReabilitado { get; set; }
    public bool DefMultipla { get; set; }
    public string? DefOutrasNecessidades { get; set; }
}

/// <summary>
/// DTO para leitura de dados do Documento de Inscrição
/// </summary>
public class DocumentoInscricaoDto
{
    public long Id { get; set; }
    public long InscricaoId { get; set; }
    public TipoDocumentoInscricao Tipo { get; set; }
    public string NomeArquivoOriginal { get; set; } = string.Empty;
    public long TamanhoBytes { get; set; }
    public DateTime DataUpload { get; set; }
}
