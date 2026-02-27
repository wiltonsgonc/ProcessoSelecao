using Microsoft.EntityFrameworkCore;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Interfaces;
using ProcessoSelecao.Infrastructure.Data;

namespace ProcessoSelecao.Infrastructure.Repositories;

/// <summary>
/// Repositório para operações com Editais
/// </summary>
public class EditalRepository : RepositoryBase<Edital>, IEditalRepository
{
    private readonly ApplicationDbContext _context;
    public EditalRepository(ApplicationDbContext context) : base(context) => _context = context;

    public async Task<Edital?> GetByIdWithOptionsAsync(int id)
    {
        return await _context.Editais
            .Include(e => e.OpcoesCurso)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Edital>> GetPublishedAsync() {
        return await _context.Editais
            .Include(e => e.OpcoesCurso)
            .Where(e => e.Status == Domain.Enums.StatusEdital.Publicado)
            .OrderByDescending(e => e.DataPublicacao)
            .ToListAsync();
    }
}

/// <summary>
/// Repositório para operações com Opções de Curso
/// </summary>
public class OpcaoCursoRepository : RepositoryBase<OpcaoCurso>, IOpcaoCursoRepository
{
    private readonly ApplicationDbContext _context;
    public OpcaoCursoRepository(ApplicationDbContext context) : base(context) => _context = context;

    public async Task<IEnumerable<OpcaoCurso>> GetByEditalIdAsync(int editalId)
    {
        return await _context.OpcoesCurso
            .Where(o => o.EditalId == editalId)
            .ToListAsync();
    }
}

/// <summary>
/// Repositório para operações com Inscrições
/// </summary>
public class InscricaoRepository : RepositoryBase<Inscricao>, IInscricaoRepository
{
    private readonly ApplicationDbContext _context;
    public InscricaoRepository(ApplicationDbContext context) : base(context) => _context = context;

    public async Task<Inscricao?> GetByIdWithDocumentsAsync(int id)
    {
        return await _context.Inscricoes
            .Include(i => i.Documentos)
            .Include(i => i.Edital)
            .Include(i => i.OpcaoCurso)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Inscricao>> GetByEditalIdAsync(int editalId)
    {
        return await _context.Inscricoes
            .Include(i => i.OpcaoCurso)
            .Where(i => i.EditalId == editalId)
            .OrderByDescending(i => i.DataInscricao)
            .ToListAsync();
    }

    public async Task<Inscricao?> GetByCpfAsync(string cpf, int editalId)
    {
        return await _context.Inscricoes
            .FirstOrDefaultAsync(i => i.NumeroDocumento == cpf && i.EditalId == editalId);
    }
}

/// <summary>
/// Repositório para operações com Documentos de Inscrição
/// </summary>
public class DocumentoInscricaoRepository : RepositoryBase<DocumentoInscricao>, IDocumentoInscricaoRepository
{
    private readonly ApplicationDbContext _context;
    public DocumentoInscricaoRepository(ApplicationDbContext context) : base(context) => _context = context;

    public async Task<IEnumerable<DocumentoInscricao>> GetByInscricaoIdAsync(int inscricaoId)
    {
        return await _context.DocumentosInscricao
            .Where(d => d.InscricaoId == inscricaoId)
            .ToListAsync();
    }
}
