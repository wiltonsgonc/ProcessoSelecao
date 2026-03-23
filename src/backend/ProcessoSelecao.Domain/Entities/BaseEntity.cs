namespace ProcessoSelecao.Domain.Entities;

/// <summary>
/// Entidade base com campos comuns a todas as entidades
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Identificador único</summary>
    public long Id { get; set; }
    
    /// <summary>Data de criação do registro</summary>
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    
    /// <summary>Data da última atualização</summary>
    public DateTime? DataAtualizacao { get; set; }
}
