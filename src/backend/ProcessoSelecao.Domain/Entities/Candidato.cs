using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Domain.Entities;

/// <summary>
/// Entidade que representa um candidato no processo seletivo
/// </summary>
public class Candidato : BaseEntity
{
    /// <summary>Nome completo do candidato</summary>
    public string Nome { get; set; } = string.Empty;
    
    /// <summary>Matrícula do candidato na instituição</summary>
    public string Cpf { get; set; } = string.Empty;
    
    /// <summary>RG do candidato</summary>
    public string? RG { get; set; }
    
    /// <summary>Telefone do candidato</summary>
    public string? Telefone { get; set; }
    
    /// <summary>E-mail do candidato</summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>Área de pesquisa escolhida</summary>
    public string? AreaPesquisa { get; set; }
    
    /// <summary>Status de validação dos documentos</summary>
    public StatusValidacao StatusValidacao { get; set; } = StatusValidacao.Pendente;
    
    /// <summary>Data de cadastro no sistema</summary>
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    
    /// <summary>ID do processo de seleção</summary>
    public long ProcessoSelecaoId { get; set; }
    
    /// <summary>Processo de seleção relacionado</summary>
    public virtual ProcessoSelecao? ProcessoSelecao { get; set; }
    
    /// <summary>Documentos enviados pelo candidato</summary>
    public virtual ICollection<Documento> Documentos { get; set; } = new List<Documento>();
    
    /// <summary>Avaliações/baremas do candidato</summary>
    public virtual ICollection<Barema> Baremas { get; set; } = new List<Barema>();

    /// <summary>
    /// Valida se todos os documentos foram validados
    /// </summary>
    public bool ValidarDocumentos()
    {
        if (!Documentos.Any()) return false;
        return Documentos.All(d => d.Validado);
    }

    /// <summary>
    /// Calcula a pontuação média das avaliações
    /// </summary>
    public float CalcularPontuacao()
    {
        if (!Baremas.Any()) return 0;
        return Baremas.Where(b => b.Status == StatusBarema.Concluido).Average(b => b.NotaFinal);
    }
}
