using Microsoft.EntityFrameworkCore;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;
using ProcessoSelecao.Domain.Interfaces;

namespace ProcessoSelecao.Infrastructure.Repositories;

/// <summary>
/// Repositório para operações com Candidatos
/// </summary>
public class CandidatoRepository : ICandidatoRepository
{
    private readonly Data.ApplicationDbContext _context;

    public CandidatoRepository(Data.ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Retorna um candidato pelo ID com documentos e avaliações</summary>
    public async Task<Candidato?> GetByIdAsync(long id)
    {
        return await _context.Candidatos
            .Include(c => c.Documentos)
            .Include(c => c.Baremas)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>Retorna todos os candidatos</summary>
    public async Task<IEnumerable<Candidato>> GetAllAsync()
    {
        return await _context.Candidatos
            .Include(c => c.Documentos)
            .ToListAsync();
    }

    /// <summary>Adiciona um novo candidato</summary>
    public async Task<Candidato> AddAsync(Candidato entity)
    {
        _context.Candidatos.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>Atualiza um candidato existente</summary>
    public async Task<Candidato> UpdateAsync(Candidato entity)
    {
        entity.DataAtualizacao = DateTime.UtcNow;
        _context.Candidatos.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>Remove um candidato pelo ID</summary>
    public async Task DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Candidatos.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>Verifica se um candidato existe pelo ID</summary>
    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Candidatos.AnyAsync(c => c.Id == id);
    }

    /// <summary>Retorna candidatos de um processo</summary>
    public async Task<IEnumerable<Candidato>> GetByProcessoIdAsync(long processoId)
    {
        return await _context.Candidatos
            .Where(c => c.ProcessoSelecaoId == processoId)
            .Include(c => c.Documentos)
            .ToListAsync();
    }

    /// <summary>Busca candidato por e-mail</summary>
    public async Task<Candidato?> GetByEmailAsync(string email)
    {
        return await _context.Candidatos.FirstOrDefaultAsync(c => c.Email == email);
    }

    /// <summary>Busca candidato por matrícula</summary>
    public async Task<Candidato?> GetByMatriculaAsync(string matricula)
    {
        return await _context.Candidatos.FirstOrDefaultAsync(c => c.Matricula == matricula);
    }

    /// <summary>Retorna candidatos por status de validação</summary>
    public async Task<IEnumerable<Candidato>> GetByStatusValidacaoAsync(StatusValidacao status)
    {
        return await _context.Candidatos
            .Where(c => c.StatusValidacao == status)
            .ToListAsync();
    }
}
