using Microsoft.EntityFrameworkCore;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Infrastructure.Repositories;

/// <summary>
/// Repositório para operações com Documentos
/// </summary>
public class DocumentoRepository : IDocumentoRepository
{
    private readonly Data.ApplicationDbContext _context;

    public DocumentoRepository(Data.ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Retorna um documento pelo ID</summary>
    public async Task<Documento?> GetByIdAsync(long id)
    {
        return await _context.Documentos.FindAsync(id);
    }

    /// <summary>Retorna todos os documentos</summary>
    public async Task<IEnumerable<Documento>> GetAllAsync()
    {
        return await _context.Documentos.ToListAsync();
    }

    /// <summary>Adiciona um novo documento</summary>
    public async Task<Documento> AddAsync(Documento entity)
    {
        _context.Documentos.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>Atualiza um documento existente</summary>
    public async Task<Documento> UpdateAsync(Documento entity)
    {
        entity.DataAtualizacao = DateTime.UtcNow;
        _context.Documentos.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>Remove um documento pelo ID</summary>
    public async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Documentos.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>Verifica se um documento existe pelo ID</summary>
    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Documentos.AnyAsync(d => d.Id == id);
    }

    /// <summary>Retorna documentos de um candidato</summary>
    public async Task<IEnumerable<Documento>> GetByCandidatoIdAsync(long candidatoId)
    {
        return await _context.Documentos
            .Where(d => d.CandidatoId == candidatoId)
            .ToListAsync();
    }

    /// <summary>Retorna documentos pendentes de validação</summary>
    public async Task<IEnumerable<Documento>> GetPendentesValidacaoAsync()
    {
        return await _context.Documentos
            .Where(d => !d.Validado)
            .ToListAsync();
    }
}
