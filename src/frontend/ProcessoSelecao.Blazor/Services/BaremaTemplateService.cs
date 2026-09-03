using ProcessoSelecao.Blazor.Models;

namespace ProcessoSelecao.Blazor.Services;

public class BaremaTemplateService
{
    private readonly ApiService _api;
    private const string Endpoint = "barematemplates";

    public BaremaTemplateService(ApiService api)
    {
        _api = api;
    }

    public virtual async Task<List<BaremaTemplate>> GetAllAsync()
        => await _api.GetAsync<List<BaremaTemplate>>(Endpoint) ?? new();

    public virtual async Task<List<BaremaTemplate>> GetActiveAsync()
        => await _api.GetAsync<List<BaremaTemplate>>($"{Endpoint}/ativas") ?? new();

    public virtual async Task<BaremaTemplate?> GetByIdAsync(long id)
        => await _api.GetAsync<BaremaTemplate>($"{Endpoint}/{id}");

    public virtual async Task<BaremaTemplate?> CreateAsync(CreateBaremaTemplate dto)
        => await _api.PostAsync<BaremaTemplate>(Endpoint, dto);

    public virtual async Task<BaremaTemplate?> UpdateAsync(long id, UpdateBaremaTemplate dto)
        => await _api.PutAsync<BaremaTemplate>($"{Endpoint}/{id}", dto);

    public virtual async Task<BaremaTemplate?> CloneAsync(long templateId, string novoNome)
        => await _api.PostAsync<BaremaTemplate>($"{Endpoint}/{templateId}/clone", new { Nome = novoNome });

    public virtual async Task DeleteAsync(long id)
        => await _api.DeleteAsync($"{Endpoint}/{id}");

    public virtual async Task<BaremaTemplate?> ToggleAtivoAsync(long id)
        => await _api.PutAsync<BaremaTemplate>($"{Endpoint}/{id}/toggle-ativo", new { });
}
