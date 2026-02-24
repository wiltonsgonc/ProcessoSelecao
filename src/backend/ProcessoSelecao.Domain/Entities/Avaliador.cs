using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

public class Avaliador : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TipoAvaliador Tipo { get; set; }
    public string? AreaEspecializacao { get; set; }
    public string? Instituicao { get; set; }
    public bool Ativo { get; set; } = true;
    
    public long? ProcessoSelecaoId { get; set; }
    public virtual ProcessoSelecao? ProcessoSelecao { get; set; }
    
    public virtual ICollection<Barema> Baremas { get; set; } = new List<Barema>();

    public List<Barema> ListarAvaliacoesPendentes()
    {
        return Baremas.Where(b => b.Status == StatusBarema.Pendente || b.Status == StatusBarema.EmPreenchimento).ToList();
    }
}
