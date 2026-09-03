using Microsoft.EntityFrameworkCore;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Infrastructure.Repositories;

/// <summary>
/// Repositório para operações com Templates de Barema
/// </summary>
public class BaremaTemplateRepository : IBaremaTemplateRepository
{
    private readonly Data.ApplicationDbContext _context;

    public BaremaTemplateRepository(Data.ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BaremaTemplate?> GetByIdAsync(long id)
    {
        return await _context.BaremaTemplates
            .Include(t => t.Itens.OrderBy(i => i.SecaoOrdem).ThenBy(i => i.Ordem))
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<BaremaTemplate>> GetAllAsync()
    {
        return await _context.BaremaTemplates
            .Include(t => t.Itens)
            .OrderBy(t => t.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<BaremaTemplate>> GetAllActiveAsync()
    {
        return await _context.BaremaTemplates
            .Where(t => t.Ativo)
            .Include(t => t.Itens)
            .OrderBy(t => t.Nome)
            .ToListAsync();
    }

    public async Task<BaremaTemplate?> GetWithItemsAsync(long id)
    {
        return await _context.BaremaTemplates
            .Include(t => t.Itens.OrderBy(i => i.SecaoOrdem).ThenBy(i => i.Ordem))
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<BaremaTemplateItem>> GetItemsByTemplateIdAsync(long templateId)
    {
        return await _context.BaremaTemplateItems
            .Where(i => i.TemplateId == templateId)
            .OrderBy(i => i.SecaoOrdem)
            .ThenBy(i => i.Ordem)
            .ToListAsync();
    }

    public async Task<BaremaTemplate> AddAsync(BaremaTemplate entity)
    {
        _context.BaremaTemplates.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<BaremaTemplate> UpdateAsync(BaremaTemplate entity)
    {
        entity.DataAtualizacao = DateTime.UtcNow;
        _context.BaremaTemplates.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.BaremaTemplates.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.BaremaTemplates.AnyAsync(t => t.Id == id);
    }

    public async Task<BaremaTemplate?> CloneAsync(long templateId, string novoNome)
    {
        var original = await GetWithItemsAsync(templateId);
        if (original == null) return null;

        var clone = new BaremaTemplate
        {
            Nome = novoNome,
            Descricao = original.Descricao,
            TipoBarema = original.TipoBarema,
            PontoMaximo = original.PontoMaximo,
            Ativo = true,
            CriadoPor = "Clone"
        };

        foreach (var item in original.Itens)
        {
            clone.Itens.Add(new BaremaTemplateItem
            {
                Secao = item.Secao,
                SecaoOrdem = item.SecaoOrdem,
                Nome = item.Nome,
                Ordem = item.Ordem,
                NotaMinima = item.NotaMinima,
                NotaMaxima = item.NotaMaxima,
                Passo = item.Passo,
                Obrigatorio = item.Obrigatorio
            });
        }

        _context.BaremaTemplates.Add(clone);
        await _context.SaveChangesAsync();
        return clone;
    }
}
