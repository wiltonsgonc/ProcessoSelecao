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

    public async Task<InscricaoResultResponse> EnviarInscricaoCompletaAsync(int processoSelecaoId)
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

        if (DadosPagina3 == null)
            throw new Exception("Dados da página 3 (documentos) não preenchidos. Preencha os documentos antes de enviar.");

        var hasAnyFile = DadosPagina3.RgCpfCandidatoArquivo != null
            || DadosPagina3.AnexoIArquivo != null
            || DadosPagina3.AnexoIIArquivo != null
            || DadosPagina3.ComprovanteMatriculaArquivo != null
            || DadosPagina3.HistoricoEscolarArquivo != null;
        var hasAnyLink = !string.IsNullOrEmpty(DadosPagina3.CurriculoLattesCandidato)
            || !string.IsNullOrEmpty(DadosPagina3.CurriculoLattesOrientador);

        if (!hasAnyFile && !hasAnyLink)
            throw new Exception("Nenhum arquivo ou link foi preenchido na página 3. Anexe os documentos obrigatórios antes de enviar.");

        if (DadosPagina3.RgCpfCandidatoArquivo != null)
            formData.Add(new ByteArrayContent(DadosPagina3.RgCpfCandidatoArquivo), "rgCpfCandidato", DadosPagina3.RgCpfCandidatoNome ?? "arquivo.pdf");
        if (DadosPagina3.AnexoIArquivo != null)
            formData.Add(new ByteArrayContent(DadosPagina3.AnexoIArquivo), "anexoI", DadosPagina3.AnexoINome ?? "arquivo.pdf");
        if (!string.IsNullOrEmpty(DadosPagina3.CurriculoLattesCandidato))
            formData.Add(new StringContent(DadosPagina3.CurriculoLattesCandidato), "curriculoLattesCandidato");
        if (!string.IsNullOrEmpty(DadosPagina3.CurriculoLattesOrientador))
            formData.Add(new StringContent(DadosPagina3.CurriculoLattesOrientador), "curriculoLattesOrientador");
        if (DadosPagina3.AnexoIIArquivo != null)
            formData.Add(new ByteArrayContent(DadosPagina3.AnexoIIArquivo), "anexoII", DadosPagina3.AnexoIINome ?? "arquivo.pdf");
        if (DadosPagina3.ComprovanteMatriculaArquivo != null)
            formData.Add(new ByteArrayContent(DadosPagina3.ComprovanteMatriculaArquivo), "comprovanteMatricula", DadosPagina3.ComprovanteMatriculaNome ?? "arquivo.pdf");
        if (DadosPagina3.HistoricoEscolarArquivo != null)
            formData.Add(new ByteArrayContent(DadosPagina3.HistoricoEscolarArquivo), "historicoEscolar", DadosPagina3.HistoricoEscolarNome ?? "arquivo.pdf");

        var response = await _http.PostAsync($"{_apiUrl}/formulario/completa", formData);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            string mensagemErro = "Erro ao enviar inscrição.";
            try
            {
                var erroObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
                if (erroObj != null && erroObj.TryGetValue("erro", out var erro))
                    mensagemErro = erro;
                else if (erroObj != null && erroObj.TryGetValue("message", out var msg))
                    mensagemErro = msg;
            }
            catch { mensagemErro += $" (Status {(int)response.StatusCode})"; }
            throw new Exception(mensagemErro);
        }
        return System.Text.Json.JsonSerializer.Deserialize<InscricaoResultResponse>(responseBody,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new InscricaoResultResponse();
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

public class InscricaoResultResponse
{
    public long CandidatoId { get; set; }
    public long ProcessoSelecaoId { get; set; }
    public string NumeroInscricao { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public DateTime DataInscricao { get; set; }
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
    public byte[]? RgCpfCandidatoArquivo { get; set; }
    public string? RgCpfCandidatoNome { get; set; }
    public byte[]? AnexoIArquivo { get; set; }
    public string? AnexoINome { get; set; }
    public string? CurriculoLattesCandidato { get; set; }
    public string? CurriculoLattesOrientador { get; set; }
    public byte[]? AnexoIIArquivo { get; set; }
    public string? AnexoIINome { get; set; }
    public byte[]? ComprovanteMatriculaArquivo { get; set; }
    public string? ComprovanteMatriculaNome { get; set; }
    public byte[]? HistoricoEscolarArquivo { get; set; }
    public string? HistoricoEscolarNome { get; set; }
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
