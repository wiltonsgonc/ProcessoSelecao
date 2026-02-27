using ProcessoSelecao.Domain.Entities;

namespace ProcessoSelecao.Domain.Interfaces;

public interface IEditalRepository : IRepository<Edital>
{
    Task<Edital?> GetByIdWithOptionsAsync(int id);
    Task<IEnumerable<Edital>> GetPublishedAsync();
}

public interface IOpcaoCursoRepository : IRepository<OpcaoCurso>
{
    Task<IEnumerable<OpcaoCurso>> GetByEditalIdAsync(int editalId);
}

public interface IInscricaoRepository : IRepository<Inscricao>
{
    Task<Inscricao?> GetByIdWithDocumentsAsync(int id);
    Task<IEnumerable<Inscricao>> GetByEditalIdAsync(int editalId);
    Task<Inscricao?> GetByCpfAsync(string cpf, int editalId);
}

public interface IDocumentoInscricaoRepository : IRepository<DocumentoInscricao>
{
    Task<IEnumerable<DocumentoInscricao>> GetByInscricaoIdAsync(int inscricaoId);
}
