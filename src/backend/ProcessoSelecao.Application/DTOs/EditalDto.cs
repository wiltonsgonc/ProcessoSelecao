using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Application.DTOs;

/// <summary>
/// DTO para leitura de dados do Edital
/// </summary>
public class EditalDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataPublicacao { get; set; }
    public DateTime DataInicioInscricao { get; set; }
    public DateTime DataFimInscricao { get; set; }
    public decimal? ValorInscricao { get; set; }
    public string? TextoEdital { get; set; }
    public StatusEdital Status { get; set; }
    public List<OpcaoCursoDto> OpcoesCurso { get; set; } = new();
    public bool EstaAberto { get; set; }
    
    public List<string> LocaisProva { get; set; } = new();
    public List<string> Campi { get; set; } = new();
    public List<string> FormasInscricao { get; set; } = new();
    
    public bool ExigeRgCpf { get; set; } = true;
    public bool ExigeAnexoI { get; set; } = true;
    public bool ExigeCurriculoLattes { get; set; } = true;
    public bool ExigeCurriculoLattesOrientador { get; set; } = false;
    public bool ExigeAnexoII { get; set; } = false;
    public bool ExigeComprovanteMatricula { get; set; } = false;
    public bool ExigeHistoricoGraduacao { get; set; } = false;
}

/// <summary>
/// DTO para criação de novo Edital
/// </summary>
public class EditalCreateDto
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataPublicacao { get; set; }
    public DateTime DataInicioInscricao { get; set; }
    public DateTime DataFimInscricao { get; set; }
    public decimal? ValorInscricao { get; set; }
    public string? TextoEdital { get; set; }
    public List<OpcaoCursoCreateDto> OpcoesCurso { get; set; } = new();
    
    public List<string> LocaisProva { get; set; } = new();
    public List<string> Campi { get; set; } = new();
    public List<string> FormasInscricao { get; set; } = new();
    
    public bool ExigeRgCpf { get; set; } = true;
    public bool ExigeAnexoI { get; set; } = true;
    public bool ExigeCurriculoLattes { get; set; } = true;
    public bool ExigeCurriculoLattesOrientador { get; set; } = false;
    public bool ExigeAnexoII { get; set; } = false;
    public bool ExigeComprovanteMatricula { get; set; } = false;
    public bool ExigeHistoricoGraduacao { get; set; } = false;
}

/// <summary>
/// DTO para atualização de Edital existente
/// </summary>
public class EditalUpdateDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataPublicacao { get; set; }
    public DateTime DataInicioInscricao { get; set; }
    public DateTime DataFimInscricao { get; set; }
    public decimal? ValorInscricao { get; set; }
    public string? TextoEdital { get; set; }
    public StatusEdital Status { get; set; }
    
    public List<string> LocaisProva { get; set; } = new();
    public List<string> Campi { get; set; } = new();
    public List<string> FormasInscricao { get; set; } = new();
    
    public bool ExigeRgCpf { get; set; } = true;
    public bool ExigeAnexoI { get; set; } = true;
    public bool ExigeCurriculoLattes { get; set; } = true;
    public bool ExigeCurriculoLattesOrientador { get; set; } = false;
    public bool ExigeAnexoII { get; set; } = false;
    public bool ExigeComprovanteMatricula { get; set; } = false;
    public bool ExigeHistoricoGraduacao { get; set; } = false;
}
