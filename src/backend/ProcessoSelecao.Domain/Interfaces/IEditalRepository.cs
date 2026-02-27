using ProcessoSelecao.Domain.Entities;

namespace ProcessoSelecao.Domain.Interfaces;

/// <summary>
/// Repositório para operações com Editais
/// </summary>
public interface IEditalRepository : IRepository<Edital>
{
    /// <summary>Retorna um edital com suas opções de curso</summary>
    Task<Edital?> GetByIdWithOptionsAsync(int id);
    
    /// <summary>Retorna todos os editais publicados</summary>
    Task<IEnumerable<Edital>> GetPublishedAsync();
}

/// <summary>
/// Repositório para operações com Opções de Curso
/// </summary>
public interface IOpcaoCursoRepository : IRepository<OpcaoCurso>
{
    /// <summary>Retorna todas as opções de curso de um edital</summary>
    Task<IEnumerable<OpcaoCurso>> GetByEditalIdAsync(int editalId);
}

/// <summary>
/// Repositório para operações com Inscrições
/// </summary>
public interface IInscricaoRepository : IRepository<Inscricao>
{
    /// <summary>Retorna uma inscrição com seus documentos</summary>
    Task<Inscricao?> GetByIdWithDocumentsAsync(int id);
    
    /// <summary>Retorna todas as inscrições de um edital</summary>
    Task<IEnumerable<Inscricao>> GetByEditalIdAsync(int editalId);
    
    /// <summary>Busca uma inscrição pelo CPF e ID do edital</summary>
    Task<Inscricao?> GetByCpfAsync(string cpf, int editalId);
}

/// <summary>
/// Repositório para operações com Documentos de Inscrição
/// </summary>
public interface IDocumentoInscricaoRepository : IRepository<DocumentoInscricao>
{
    /// <summary>Retorna todos os documentos de uma inscrição</summary>
    Task<IEnumerable<DocumentoInscricao>> GetByInscricaoIdAsync(int inscricaoId);
}
