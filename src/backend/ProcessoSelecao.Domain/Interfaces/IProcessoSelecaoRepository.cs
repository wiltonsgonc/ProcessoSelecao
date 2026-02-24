namespace ProcessoSelecao.Domain.Interfaces;

public interface IProcessoSelecaoRepository : IRepository<ProcessoSelecao.Domain.Entities.ProcessoSelecao>
{
    Task<IEnumerable<ProcessoSelecao.Domain.Entities.ProcessoSelecao>> GetAtivosAsync();
    Task<ProcessoSelecao.Domain.Entities.ProcessoSelecao?> GetWithCandidatosAsync(long id);
}
