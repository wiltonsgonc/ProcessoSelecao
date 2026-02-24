using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

public class Candidato : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Matricula { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AreaPesquisa { get; set; }
    public StatusValidacao StatusValidacao { get; set; } = StatusValidacao.Pendente;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    
    public long ProcessoSelecaoId { get; set; }
    public virtual ProcessoSelecao? ProcessoSelecao { get; set; }
    
    public virtual ICollection<Documento> Documentos { get; set; } = new List<Documento>();
    public virtual ICollection<Barema> Baremas { get; set; } = new List<Barema>();

    public bool ValidarDocumentos()
    {
        if (!Documentos.Any()) return false;
        return Documentos.All(d => d.Validado);
    }

    public float CalcularPontuacao()
    {
        if (!Baremas.Any()) return 0;
        return Baremas.Where(b => b.Status == StatusBarema.Concluido).Average(b => b.NotaFinal);
    }
}
