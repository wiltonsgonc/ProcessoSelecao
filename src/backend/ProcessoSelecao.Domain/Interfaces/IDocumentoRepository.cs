using ProcessoSelecao.Domain.Entities;

namespace ProcessoSelecao.Domain.Interfaces;

public interface IDocumentoRepository : IRepository<Documento>
{
    Task<IEnumerable<Documento>> GetByCandidatoIdAsync(long candidatoId);
    Task<IEnumerable<Documento>> GetPendentesValidacaoAsync();
}
