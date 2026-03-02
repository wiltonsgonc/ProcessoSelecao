using Microsoft.EntityFrameworkCore;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Infrastructure.Repositories;

/// <summary>
/// Repositório para operações com Avaliadores
/// </summary>
public class AvaliadorRepository : IAvaliadorRepository
{
    private readonly Data.ApplicationDbContext _context;

    public AvaliadorRepository(Data.ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Retorna um avaliador pelo ID com suas avaliações</summary>
    public async Task<Avaliador?> GetByIdAsync(long id)
    {
        return await _context.Avaliadores
            .Include(a => a.Baremas)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    /// <summary>Retorna todos os avaliadores</summary>
    public async Task<IEnumerable<Avaliador>> GetAllAsync()
    {
        return await _context.Avaliadores.ToListAsync();
    }

    /// <summary>Adiciona um novo avaliador</summary>
    public async Task<Avaliador> AddAsync(Avaliador entity)
    {
        _context.Avaliadores.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>Atualiza um avaliador existente</summary>
    public async Task<Avaliador> UpdateAsync(Avaliador entity)
    {
        entity.DataAtualizacao = DateTime.UtcNow;
        _context.Avaliadores.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>Remove um avaliador pelo ID</summary>
    public async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Avaliadores.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>Verifica se um avaliador existe pelo ID</summary>
    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Avaliadores.AnyAsync(a => a.Id == id);
    }

    /// <summary>Retorna avaliadores de um processo</summary>
    public async Task<IEnumerable<Avaliador>> GetByProcessoIdAsync(long processoId)
    {
        return await _context.Avaliadores
            .Where(a => a.ProcessoSelecaoId == processoId)
            .ToListAsync();
    }

    /// <summary>Busca avaliador por e-mail</summary>
    public async Task<Avaliador?> GetByEmailAsync(string email)
    {
        return await _context.Avaliadores.FirstOrDefaultAsync(a => a.Email == email);
    }

    /// <summary>Retorna apenas avaliadores ativos</summary>
    public async Task<IEnumerable<Avaliador>> GetAtivosAsync()
    {
        return await _context.Avaliadores
            .Where(a => a.Ativo)
            .ToListAsync();
    }
}
