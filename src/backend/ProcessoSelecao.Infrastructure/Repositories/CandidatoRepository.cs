using Microsoft.EntityFrameworkCore;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Infrastructure.Repositories;

public class CandidatoRepository : ICandidatoRepository
{
    private readonly Data.ApplicationDbContext _context;

    public CandidatoRepository(Data.ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Candidato?> GetByIdAsync(long id)
    {
        return await _context.Candidatos
            .Include(c => c.Documentos)
            .Include(c => c.Baremas)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Candidato>> GetAllAsync()
    {
        return await _context.Candidatos
            .Include(c => c.Documentos)
            .ToListAsync();
    }

    public async Task<Candidato> AddAsync(Candidato entity)
    {
        _context.Candidatos.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Candidato> UpdateAsync(Candidato entity)
    {
        entity.DataAtualizacao = DateTime.UtcNow;
        _context.Candidatos.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Candidatos.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Candidatos.AnyAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Candidato>> GetByProcessoIdAsync(long processoId)
    {
        return await _context.Candidatos
            .Where(c => c.ProcessoSelecaoId == processoId)
            .Include(c => c.Documentos)
            .ToListAsync();
    }

    public async Task<Candidato?> GetByEmailAsync(string email)
    {
        return await _context.Candidatos.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<Candidato?> GetByMatriculaAsync(string matricula)
    {
        return await _context.Candidatos.FirstOrDefaultAsync(c => c.Matricula == matricula);
    }

    public async Task<IEnumerable<Candidato>> GetByStatusValidacaoAsync(StatusValidacao status)
    {
        return await _context.Candidatos
            .Where(c => c.StatusValidacao == status)
            .ToListAsync();
    }
}
