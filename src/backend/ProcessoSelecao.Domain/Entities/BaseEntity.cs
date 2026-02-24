namespace ProcessoSelecao.Domain.Entities;

public abstract class BaseEntity
{
    public long Id { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataAtualizacao { get; set; }
}
