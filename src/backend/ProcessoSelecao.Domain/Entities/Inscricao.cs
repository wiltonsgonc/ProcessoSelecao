using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

/// <summary>
/// Entidade que representa uma inscrição de candidato em um Edital
/// </summary>
public class Inscricao : BaseEntity
{
    /// <summary>ID do edital no qual o candidato se inscreveu</summary>
    public long EditalId { get; set; }
    
    /// <summary>ID do curso/opção escolhida pelo candidato</summary>
    public long? OpcaoCursoId { get; set; }
    
    // ============================================
    // Página 1 - Dados Iniciais
    // ============================================
    
    /// <summary>Nome completo do candidato</summary>
    public string Nome { get; set; } = string.Empty;
    
    /// <summary>Data de nascimento</summary>
    public DateTime DataNascimento { get; set; }
    
    /// <summary>Tipo de documento de identificação (CPF, RG, Passaporte)</summary>
    public string TipoDocumento { get; set; } = string.Empty;
    
    /// <summary>Número do documento de identificação</summary>
    public string NumeroDocumento { get; set; } = string.Empty;
    
    /// <summary>E-mail do candidato</summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>Telefone principal</summary>
    public string Telefone1 { get; set; } = string.Empty;
    
    /// <summary>Telefone secundário</summary>
    public string? Telefone2 { get; set; }
    
    /// <summary>Indica se aceitou a política de privacidade</summary>
    public bool AceitaPoliticaPrivacidade { get; set; }

    // ============================================
    // Página 2 - Dados Pessoais
    // ============================================
    
    /// <summary>País de nascimento</summary>
    public string? PaisNatal { get; set; }
    
    /// <summary>Estado de nascimento</summary>
    public string? EstadoNatal { get; set; }
    
    /// <summary>Cidade de nascimento</summary>
    public string? Naturalidade { get; set; }
    
    /// <summary>Nome social (se diferente do nome civil)</summary>
    public string? NomeSocial { get; set; }
    
    /// <summary>Estado civil</summary>
    public string? EstadoCivil { get; set; }
    
    /// <summary>Nacionalidade</summary>
    public string? Nacionalidade { get; set; }
    
    /// <summary>Sexo/Gênero</summary>
    public string? Sexo { get; set; }
    
    /// <summary>Cor/Raça (conforme IBGE)</summary>
    public string? CorRaca { get; set; }
    
    /// <summary>Autoriza uso dos dados pessoais para fins estatísticos</summary>
    public string? AutorizaDadosPessoais { get; set; }

    // ============================================
    // Dados Estrangeiros
    // ============================================
    
    /// <summary>Tipo de visto (para candidatos estrangeiros)</summary>
    public string? TipoVisto { get; set; }
    
    /// <summary> Número do Registro Geral (RG)</summary>
    public string? NumeroRegistroGeral { get; set; }
    
    /// <summary>Data de vencimento do RG</summary>
    public DateTime? DataVencimentoRg { get; set; }

    // ============================================
    // Página 3 - Opção de Interesse
    // ============================================
    
    /// <summary>Forma de inscrição escolhida</summary>
    public FormaInscricao? FormaInscricao { get; set; }
    
    /// <summary>Local de realização da prova</summary>
    public string? LocalRealizacaoProva { get; set; }
    
    /// <summary>Campus de realização da prova</summary>
    public string? CampusRealizacaoProva { get; set; }

    // ============================================
    // Deficiências / Necessidades Especiais
    // ============================================
    
    /// <summary>Deficiência física</summary>
    public bool DefFisica { get; set; }
    
    /// <summary>Deficiência auditiva</summary>
    public bool DefAuditiva { get; set; }
    
    /// <summary>Deficiência na fala</summary>
    public bool DefFala { get; set; }
    
    /// <summary>Deficiência visual</summary>
    public bool DefVisual { get; set; }
    
    /// <summary>Deficiência mental</summary>
    public bool DefMental { get; set; }
    
    /// <summary>Deficiência intelectual</summary>
    public bool DefIntelectual { get; set; }
    
    /// <summary>Pessoa reabilitada</summary>
    public bool DefReabilitado { get; set; }
    
    /// <summary>Deficiência múltipla</summary>
    public bool DefMultipla { get; set; }
    
    /// <summary>Descrição de outras necessidades especiais</summary>
    public string? DefOutrasNecessidades { get; set; }

    /// <summary>Data/hora em que a inscrição foi realizada</summary>
    public DateTime DataInscricao { get; set; } = DateTime.UtcNow;
    
    /// <summary>Status atual da inscrição</summary>
    public StatusInscricao Status { get; set; } = StatusInscricao.Pendente;

    // ============================================
    // Relacionamentos
    // ============================================
    
    /// <summary>Edital relacionado à inscrição</summary>
    public virtual Edital? Edital { get; set; }
    
    /// <summary>Curso/opção escolhida</summary>
    public virtual OpcaoCurso? OpcaoCurso { get; set; }
    
    /// <summary>Documentos anexados na inscrição</summary>
    public virtual ICollection<DocumentoInscricao> Documentos { get; set; } = new List<DocumentoInscricao>();
}
