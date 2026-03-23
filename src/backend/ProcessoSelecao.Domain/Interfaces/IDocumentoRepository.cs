using ProcessoSelecao.Domain.Entities;

namespace ProcessoSelecao.Domain.Interfaces;

/// <summary>
/// Interface para operações com Documentos
/// </summary>
public interface IDocumentoRepository : IRepository<Documento>
{
    /// <summary>Retorna todos os documentos de um candidato</summary>
    Task<IEnumerable<Documento>> GetByCandidatoIdAsync(long candidatoId);
    
    /// <summary>Retorna documentos pendentes de validação</summary>
    Task<IEnumerable<Documento>> GetPendentesValidacaoAsync();
}
