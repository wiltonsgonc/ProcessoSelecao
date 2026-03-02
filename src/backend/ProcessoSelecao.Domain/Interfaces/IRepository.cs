namespace ProcessoSelecao.Domain.Interfaces;

/// <summary>
/// Interface base para repositórios genéricos
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>Retorna uma entidade pelo ID</summary>
    Task<T?> GetByIdAsync(long id);
    
    /// <summary>Retorna todas as entidades</summary>
    Task<IEnumerable<T>> GetAllAsync();
    
    /// <summary>Adiciona uma nova entidade</summary>
    Task<T> AddAsync(T entity);
    
    /// <summary>Atualiza uma entidade existente</summary>
    Task<T> UpdateAsync(T entity);
    
    /// <summary>Remove uma entidade pelo ID</summary>
    Task DeleteAsync(long id);
    
    /// <summary>Verifica se uma entidade existe pelo ID</summary>
    Task<bool> ExistsAsync(long id);
}
