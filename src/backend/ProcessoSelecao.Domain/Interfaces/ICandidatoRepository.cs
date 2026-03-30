using ProcessoSelecao.Domain.Entities;

namespace ProcessoSelecao.Domain.Interfaces;

/// <summary>
/// Interface para operações com Candidatos
/// </summary>
public interface ICandidatoRepository : IRepository<Candidato>
{
    /// <summary>Retorna todos os candidatos de um processo</summary>
    Task<IEnumerable<Candidato>> GetByProcessoIdAsync(long processoId);
    
    /// <summary>Busca candidato pelo e-mail</summary>
    Task<Candidato?> GetByEmailAsync(string email);
    
    /// <summary>Busca candidato pelo CPF</summary>
    Task<Candidato?> GetByCpfAsync(string cpf);
    
    /// <summary>Retorna candidatos por status de validação</summary>
    Task<IEnumerable<Candidato>> GetByStatusValidacaoAsync(Domain.Enums.StatusValidacao status);
}
