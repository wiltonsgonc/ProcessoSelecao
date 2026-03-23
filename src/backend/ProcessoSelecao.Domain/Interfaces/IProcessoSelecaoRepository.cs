namespace ProcessoSelecao.Domain.Interfaces;

/// <summary>
/// Interface para operações com Processos de Seleção
/// </summary>
public interface IProcessoSelecaoRepository : IRepository<ProcessoSelecao.Domain.Entities.ProcessoSelecao>
{
    /// <summary>Retorna apenas processos ativos</summary>
    Task<IEnumerable<ProcessoSelecao.Domain.Entities.ProcessoSelecao>> GetAtivosAsync();
    
    /// <summary>Retorna um processo com seus candidatos</summary>
    Task<ProcessoSelecao.Domain.Entities.ProcessoSelecao?> GetWithCandidatosAsync(long id);
}
