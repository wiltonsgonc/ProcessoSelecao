using ProcessoSelecao.Domain.Entities;

namespace ProcessoSelecao.Domain.Interfaces;

public interface IAvaliadorRepository : IRepository<Avaliador>
{
    Task<IEnumerable<Avaliador>> GetByProcessoIdAsync(long processoId);
    Task<Avaliador?> GetByEmailAsync(string email);
    Task<IEnumerable<Avaliador>> GetAtivosAsync();
}
