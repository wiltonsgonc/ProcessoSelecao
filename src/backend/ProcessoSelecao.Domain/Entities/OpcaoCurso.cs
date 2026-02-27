using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

/// <summary>
/// Entidade que representa uma opção de curso dentro de um Edital
/// </summary>
public class OpcaoCurso : BaseEntity
{
    /// <summary>ID do edital ao qual esta opção pertence</summary>
    public long EditalId { get; set; }
    
    /// <summary>Nome do curso/opção</summary>
    public string Nome { get; set; } = string.Empty;
    
    /// <summary>Descrição detalhada do curso</summary>
    public string? Descricao { get; set; }
    
    /// <summary>Número de vagas disponíveis</summary>
    public int Vagas { get; set; }
    
    /// <summary>Campus onde o curso será ofertado</summary>
    public string? Campus { get; set; }
    
    /// <summary>Local onde será aplicada a prova para este curso</summary>
    public string? LocalProva { get; set; }
    
    /// <summary>Edital relacionado</summary>
    public virtual Edital? Edital { get; set; }
    
    /// <summary>Inscrições realizadas para este curso</summary>
    public virtual ICollection<Inscricao> Inscricoes { get; set; } = new List<Inscricao>();
}
