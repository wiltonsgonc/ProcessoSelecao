namespace ProcessoSelecao.Blazor.Models;

public class VerificarCandidato
{
    public string? Cpf { get; set; }
    public string? Email { get; set; }
    public DateTime? DataNascimento { get; set; }
}

public class LoginCandidato
{
    public string? Cpf { get; set; }
    public string? Email { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string Senha { get; set; } = string.Empty;
}

public class DefinirSenhaCandidato
{
    public long CandidatoId { get; set; }
    public string Senha { get; set; } = string.Empty;
}

public class CandidatoAuthResponse
{
    public string? Token { get; set; }
    public long CandidatoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? Expiracao { get; set; }
    public bool PrimeiroAcesso { get; set; }
}
