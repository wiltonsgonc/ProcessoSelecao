using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

/// <summary>
/// Entidade que representa um avaliador/examinador do processo
/// </summary>
public class Avaliador : BaseEntity
{
    /// <summary>Nome completo do avaliador</summary>
    public string Nome { get; set; } = string.Empty;
    
    /// <summary>CPF do avaliador</summary>
    public string Cpf { get; set; } = string.Empty;
    
    /// <summary>E-mail do avaliador</summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>Tipo de avaliador (Interno ou Externo)</summary>
    public TipoAvaliador Tipo { get; set; }
    
    /// <summary>Área de especialização</summary>
    public string? AreaEspecializacao { get; set; }
    
    /// <summary>Instituição vinculada</summary>
    public string? Instituicao { get; set; }
    
    /// <summary>Indica se o avaliador está ativo no sistema</summary>
    public bool Ativo { get; set; } = true;
    
    /// <summary>Hash da senha do avaliador (para login)</summary>
    public string? SenhaHash { get; set; }
    
    /// <summary>ID do processo de seleção (opcional)</summary>
    public long? ProcessoSelecaoId { get; set; }
    
    /// <summary>Processo de seleção relacionado</summary>
    public virtual ProcessoSelecao? ProcessoSelecao { get; set; }
    
    /// <summary>Avaliações realizadas por este avaliador</summary>
    public virtual ICollection<Barema> Baremas { get; set; } = new List<Barema>();

    /// <summary>Link do currículo Lattes</summary>
    public string? LinkLattes { get; set; }

    /// <summary>Última formação acadêmica</summary>
    public string? UltimaFormacao { get; set; }

    /// <summary>Cargo atual</summary>
    public string? Cargo { get; set; }

    /// <summary>Nível de pesquisador/bolsista do CNPq</summary>
    public NivelCnpq NivelCnpq { get; set; } = NivelCnpq.NaoSeAplica;

    /// <summary>
    /// Lista avaliações pendentes do avaliador
    /// </summary>
    public List<Barema> ListarAvaliacoesPendentes()
    {
        return Baremas.Where(b => b.Status == StatusBarema.Pendente || b.Status == StatusBarema.EmPreenchimento).ToList();
    }
}