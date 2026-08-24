using Microsoft.AspNetCore.Components.Forms;
using ProcessoSelecao.Blazor.Models;

namespace ProcessoSelecao.Blazor.Services;

public class DocumentoService
{
    private readonly ApiService _api;
    private const string Endpoint = "documentos";

    public DocumentoService(ApiService api)
    {
        _api = api;
    }

    public virtual async Task<List<Documento>> GetAllAsync()
        => await _api.GetAsync<List<Documento>>(Endpoint) ?? new();

    public virtual async Task<Documento?> GetByIdAsync(int id)
        => await _api.GetAsync<Documento>($"{Endpoint}/{id}");

    public virtual async Task<List<Documento>> GetByCandidatoIdAsync(long candidatoId)
        => await _api.GetAsync<List<Documento>>($"{Endpoint}/candidato/{candidatoId}") ?? new();

    public virtual async Task<Documento?> UploadAsync(IBrowserFile file, CreateDocumento data)
    {
        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream();
        content.Add(new StreamContent(stream), "arquivo", file.Name);
        content.Add(new StringContent(data.Tipo.ToString()), "tipo");
        content.Add(new StringContent(data.NomeArquivo), "nomeArquivo");
        content.Add(new StringContent(data.CandidatoId.ToString()), "candidatoId");
        return await _api.UploadFileAsync<Documento>(Endpoint, content);
    }

    public virtual async Task<Documento?> CreateWithUrlAsync(CreateDocumentoWithUrl dto)
        => await _api.PostAsync<Documento>($"{Endpoint}/with-url", dto);

    public virtual async Task<Documento?> ValidateAsync(int id, ValidateDocumento dto)
        => await _api.PutAsync<Documento>($"{Endpoint}/{id}/validar", dto);

    public virtual async Task DeleteAsync(int id)
        => await _api.DeleteAsync($"{Endpoint}/{id}");

    public virtual string GetDownloadUrl(int id)
        => $"{_api.GetBaseUrl()}/{Endpoint}/download/{id}";

    public virtual string GetViewUrl(int id)
        => $"{_api.GetBaseUrl()}/{Endpoint}/{id}/view";

    public virtual async Task<byte[]> ViewDocumentAsync(int id)
        => await _api.GetBytesAsync($"{Endpoint}/{id}/view");

    public virtual async Task<byte[]> DownloadMultipleAsync(List<int> ids)
        => await _api.PostBytesAsync($"{Endpoint}/download-multiple", ids);
}
