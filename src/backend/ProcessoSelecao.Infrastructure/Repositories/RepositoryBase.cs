using Microsoft.EntityFrameworkCore;
using ProcessoSelecao.Domain.Interfaces;
using ProcessoSelecao.Infrastructure.Data;

namespace ProcessoSelecao.Infrastructure.Repositories;

/// <summary>
/// Classe base para repositórios com operações CRUD genéricas
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public abstract class RepositoryBase<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    protected RepositoryBase(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <summary>Retorna uma entidade pelo ID</summary>
    public virtual async Task<T?> GetByIdAsync(long id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>Retorna todas as entidades</summary>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>Adiciona uma nova entidade</summary>
    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>Atualiza uma entidade existente</summary>
    public virtual async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>Remove uma entidade pelo ID</summary>
    public virtual async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>Verifica se uma entidade existe pelo ID</summary>
    public virtual async Task<bool> ExistsAsync(long id)
    {
        return await _dbSet.FindAsync(id) != null;
    }
}
