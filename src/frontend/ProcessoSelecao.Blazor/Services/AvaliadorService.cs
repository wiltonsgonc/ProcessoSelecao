using ProcessoSelecao.Blazor.Models;

namespace ProcessoSelecao.Blazor.Services;

public class AvaliadorService
{
    private readonly ApiService _api;
    private const string Endpoint = "avaliadores";

    public AvaliadorService(ApiService api)
    {
        _api = api;
    }

    public async Task<List<Avaliador>> GetAllAsync()
        => await _api.GetAsync<List<Avaliador>>(Endpoint) ?? new();

    public async Task<Avaliador?> GetByIdAsync(int id)
        => await _api.GetAsync<Avaliador>($"{Endpoint}/{id}");

    public async Task<List<Avaliador>> GetByProcessoIdAsync(int processoId)
        => await _api.GetAsync<List<Avaliador>>($"{Endpoint}/processo/{processoId}") ?? new();

    public async Task<Avaliador?> CreateAsync(CreateAvaliador dto)
        => await _api.PostAsync<Avaliador>(Endpoint, dto);

    public async Task<Avaliador?> UpdateAsync(int id, UpdateAvaliador dto)
        => await _api.PutAsync<Avaliador>($"{Endpoint}/{id}", dto);

    public async Task DeleteAsync(int id)
        => await _api.DeleteAsync($"{Endpoint}/{id}");
}
