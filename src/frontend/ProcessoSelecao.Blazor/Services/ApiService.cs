namespace ProcessoSelecao.Blazor.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient http, ILogger<ApiService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var url = new Uri(_http.BaseAddress!, endpoint);
        _logger.LogInformation("GET {Url}", url);
        var response = await _http.GetAsync(endpoint);
        _logger.LogInformation("GET {Url} -> {Status}", url, response.StatusCode);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("GET {Url} failed: {Body}", url, body);
            return default;
        }
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        var response = await _http.PostAsJsonAsync(endpoint, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        var response = await _http.PutAsJsonAsync(endpoint, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task DeleteAsync(string endpoint)
    {
        var response = await _http.DeleteAsync(endpoint);
        response.EnsureSuccessStatusCode();
    }

    public async Task<T?> UploadFileAsync<T>(string endpoint, MultipartFormDataContent formData)
    {
        var response = await _http.PostAsync(endpoint, formData);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<byte[]> GetBytesAsync(string endpoint)
    {
        return await _http.GetByteArrayAsync(endpoint);
    }

    public async Task<byte[]> PostBytesAsync(string endpoint, object data)
    {
        var response = await _http.PostAsJsonAsync(endpoint, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public string GetBaseUrl()
    {
        return _http.BaseAddress?.ToString().TrimEnd('/') ?? string.Empty;
    }
}
