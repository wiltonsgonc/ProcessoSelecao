using ProcessoSelecao.Blazor.Models;

namespace ProcessoSelecao.Blazor.Services;

public class CandidatoService
{
    private readonly ApiService _api;
    private const string Endpoint = "candidatos";

    public CandidatoService(ApiService api)
    {
        _api = api;
    }

    public async Task<List<Candidato>> GetAllAsync()
        => await _api.GetAsync<List<Candidato>>(Endpoint) ?? new();

    public async Task<Candidato?> GetByIdAsync(int id)
        => await _api.GetAsync<Candidato>($"{Endpoint}/{id}");

    public async Task<List<Candidato>> GetByProcessoIdAsync(int processoId)
        => await _api.GetAsync<List<Candidato>>($"{Endpoint}/processo/{processoId}") ?? new();

    public async Task<double> GetPontuacaoAsync(int id)
        => await _api.GetAsync<double>($"{Endpoint}/{id}/pontuacao");

    public async Task<Candidato?> CreateAsync(CreateCandidato dto)
        => await _api.PostAsync<Candidato>(Endpoint, dto);

    public async Task<Candidato?> UpdateAsync(int id, UpdateCandidato dto)
        => await _api.PutAsync<Candidato>($"{Endpoint}/{id}", dto);

    public async Task DeleteAsync(int id)
        => await _api.DeleteAsync($"{Endpoint}/{id}");
}
