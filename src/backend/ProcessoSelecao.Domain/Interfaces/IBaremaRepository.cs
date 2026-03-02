using ProcessoSelecao.Domain.Entities;

namespace ProcessoSelecao.Domain.Interfaces;

/// <summary>
/// Interface para operações com Baremas/Avaliações
/// </summary>
public interface IBaremaRepository : IRepository<Barema>
{
    /// <summary>Retorna todas as avaliações de um candidato</summary>
    Task<IEnumerable<Barema>> GetByCandidatoIdAsync(long candidatoId);
    
    /// <summary>Retorna todas as avaliações de um avaliador</summary>
    Task<IEnumerable<Barema>> GetByAvaliadorIdAsync(long avaliadorId);
    
    /// <summary>Retorna avaliações pendentes</summary>
    Task<IEnumerable<Barema>> GetPendentesAsync();
}
