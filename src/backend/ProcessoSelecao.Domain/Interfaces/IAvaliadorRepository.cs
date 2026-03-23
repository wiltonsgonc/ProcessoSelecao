using ProcessoSelecao.Domain.Entities;

namespace ProcessoSelecao.Domain.Interfaces;

/// <summary>
/// Interface para operações com Avaliadores
/// </summary>
public interface IAvaliadorRepository : IRepository<Avaliador>
{
    /// <summary>Retorna todos os avaliadores de um processo</summary>
    Task<IEnumerable<Avaliador>> GetByProcessoIdAsync(long processoId);
    
    /// <summary>Busca avaliador pelo e-mail</summary>
    Task<Avaliador?> GetByEmailAsync(string email);
    
    /// <summary>Retorna apenas avaliadores ativos</summary>
    Task<IEnumerable<Avaliador>> GetAtivosAsync();
}
