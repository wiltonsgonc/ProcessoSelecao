using Microsoft.AspNetCore.Components.Forms;

namespace ProcessoSelecao.Blazor.Services;

public class FormularioService
{
    private readonly HttpClient _http;
    private readonly string _apiUrl;

    public DadosPagina1? DadosPagina1 { get; private set; }
    public DadosPagina2? DadosPagina2 { get; private set; }
    public DadosPagina3? DadosPagina3 { get; private set; }
    public DadosPagina4? DadosPagina4 { get; private set; }
    public int PaginaAtual { get; set; } = 1;

    public event Action? OnChange;

    public FormularioService(HttpClient http, IConfiguration configuration)
    {
        _http = http;
        _apiUrl = configuration["Backend:BaseUrl"] ?? "http://localhost:5002/api";
    }

    public void SalvarPagina1(DadosPagina1 dados)
    {
        DadosPagina1 = Merge(DadosPagina1, dados);
        NotifyStateChanged();
    }

    public void SalvarPagina2(DadosPagina2 dados)
    {
        DadosPagina2 = Merge(DadosPagina2, dados);
        NotifyStateChanged();
    }

    public void SalvarPagina3(DadosPagina3 dados)
    {
        DadosPagina3 = Merge(DadosPagina3, dados);
        NotifyStateChanged();
    }

    public void SalvarPagina4(DadosPagina4 dados)
    {
        DadosPagina4 = Merge(DadosPagina4, dados);
        NotifyStateChanged();
    }

    public void LimparDados()
    {
        DadosPagina1 = null;
        DadosPagina2 = null;
        DadosPagina3 = null;
        DadosPagina4 = null;
        PaginaAtual = 1;
        NotifyStateChanged();
    }

    public async Task<object?> EnviarInscricaoCompletaAsync(int processoSelecaoId)
    {
        using var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(processoSelecaoId.ToString()), "processoSelecaoId");

        var dados = new
        {
            pagina1 = DadosPagina1,
            pagina2 = DadosPagina2,
            pagina4 = DadosPagina4
        };
        formData.Add(new StringContent(System.Text.Json.JsonSerializer.Serialize(dados)), "dados");

        if (DadosPagina3?.RgCpfCandidato != null)
            formData.Add(new StreamContent(DadosPagina3.RgCpfCandidato.OpenReadStream()), "rgCpfCandidato", DadosPagina3.RgCpfCandidato.Name);
        if (DadosPagina3?.AnexoI != null)
            formData.Add(new StreamContent(DadosPagina3.AnexoI.OpenReadStream()), "anexoI", DadosPagina3.AnexoI.Name);
        if (!string.IsNullOrEmpty(DadosPagina3?.CurriculoLattesCandidato))
            formData.Add(new StringContent(DadosPagina3.CurriculoLattesCandidato), "curriculoLattesCandidato");
        if (!string.IsNullOrEmpty(DadosPagina3?.CurriculoLattesOrientador))
            formData.Add(new StringContent(DadosPagina3.CurriculoLattesOrientador), "curriculoLattesOrientador");
        if (DadosPagina3?.AnexoII != null)
            formData.Add(new StreamContent(DadosPagina3.AnexoII.OpenReadStream()), "anexoII", DadosPagina3.AnexoII.Name);
        if (DadosPagina3?.ComprovanteMatricula != null)
            formData.Add(new StreamContent(DadosPagina3.ComprovanteMatricula.OpenReadStream()), "comprovanteMatricula", DadosPagina3.ComprovanteMatricula.Name);
        if (DadosPagina3?.HistoricoEscolar != null)
            formData.Add(new StreamContent(DadosPagina3.HistoricoEscolar.OpenReadStream()), "historicoEscolar", DadosPagina3.HistoricoEscolar.Name);

        var response = await _http.PostAsync($"{_apiUrl}/formulario/completa", formData);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<object>();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    private static T? Merge<T>(T? existing, T incoming) where T : class
    {
        if (existing == null) return incoming;
        var props = typeof(T).GetProperties();
        foreach (var prop in props)
        {
            var value = prop.GetValue(incoming);
            if (value != null && !(value is string s && string.IsNullOrEmpty(s)))
                prop.SetValue(existing, value);
        }
        return existing;
    }
}

public class DadosPagina1
{
    public string? Nome { get; set; }
    public string? DataNascimento { get; set; }
    public string? TipoDocumento { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? AreaOfertada { get; set; }
    public bool PoliticaPrivacidade { get; set; }
}

public class DadosPagina2
{
    public string? Nome { get; set; }
    public string? DataNascimento { get; set; }
    public string? PaisNatal { get; set; }
    public string? EstadoNatal { get; set; }
    public string? Naturalidade { get; set; }
    public string? NomeSocial { get; set; }
    public string? EstadoCivil { get; set; }
    public string? Nacionalidade { get; set; }
    public string? Email { get; set; }
    public string? Sexo { get; set; }
    public string? Cpf { get; set; }
    public string? Telefone1 { get; set; }
    public string? Telefone2 { get; set; }
    public string? CorRaca { get; set; }
    public string? AutorizacaoDados { get; set; }
    public string? TipoVisto { get; set; }
    public string? NumeroRegistroGeral { get; set; }
    public string? DataVencimentoRG { get; set; }
}

public class DadosPagina3
{
    public IBrowserFile? RgCpfCandidato { get; set; }
    public IBrowserFile? AnexoI { get; set; }
    public string? CurriculoLattesCandidato { get; set; }
    public string? CurriculoLattesOrientador { get; set; }
    public IBrowserFile? AnexoII { get; set; }
    public IBrowserFile? ComprovanteMatricula { get; set; }
    public IBrowserFile? HistoricoEscolar { get; set; }
}

public class DadosPagina4
{
    public string? ProcessoSeletivo { get; set; }
    public string? AreaOfertada { get; set; }
    public string? FormaInscricao { get; set; }
    public string? LocalProva { get; set; }
    public string? CampusProva { get; set; }
    public string? DataInscricao { get; set; }
    public double? ValorInscricao { get; set; }
    public bool DeficienciaFisica { get; set; }
    public bool DeficienciaAuditiva { get; set; }
    public bool DeficienciaFala { get; set; }
    public bool DeficienciaVisual { get; set; }
    public bool DeficienciaMental { get; set; }
    public bool DeficienciaIntelectual { get; set; }
    public bool DeficienciaReabilitado { get; set; }
    public bool DeficienciaMultipla { get; set; }
    public string? MotivoOutrasNecessidades { get; set; }
}
