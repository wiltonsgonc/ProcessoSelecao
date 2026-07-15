using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ProcessoSelecao.Blazor.Models;

namespace ProcessoSelecao.Blazor.Services;

public class AvaliadorAuthService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _configuration;
    private const string AuthTokenKey = "avaliador_token";
    private const string AvaliadorIdKey = "avaliador_id";
    private const string AvaliadorNomeKey = "avaliador_nome";

    public AvaliadorAuthService(HttpClient http, IConfiguration configuration)
    {
        _http = http;
        _configuration = configuration;
    }

    public async Task<bool> LoginAsync(string cpf, string senha)
    {
        try
        {
            var baseUrl = _configuration["ApiBaseUrl"] ?? _configuration["ApiSettings__BaseUrl"] ?? "http://localhost:5002";
            var response = await _http.PostAsJsonAsync($"{baseUrl}/api/avaliador-auth/login", new { Cpf = cpf, Senha = senha });

            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadFromJsonAsync<AvaliadorAuthResponse>();
            if (result == null) return false;

            // Armazena no localStorage via JS interop (será chamado pela página)
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void SetToken(string token)
    {
        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public string? GetToken()
    {
        return null; // Será gerenciado via sessionStorage pelo Blazor
    }
}
