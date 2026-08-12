namespace ProcessoSelecao.Blazor.Services;

public class ProcessoSelecaoService
{
    private readonly ApiService _api;
    private const string Endpoint = "processosselecao";

    public ProcessoSelecaoService(ApiService api)
    {
        _api = api;
    }

    public virtual async Task<List<Models.ProcessoSelecao>> GetAllAsync()
        => await _api.GetAsync<List<Models.ProcessoSelecao>>(Endpoint) ?? new();

    public virtual async Task<Models.ProcessoSelecao?> GetByIdAsync(int id)
        => await _api.GetAsync<Models.ProcessoSelecao>($"{Endpoint}/{id}");

    public virtual async Task<Models.ProcessoSelecao?> CreateAsync(Models.CreateProcessoSelecao dto)
        => await _api.PostAsync<Models.ProcessoSelecao>(Endpoint, dto);

    public virtual async Task<Models.ProcessoSelecao?> UpdateAsync(int id, Models.UpdateProcessoSelecao dto)
        => await _api.PutAsync<Models.ProcessoSelecao>($"{Endpoint}/{id}", dto);

    public virtual async Task IniciarAsync(int id)
        => await _api.PostAsync<object>($"{Endpoint}/{id}/iniciar", new { });

    public virtual async Task FinalizarAsync(int id)
        => await _api.PostAsync<object>($"{Endpoint}/{id}/finalizar", new { });

    public virtual async Task DeleteAsync(int id)
        => await _api.DeleteAsync($"{Endpoint}/{id}");
}
