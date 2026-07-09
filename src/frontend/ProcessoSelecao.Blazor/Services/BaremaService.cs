using ProcessoSelecao.Blazor.Models;

namespace ProcessoSelecao.Blazor.Services;

public class BaremaService
{
    private readonly ApiService _api;
    private const string Endpoint = "baremas";

    public BaremaService(ApiService api)
    {
        _api = api;
    }

    public async Task<List<Barema>> GetAllAsync()
        => await _api.GetAsync<List<Barema>>(Endpoint) ?? new();

    public async Task<Barema?> GetByIdAsync(int id)
        => await _api.GetAsync<Barema>($"{Endpoint}/{id}");

    public async Task<List<Barema>> GetByCandidatoIdAsync(int candidatoId)
        => await _api.GetAsync<List<Barema>>($"{Endpoint}/candidato/{candidatoId}") ?? new();

    public async Task<List<Barema>> GetByAvaliadorIdAsync(int avaliadorId)
        => await _api.GetAsync<List<Barema>>($"{Endpoint}/avaliador/{avaliadorId}") ?? new();

    public async Task<Barema?> CreateAsync(CreateBarema dto)
        => await _api.PostAsync<Barema>(Endpoint, dto);

    public async Task<Barema?> UpdateCriteriosAsync(int id, UpdateBarema dto)
        => await _api.PutAsync<Barema>($"{Endpoint}/{id}/criterios", dto);

    public async Task<Barema?> FinalizarAsync(int id, FinalizarBarema dto)
        => await _api.PostAsync<Barema>($"{Endpoint}/{id}/finalizar", dto);

    public async Task DeleteAsync(int id)
        => await _api.DeleteAsync($"{Endpoint}/{id}");
}
