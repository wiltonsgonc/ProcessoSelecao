namespace ProcessoSelecao.Application.DTOs
{
    public class Pagina1Dto
    {
        public string Nome { get; set; } = string.Empty;
        public string? DataNascimento { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string AreaOfertada { get; set; } = string.Empty;
        public bool PoliticaPrivacidade { get; set; }
    }
}
