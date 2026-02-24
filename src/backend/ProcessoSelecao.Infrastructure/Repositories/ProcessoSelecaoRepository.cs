using Microsoft.EntityFrameworkCore;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;
using DomainEntities = ProcessoSelecao.Domain.Entities;

namespace ProcessoSelecao.Infrastructure.Repositories;

public class ProcessoSelecaoRepository : IProcessoSelecaoRepository
{
    private readonly Data.ApplicationDbContext _context;

    public ProcessoSelecaoRepository(Data.ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DomainEntities.ProcessoSelecao?> GetByIdAsync(long id)
    {
        return await _context.ProcessosSelecao
            .Include(p => p.Candidatos)
            .Include(p => p.Avaliadores)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<DomainEntities.ProcessoSelecao>> GetAllAsync()
    {
        return await _context.ProcessosSelecao.ToListAsync();
    }

    public async Task<DomainEntities.ProcessoSelecao> AddAsync(DomainEntities.ProcessoSelecao entity)
    {
        _context.ProcessosSelecao.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<DomainEntities.ProcessoSelecao> UpdateAsync(DomainEntities.ProcessoSelecao entity)
    {
        entity.DataAtualizacao = DateTime.UtcNow;
        _context.ProcessosSelecao.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.ProcessosSelecao.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.ProcessosSelecao.AnyAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<DomainEntities.ProcessoSelecao>> GetAtivosAsync()
    {
        return await _context.ProcessosSelecao
            .Where(p => p.Status == StatusProcesso.Aberto || p.Status == StatusProcesso.EmAndamento)
            .ToListAsync();
    }

    public async Task<DomainEntities.ProcessoSelecao?> GetWithCandidatosAsync(long id)
    {
        return await _context.ProcessosSelecao
            .Include(p => p.Candidatos)
                .ThenInclude(c => c.Documentos)
            .Include(p => p.Candidatos)
                .ThenInclude(c => c.Baremas)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
