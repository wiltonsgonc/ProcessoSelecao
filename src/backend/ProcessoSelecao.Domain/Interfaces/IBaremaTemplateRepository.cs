using ProcessoSelecao.Domain.Entities;

namespace ProcessoSelecao.Domain.Interfaces;

/// <summary>
/// Interface para acesso a dados de templates de barema
/// </summary>
public interface IBaremaTemplateRepository : IRepository<BaremaTemplate>
{
    Task<IEnumerable<BaremaTemplate>> GetAllActiveAsync();
    Task<BaremaTemplate?> GetWithItemsAsync(long id);
    Task<IEnumerable<BaremaTemplateItem>> GetItemsByTemplateIdAsync(long templateId);
    Task<BaremaTemplate?> CloneAsync(long templateId, string novoNome);
}
