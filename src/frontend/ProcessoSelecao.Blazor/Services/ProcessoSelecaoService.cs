namespace ProcessoSelecao.Blazor.Services;

public class ProcessoSelecaoService
{
    private readonly ApiService _api;
    private const string Endpoint = "processosselecao";

    public ProcessoSelecaoService(ApiService api)
    {
        _api = api;
    }

    public async Task<List<Models.ProcessoSelecao>> GetAllAsync()
        => await _api.GetAsync<List<Models.ProcessoSelecao>>(Endpoint) ?? new();

    public async Task<Models.ProcessoSelecao?> GetByIdAsync(int id)
        => await _api.GetAsync<Models.ProcessoSelecao>($"{Endpoint}/{id}");

    public async Task<Models.ProcessoSelecao?> CreateAsync(Models.CreateProcessoSelecao dto)
        => await _api.PostAsync<Models.ProcessoSelecao>(Endpoint, dto);

    public async Task<Models.ProcessoSelecao?> UpdateAsync(int id, Models.UpdateProcessoSelecao dto)
        => await _api.PutAsync<Models.ProcessoSelecao>($"{Endpoint}/{id}", dto);

    public async Task IniciarAsync(int id)
        => await _api.PostAsync<object>($"{Endpoint}/{id}/iniciar", new { });

    public async Task FinalizarAsync(int id)
        => await _api.PostAsync<object>($"{Endpoint}/{id}/finalizar", new { });

    public async Task DeleteAsync(int id)
        => await _api.DeleteAsync($"{Endpoint}/{id}");
}
