using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

public class OpcaoCurso : BaseEntity
{
    public long EditalId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int Vagas { get; set; }
    public string? Campus { get; set; }
    public string? LocalProva { get; set; }
    
    public virtual Edital? Edital { get; set; }
    public virtual ICollection<Inscricao> Inscricoes { get; set; } = new List<Inscricao>();
}
