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

    public virtual async Task<List<Barema>> GetAllAsync()
        => await _api.GetAsync<List<Barema>>(Endpoint) ?? new();

    public virtual async Task<Barema?> GetByIdAsync(int id)
        => await _api.GetAsync<Barema>($"{Endpoint}/{id}");

    public virtual async Task<List<Barema>> GetByCandidatoIdAsync(int candidatoId)
        => await _api.GetAsync<List<Barema>>($"{Endpoint}/candidato/{candidatoId}") ?? new();

    public virtual async Task<List<Barema>> GetByAvaliadorIdAsync(int avaliadorId)
        => await _api.GetAsync<List<Barema>>($"{Endpoint}/avaliador/{avaliadorId}") ?? new();

    public virtual async Task<Barema?> CreateAsync(CreateBarema dto)
        => await _api.PostAsync<Barema>(Endpoint, dto);

    public virtual async Task<Barema?> UpdateCriteriosAsync(int id, UpdateBarema dto)
        => await _api.PutAsync<Barema>($"{Endpoint}/{id}/criterios", dto);

    public virtual async Task<Barema?> FinalizarAsync(int id, FinalizarBarema dto)
        => await _api.PostAsync<Barema>($"{Endpoint}/{id}/finalizar", dto);

    public virtual async Task DeleteAsync(int id)
        => await _api.DeleteAsync($"{Endpoint}/{id}");

    public virtual async Task<List<ProgressoCandidato>> GetProgressoAsync(int processoId)
        => await _api.GetAsync<List<ProgressoCandidato>>($"{Endpoint}/progresso/{processoId}") ?? new();
}
