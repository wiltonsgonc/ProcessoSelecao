using Microsoft.EntityFrameworkCore;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Infrastructure.Repositories;

public class BaremaRepository : IBaremaRepository
{
    private readonly Data.ApplicationDbContext _context;

    public BaremaRepository(Data.ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Barema?> GetByIdAsync(long id)
    {
        return await _context.Baremas
            .Include(b => b.Candidato)
            .Include(b => b.Avaliador)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Barema>> GetAllAsync()
    {
        return await _context.Baremas
            .Include(b => b.Candidato)
            .Include(b => b.Avaliador)
            .ToListAsync();
    }

    public async Task<Barema> AddAsync(Barema entity)
    {
        _context.Baremas.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Barema> UpdateAsync(Barema entity)
    {
        entity.DataAtualizacao = DateTime.UtcNow;
        _context.Baremas.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Baremas.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Baremas.AnyAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Barema>> GetByCandidatoIdAsync(long candidatoId)
    {
        return await _context.Baremas
            .Where(b => b.CandidatoId == candidatoId)
            .Include(b => b.Avaliador)
            .ToListAsync();
    }

    public async Task<IEnumerable<Barema>> GetByAvaliadorIdAsync(long avaliadorId)
    {
        return await _context.Baremas
            .Where(b => b.AvaliadorId == avaliadorId)
            .Include(b => b.Candidato)
            .ToListAsync();
    }

    public async Task<IEnumerable<Barema>> GetPendentesAsync()
    {
        return await _context.Baremas
            .Where(b => b.Status == StatusBarema.Pendente || b.Status == StatusBarema.EmPreenchimento)
            .Include(b => b.Candidato)
            .Include(b => b.Avaliador)
            .ToListAsync();
    }
}
