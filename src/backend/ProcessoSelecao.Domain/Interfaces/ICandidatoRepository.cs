using ProcessoSelecao.Domain.Entities;

namespace ProcessoSelecao.Domain.Interfaces;

public interface ICandidatoRepository : IRepository<Candidato>
{
    Task<IEnumerable<Candidato>> GetByProcessoIdAsync(long processoId);
    Task<Candidato?> GetByEmailAsync(string email);
    Task<Candidato?> GetByMatriculaAsync(string matricula);
    Task<IEnumerable<Candidato>> GetByStatusValidacaoAsync(Domain.Enums.StatusValidacao status);
}
