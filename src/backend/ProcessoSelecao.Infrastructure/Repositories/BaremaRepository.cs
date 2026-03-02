using Microsoft.EntityFrameworkCore;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Infrastructure.Repositories;

/// <summary>
/// Repositório para operações com Baremas/Avaliações
/// </summary>
public class BaremaRepository : IBaremaRepository
{
    private readonly Data.ApplicationDbContext _context;

    public BaremaRepository(Data.ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Retorna um barema pelo ID com candidato e avaliador</summary>
    public async Task<Barema?> GetByIdAsync(long id)
    {
        return await _context.Baremas
            .Include(b => b.Candidato)
            .Include(b => b.Avaliador)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    /// <summary>Retorna todos os baremas</summary>
    public async Task<IEnumerable<Barema>> GetAllAsync()
    {
        return await _context.Baremas
            .Include(b => b.Candidato)
            .Include(b => b.Avaliador)
            .ToListAsync();
    }

    /// <summary>Adiciona um novo barema</summary>
    public async Task<Barema> AddAsync(Barema entity)
    {
        _context.Baremas.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>Atualiza um barema existente</summary>
    public async Task<Barema> UpdateAsync(Barema entity)
    {
        entity.DataAtualizacao = DateTime.UtcNow;
        _context.Baremas.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>Remove um barema pelo ID</summary>
    public async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Baremas.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>Verifica se um barema existe pelo ID</summary>
    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Baremas.AnyAsync(b => b.Id == id);
    }

    /// <summary>Retorna baremas de um candidato</summary>
    public async Task<IEnumerable<Barema>> GetByCandidatoIdAsync(long candidatoId)
    {
        return await _context.Baremas
            .Where(b => b.CandidatoId == candidatoId)
            .Include(b => b.Avaliador)
            .ToListAsync();
    }

    /// <summary>Retorna baremas de um avaliador</summary>
    public async Task<IEnumerable<Barema>> GetByAvaliadorIdAsync(long avaliadorId)
    {
        return await _context.Baremas
            .Where(b => b.AvaliadorId == avaliadorId)
            .Include(b => b.Candidato)
            .ToListAsync();
    }

    /// <summary>Retorna baremas pendentes</summary>
    public async Task<IEnumerable<Barema>> GetPendentesAsync()
    {
        return await _context.Baremas
            .Where(b => b.Status == StatusBarema.Pendente || b.Status == StatusBarema.EmPreenchimento)
            .Include(b => b.Candidato)
            .Include(b => b.Avaliador)
            .ToListAsync();
    }
}
