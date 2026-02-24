using Microsoft.EntityFrameworkCore;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Infrastructure.Repositories;

public class AvaliadorRepository : IAvaliadorRepository
{
    private readonly Data.ApplicationDbContext _context;

    public AvaliadorRepository(Data.ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Avaliador?> GetByIdAsync(long id)
    {
        return await _context.Avaliadores
            .Include(a => a.Baremas)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Avaliador>> GetAllAsync()
    {
        return await _context.Avaliadores.ToListAsync();
    }

    public async Task<Avaliador> AddAsync(Avaliador entity)
    {
        _context.Avaliadores.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Avaliador> UpdateAsync(Avaliador entity)
    {
        entity.DataAtualizacao = DateTime.UtcNow;
        _context.Avaliadores.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Avaliadores.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Avaliadores.AnyAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Avaliador>> GetByProcessoIdAsync(long processoId)
    {
        return await _context.Avaliadores
            .Where(a => a.ProcessoSelecaoId == processoId)
            .ToListAsync();
    }

    public async Task<Avaliador?> GetByEmailAsync(string email)
    {
        return await _context.Avaliadores.FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<IEnumerable<Avaliador>> GetAtivosAsync()
    {
        return await _context.Avaliadores
            .Where(a => a.Ativo)
            .ToListAsync();
    }
}
