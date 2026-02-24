using Microsoft.EntityFrameworkCore;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Infrastructure.Repositories;

public class DocumentoRepository : IDocumentoRepository
{
    private readonly Data.ApplicationDbContext _context;

    public DocumentoRepository(Data.ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Documento?> GetByIdAsync(long id)
    {
        return await _context.Documentos.FindAsync(id);
    }

    public async Task<IEnumerable<Documento>> GetAllAsync()
    {
        return await _context.Documentos.ToListAsync();
    }

    public async Task<Documento> AddAsync(Documento entity)
    {
        _context.Documentos.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Documento> UpdateAsync(Documento entity)
    {
        entity.DataAtualizacao = DateTime.UtcNow;
        _context.Documentos.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Documentos.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Documentos.AnyAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Documento>> GetByCandidatoIdAsync(long candidatoId)
    {
        return await _context.Documentos
            .Where(d => d.CandidatoId == candidatoId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Documento>> GetPendentesValidacaoAsync()
    {
        return await _context.Documentos
            .Where(d => !d.Validado)
            .ToListAsync();
    }
}
