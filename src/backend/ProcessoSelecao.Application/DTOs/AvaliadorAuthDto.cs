namespace ProcessoSelecao.Application.DTOs;

public class LoginAvaliadorDto
{
    public string Cpf { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class DefinirSenhaDto
{
    public long AvaliadorId { get; set; }
    public string Senha { get; set; } = string.Empty;
}

public class AvaliadorAuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public long AvaliadorId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime Expiracao { get; set; }
}
