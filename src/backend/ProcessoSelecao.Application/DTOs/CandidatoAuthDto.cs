namespace ProcessoSelecao.Application.DTOs;

public class VerificarCandidatoDto
{
    public string? Cpf { get; set; }
    public string? Email { get; set; }
    public DateTime? DataNascimento { get; set; }
}

public class LoginCandidatoDto
{
    public string? Cpf { get; set; }
    public string? Email { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string Senha { get; set; } = string.Empty;
}

public class DefinirSenhaCandidatoDto
{
    public long CandidatoId { get; set; }
    public string Senha { get; set; } = string.Empty;
}

public class CandidatoAuthResponseDto
{
    public string? Token { get; set; }
    public long CandidatoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? Expiracao { get; set; }
    public bool PrimeiroAcesso { get; set; }
}
