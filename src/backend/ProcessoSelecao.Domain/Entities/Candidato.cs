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
    
    /// <summary>Número de inscrição gerado automaticamente</summary>
    public string NumeroInscricao { get; set; } = string.Empty;
    
    /// <summary>Data de nascimento do candidato</summary>
    public DateTime? DataNascimento { get; set; }
    
    /// <summary>País de nascimento</summary>
    public string? PaisNatal { get; set; }
    
    /// <summary>Estado de nascimento</summary>
    public string? EstadoNatal { get; set; }
    
    /// <summary>Naturalidade/Cidade de nascimento</summary>
    public string? Naturalidade { get; set; }
    
    /// <summary>Nome social (se houver)</summary>
    public string? NomeSocial { get; set; }
    
    /// <summary>Estado civil</summary>
    public string? EstadoCivil { get; set; }
    
    /// <summary>Nacionalidade</summary>
    public string? Nacionalidade { get; set; }
    
    /// <summary>Sexo</summary>
    public string? Sexo { get; set; }
    
    /// <summary>Segundo telefone</summary>
    public string? Telefone2 { get; set; }
    
    /// <summary>Cor/Raça</summary>
    public string? CorRaca { get; set; }
    
    /// <summary>Data de vencimento do RG</summary>
    public DateTime? DataVencimentoRG { get; set; }
    
    /// <summary>Tipo de visto</summary>
    public string? TipoVisto { get; set; }
    
    /// <summary>Forma de inscrição</summary>
    public string? FormaInscricao { get; set; }
    
    /// <summary>Local da prova</summary>
    public string? LocalProva { get; set; }
    
    /// <summary>Campus da prova</summary>
    public string? CampusProva { get; set; }
    
    /// <summary>Valor da inscrição</summary>
    public decimal? ValorInscricao { get; set; }
    
    /// <summary>Deficiência física</summary>
    public bool DeficienciaFisica { get; set; }
    
    /// <summary>Deficiência auditiva</summary>
    public bool DeficienciaAuditiva { get; set; }
    
    /// <summary>Deficiência de fala</summary>
    public bool DeficienciaFala { get; set; }
    
    /// <summary>Deficiência visual</summary>
    public bool DeficienciaVisual { get; set; }
    
    /// <summary>Deficiência mental</summary>
    public bool DeficienciaMental { get; set; }
    
    /// <summary>Deficiência intelectual</summary>
    public bool DeficienciaIntelectual { get; set; }
    
    /// <summary>Pessoa reabilitada</summary>
    public bool DeficienciaReabilitado { get; set; }
    
    /// <summary>Deficiência múltipla</summary>
    public bool DeficienciaMultipla { get; set; }
    
    /// <summary>Motivo/descrição outras necessidades</summary>
    public string? MotivoOutrasNecessidades { get; set; }
    
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
