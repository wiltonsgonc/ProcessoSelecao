using ProcessoSelecao.Domain.Entities;

namespace ProcessoSelecao.Domain.Interfaces;

public interface IBaremaRepository : IRepository<Barema>
{
    Task<IEnumerable<Barema>> GetByCandidatoIdAsync(long candidatoId);
    Task<IEnumerable<Barema>> GetByAvaliadorIdAsync(long avaliadorId);
    Task<IEnumerable<Barema>> GetPendentesAsync();
}
