using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

public class Inscricao : BaseEntity
{
    public long EditalId { get; set; }
    public long? OpcaoCursoId { get; set; }
    
    // Página 1 - Dados Iniciais
    public string Nome { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string TipoDocumento { get; set; } = string.Empty; // CPF, RG, Passaporte
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
    public string? AutorizaDadosPessoais { get; set; } // Sim/Não

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

    public DateTime DataInscricao { get; set; } = DateTime.UtcNow;
    public StatusInscricao Status { get; set; } = StatusInscricao.Pendente;

    // Relacionamentos
    public virtual Edital? Edital { get; set; }
    public virtual OpcaoCurso? OpcaoCurso { get; set; }
    public virtual ICollection<DocumentoInscricao> Documentos { get; set; } = new List<DocumentoInscricao>();
}
