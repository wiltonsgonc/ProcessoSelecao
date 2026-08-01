using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProcessoSelecao.Domain.Entities;
using ProcessoSelecao.Domain.Enums;

namespace ProcessoSelecao.Infrastructure.Data;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public DataSeeder(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        var seedEnabled = _configuration.GetSection("SeedData")?["Enabled"];
        Console.WriteLine($"[SeedData] Config SeedData:Enabled = '{seedEnabled ?? "(null)"}'");
        if (!bool.TryParse(seedEnabled, out var enabled) || !enabled)
        {
            Console.WriteLine("[SeedData] Desabilitado via configuracao (SeedData:Enabled=false).");
            return;
        }

        if (await _context.ProcessosSelecao.AnyAsync())
        {
            Console.WriteLine("[SeedData] Banco ja possui dados. Seed ignorado (idempotente).");
            return;
        }

        Console.WriteLine("[SeedData] Iniciando preenchimento automatico...");

        var senhaPadrao = BCrypt.Net.BCrypt.HashPassword("123456");

        // ============================================================
        // ProcessosSelecao
        // ============================================================
        var processo1 = new ProcessoSelecao.Domain.Entities.ProcessoSelecao
        {
            Nome = "Edital PIBIC 2026/2027",
            Descricao = "Programa Institucional de Bolsas de Iniciacao Cientifica - Ciclo 2026/2027. Selecao de alunos para bolsas de pesquisa.",
            DataInicio = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            DataFim = new DateTime(2026, 8, 31, 23, 59, 59, DateTimeKind.Utc),
            VagasDisponiveis = 20,
            AgenciaFomento = "CNPq",
            NivelBolsa = "IC",
            Status = StatusProcesso.Aberto
        };

        var processo2 = new ProcessoSelecao.Domain.Entities.ProcessoSelecao
        {
            Nome = "Edital PIBITI 2026/2027",
            Descricao = "Programa Institucional de Bolsas de Iniciacao em Desenvolvimento Tecnologico e Inovacao.",
            DataInicio = new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc),
            DataFim = new DateTime(2026, 9, 30, 23, 59, 59, DateTimeKind.Utc),
            VagasDisponiveis = 15,
            AgenciaFomento = "CNPq",
            NivelBolsa = "ITI",
            Status = StatusProcesso.Aberto
        };

        var processo3 = new ProcessoSelecao.Domain.Entities.ProcessoSelecao
        {
            Nome = "Edital Mestrado 2025",
            Descricao = "Processo seletivo para ingresso no Programa de Pos-Graduacao - Turma 2025.",
            DataInicio = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            DataFim = new DateTime(2025, 6, 30, 23, 59, 59, DateTimeKind.Utc),
            VagasDisponiveis = 10,
            AgenciaFomento = "CAPES",
            NivelBolsa = "Mestrado",
            Status = StatusProcesso.Finalizado
        };

        _context.ProcessosSelecao.AddRange(processo1, processo2, processo3);
        await _context.SaveChangesAsync();

        // ============================================================
        // Avaliadores (senha padrao: 123456)
        // ============================================================
        var avaliador1 = new Avaliador
        {
            Nome = "Joao Silva",
            Cpf = "98765432100",
            Email = "joao.silva@universidade.edu.br",
            Tipo = TipoAvaliador.Interno,
            AreaEspecializacao = "Ciencia da Computacao",
            Instituicao = "Universidade Federal",
            Ativo = true,
            SenhaHash = senhaPadrao,
            ProcessoSelecaoId = processo1.Id,
            LinkLattes = "https://lattes.cnpq.br/9876543210",
            UltimaFormacao = "Doutorado em Ciencia da Computacao",
            Cargo = "Professor Associado",
            NivelCnpq = NivelCnpq.Pq1D
        };

        var avaliador2 = new Avaliador
        {
            Nome = "Maria Santos",
            Cpf = "12345678900",
            Email = "maria.santos@universidade.edu.br",
            Tipo = TipoAvaliador.Interno,
            AreaEspecializacao = "Engenharia de Software",
            Instituicao = "Universidade Federal",
            Ativo = true,
            SenhaHash = senhaPadrao,
            ProcessoSelecaoId = processo1.Id,
            LinkLattes = "https://lattes.cnpq.br/1234567890",
            UltimaFormacao = "Doutorado em Engenharia de Software",
            Cargo = "Professora Associada",
            NivelCnpq = NivelCnpq.Pq1C
        };

        var avaliador3 = new Avaliador
        {
            Nome = "Carlos Oliveira",
            Cpf = "11122233344",
            Email = "carlos.oliveira@ufpe.br",
            Tipo = TipoAvaliador.Externo,
            AreaEspecializacao = "Sistemas de Informacao",
            Instituicao = "UFPE",
            Ativo = true,
            SenhaHash = senhaPadrao,
            ProcessoSelecaoId = processo2.Id,
            LinkLattes = "https://lattes.cnpq.br/1112223334",
            UltimaFormacao = "Mestrado em Sistemas de Informacao",
            Cargo = "Pesquisador",
            NivelCnpq = NivelCnpq.Pq2
        };

        var avaliador4 = new Avaliador
        {
            Nome = "Ana Costa",
            Cpf = "55566677788",
            Email = "ana.costa@ufrpe.br",
            Tipo = TipoAvaliador.Externo,
            AreaEspecializacao = "Inteligencia Artificial",
            Instituicao = "UFRPE",
            Ativo = true,
            SenhaHash = senhaPadrao,
            ProcessoSelecaoId = processo2.Id,
            LinkLattes = "https://lattes.cnpq.br/5556667778",
            UltimaFormacao = "Pos-Doutorado em Inteligencia Artificial",
            Cargo = "Pesquisadora Titular",
            NivelCnpq = NivelCnpq.Pq1A
        };

        _context.Avaliadores.AddRange(avaliador1, avaliador2, avaliador3, avaliador4);
        await _context.SaveChangesAsync();

        // ============================================================
        // Candidatos
        // ============================================================
        var candidato1 = new Candidato
        {
            Nome = "Pedro Alves",
            Cpf = "52998224725",
            Email = "pedro.alves@email.com",
            Telefone = "(81) 98888-0001",
            NomeSocial = "Pedro",
            DataNascimento = new DateTime(2002, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            Nacionalidade = "Brasileiro",
            Naturalidade = "Recife",
            EstadoCivil = "Solteiro",
            Sexo = "Masculino",
            CorRaca = "Parda",
            AreaPesquisa = "Inteligencia Artificial",
            TituloProjeto = "Aplicacoes de Machine Learning na Saude",
            LocalProva = "Campus Recife",
            CampusProva = "Recife",
            ValorInscricao = 50.00m,
            ProcessoSelecaoId = processo1.Id,
            NumeroInscricao = "202600100001",
            StatusValidacao = StatusValidacao.Pendente,
            DataCadastro = new DateTime(2026, 6, 15, 10, 30, 0, DateTimeKind.Utc)
        };

        var candidato2 = new Candidato
        {
            Nome = "Lucia Mendes",
            Cpf = "12345678901",
            Email = "lucia.mendes@email.com",
            Telefone = "(81) 98888-0002",
            NomeSocial = "Lucia",
            DataNascimento = new DateTime(2001, 7, 22, 0, 0, 0, DateTimeKind.Utc),
            Nacionalidade = "Brasileira",
            Naturalidade = "Olinda",
            EstadoCivil = "Solteira",
            Sexo = "Feminino",
            CorRaca = "Branca",
            DeficienciaVisual = true,
            AreaPesquisa = "Engenharia de Software",
            TituloProjeto = "Testes Automatizados em Microsservicos",
            LocalProva = "Campus Recife",
            CampusProva = "Recife",
            ValorInscricao = 50.00m,
            ProcessoSelecaoId = processo1.Id,
            NumeroInscricao = "202600100002",
            StatusValidacao = StatusValidacao.Validado,
            DataCadastro = new DateTime(2026, 6, 14, 14, 0, 0, DateTimeKind.Utc)
        };

        var candidato3 = new Candidato
        {
            Nome = "Roberto Lima",
            Cpf = "98765432101",
            Email = "roberto.lima@email.com",
            Telefone = "(81) 98888-0003",
            DataNascimento = new DateTime(2003, 11, 5, 0, 0, 0, DateTimeKind.Utc),
            Nacionalidade = "Brasileiro",
            Naturalidade = "Paulista",
            EstadoCivil = "Solteiro",
            Sexo = "Masculino",
            CorRaca = "Preta",
            AreaPesquisa = "Banco de Dados",
            TituloProjeto = "Otimizacao de Consultas em Big Data",
            LocalProva = "Campus Caruaru",
            CampusProva = "Caruaru",
            ValorInscricao = 50.00m,
            ProcessoSelecaoId = processo1.Id,
            NumeroInscricao = "202600100003",
            StatusValidacao = StatusValidacao.EmAnalise,
            DataCadastro = new DateTime(2026, 6, 20, 9, 15, 0, DateTimeKind.Utc)
        };

        var candidato4 = new Candidato
        {
            Nome = "Fernanda Souza",
            Cpf = "11122233355",
            Email = "fernanda.souza@email.com",
            Telefone = "(81) 98888-0004",
            DataNascimento = new DateTime(2002, 9, 18, 0, 0, 0, DateTimeKind.Utc),
            Nacionalidade = "Brasileira",
            Naturalidade = "Jaboatao",
            EstadoCivil = "Casada",
            Sexo = "Feminino",
            CorRaca = "Branca",
            AreaPesquisa = "Desenvolvimento Web",
            TituloProjeto = "Framework para Acessibilidade em Aplicacoes Web",
            LocalProva = "Campus Recife",
            CampusProva = "Recife",
            ValorInscricao = 50.00m,
            ProcessoSelecaoId = processo2.Id,
            NumeroInscricao = "202600200001",
            StatusValidacao = StatusValidacao.Pendente,
            DataCadastro = new DateTime(2026, 7, 20, 11, 0, 0, DateTimeKind.Utc)
        };

        var candidato5 = new Candidato
        {
            Nome = "Marcos Paulo",
            Cpf = "55566677799",
            Email = "marcos.paulo@email.com",
            Telefone = "(81) 98888-0005",
            DataNascimento = new DateTime(2000, 12, 30, 0, 0, 0, DateTimeKind.Utc),
            Nacionalidade = "Brasileiro",
            Naturalidade = "Recife",
            EstadoCivil = "Solteiro",
            Sexo = "Masculino",
            CorRaca = "Amarela",
            DeficienciaAuditiva = true,
            DeficienciaFala = true,
            AreaPesquisa = "Redes de Computadores",
            TituloProjeto = "Seguranca em Redes IoT",
            LocalProva = "Campus Recife",
            CampusProva = "Recife",
            ValorInscricao = 50.00m,
            ProcessoSelecaoId = processo2.Id,
            NumeroInscricao = "202600200002",
            StatusValidacao = StatusValidacao.Validado,
            DataCadastro = new DateTime(2026, 8, 1, 16, 30, 0, DateTimeKind.Utc)
        };

        var candidato6 = new Candidato
        {
            Nome = "Juliana Torres",
            Cpf = "99988877766",
            Email = "juliana.torres@email.com",
            Telefone = "(81) 98888-0006",
            DataNascimento = new DateTime(1999, 5, 10, 0, 0, 0, DateTimeKind.Utc),
            Nacionalidade = "Brasileira",
            Naturalidade = "Nazare da Mata",
            EstadoCivil = "Solteira",
            Sexo = "Feminino",
            CorRaca = "Indigena",
            AreaPesquisa = "Educacao e Tecnologia",
            TituloProjeto = "Plataforma de Ensino Adaptativo",
            LocalProva = "Campus Recife",
            CampusProva = "Recife",
            ValorInscricao = 50.00m,
            ProcessoSelecaoId = processo3.Id,
            NumeroInscricao = "202500300001",
            StatusValidacao = StatusValidacao.Validado,
            DataCadastro = new DateTime(2025, 1, 15, 8, 0, 0, DateTimeKind.Utc)
        };

        _context.Candidatos.AddRange(candidato1, candidato2, candidato3, candidato4, candidato5, candidato6);
        await _context.SaveChangesAsync();

        await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Candidatos', RESEED, 6)");

        // ============================================================
        // Documentos
        // ============================================================
        var caminhoBase = Path.GetFullPath(_configuration["Storage:CaminhoBase"] ?? "../../../documentos");
        var caminhoSeed = Path.Combine(caminhoBase, "seed");

        var documentos = new List<Documento>
        {
            new() { Tipo = TipoDocumento.HistoricoEscolar, NomeArquivo = "historico_pedro_alves.pdf", CaminhoLocal = Path.Combine(caminhoSeed, "historico_pedro_alves.pdf"), DataUpload = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc), Validado = false, CandidatoId = candidato1.Id },
            new() { Tipo = TipoDocumento.ComprovanteMatricula, NomeArquivo = "comprovante_pedro_alves.pdf", CaminhoLocal = Path.Combine(caminhoSeed, "comprovante_pedro_alves.pdf"), DataUpload = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc), Validado = false, CandidatoId = candidato1.Id },
            new() { Tipo = TipoDocumento.CurriculumLatte, NomeArquivo = "Curriculo Lattes", CaminhoLocal = "", LinkUrl = "http://lattes.cnpq.br/1234567890123456", DataUpload = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc), Validado = true, CandidatoId = candidato1.Id },

            new() { Tipo = TipoDocumento.HistoricoEscolar, NomeArquivo = "historico_lucia_mendes.pdf", CaminhoLocal = Path.Combine(caminhoSeed, "historico_lucia_mendes.pdf"), DataUpload = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc), Validado = true, CandidatoId = candidato2.Id },
            new() { Tipo = TipoDocumento.CartaIntencao, NomeArquivo = "carta_lucia_mendes.pdf", CaminhoLocal = Path.Combine(caminhoSeed, "carta_lucia_mendes.pdf"), DataUpload = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc), Validado = true, CandidatoId = candidato2.Id },
            new() { Tipo = TipoDocumento.CurriculumLatte, NomeArquivo = "Curriculo Lattes", CaminhoLocal = "", LinkUrl = "http://lattes.cnpq.br/2345678901234567", DataUpload = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc), Validado = true, CandidatoId = candidato2.Id },

            new() { Tipo = TipoDocumento.HistoricoEscolar, NomeArquivo = "historico_roberto_lima.pdf", CaminhoLocal = Path.Combine(caminhoSeed, "historico_roberto_lima.pdf"), DataUpload = new DateTime(2026, 6, 20, 0, 0, 0, DateTimeKind.Utc), Validado = false, CandidatoId = candidato3.Id },
            new() { Tipo = TipoDocumento.CartaIntencao, NomeArquivo = "carta_roberto_lima.pdf", CaminhoLocal = Path.Combine(caminhoSeed, "carta_roberto_lima.pdf"), DataUpload = new DateTime(2026, 6, 20, 0, 0, 0, DateTimeKind.Utc), Validado = false, MotivoRejeicao = "Documento ilegivel, favor reenviar.", CandidatoId = candidato3.Id },

            new() { Tipo = TipoDocumento.HistoricoEscolar, NomeArquivo = "historico_fernanda_souza.pdf", CaminhoLocal = Path.Combine(caminhoSeed, "historico_fernanda_souza.pdf"), DataUpload = new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc), Validado = false, CandidatoId = candidato4.Id },
            new() { Tipo = TipoDocumento.ComprovanteMatricula, NomeArquivo = "comprovante_fernanda_souza.pdf", CaminhoLocal = Path.Combine(caminhoSeed, "comprovante_fernanda_souza.pdf"), DataUpload = new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc), Validado = true, CandidatoId = candidato4.Id },
            new() { Tipo = TipoDocumento.CartaRecomendacao, NomeArquivo = "recomendacao_fernanda_souza.pdf", CaminhoLocal = Path.Combine(caminhoSeed, "recomendacao_fernanda_souza.pdf"), DataUpload = new DateTime(2026, 7, 21, 0, 0, 0, DateTimeKind.Utc), Validado = false, CandidatoId = candidato4.Id },

            new() { Tipo = TipoDocumento.HistoricoEscolar, NomeArquivo = "historico_marcos_paulo.pdf", CaminhoLocal = Path.Combine(caminhoSeed, "historico_marcos_paulo.pdf"), DataUpload = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc), Validado = true, CandidatoId = candidato5.Id },
            new() { Tipo = TipoDocumento.CurriculumLatte, NomeArquivo = "Curriculo Lattes", CaminhoLocal = "", LinkUrl = "http://lattes.cnpq.br/3456789012345678", DataUpload = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc), Validado = true, CandidatoId = candidato5.Id },

            new() { Tipo = TipoDocumento.HistoricoEscolar, NomeArquivo = "historico_juliana_torres.pdf", CaminhoLocal = Path.Combine(caminhoSeed, "historico_juliana_torres.pdf"), DataUpload = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc), Validado = true, CandidatoId = candidato6.Id },
            new() { Tipo = TipoDocumento.CartaIntencao, NomeArquivo = "carta_juliana_torres.pdf", CaminhoLocal = Path.Combine(caminhoSeed, "carta_juliana_torres.pdf"), DataUpload = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc), Validado = true, CandidatoId = candidato6.Id },
            new() { Tipo = TipoDocumento.CartaRecomendacao, NomeArquivo = "recomendacao_juliana_torres.pdf", CaminhoLocal = Path.Combine(caminhoSeed, "recomendacao_juliana_torres.pdf"), DataUpload = new DateTime(2025, 1, 16, 0, 0, 0, DateTimeKind.Utc), Validado = true, CandidatoId = candidato6.Id }
        };

        _context.Documentos.AddRange(documentos);
        await _context.SaveChangesAsync();

        // ============================================================
        // Baremas
        // ============================================================
        var baremas = new List<Barema>
        {
            new()
            {
                CandidatoId = candidato2.Id,
                AvaliadorId = avaliador1.Id,
                CriteriosJson = """{"Originalidade":8.0,"Relevancia":9.0,"Metodologia":7.5,"Apresentacao":8.5}""",
                NotaFinal = 8.25f,
                Observacoes = "Candidata bem preparada. Projeto relevante para a area.",
                DataPreenchimento = new DateTime(2026, 7, 10, 0, 0, 0, DateTimeKind.Utc),
                Status = StatusBarema.Concluido
            },
            new()
            {
                CandidatoId = candidato1.Id,
                AvaliadorId = avaliador2.Id,
                CriteriosJson = """{"Originalidade":7.0,"Relevancia":7.0,"Metodologia":6.5,"Apresentacao":7.5}""",
                NotaFinal = 7.0f,
                Observacoes = "Projeto interessante, mas precisa de ajustes na metodologia.",
                DataPreenchimento = new DateTime(2026, 7, 12, 0, 0, 0, DateTimeKind.Utc),
                Status = StatusBarema.EmPreenchimento
            },
            new()
            {
                CandidatoId = candidato4.Id,
                AvaliadorId = avaliador3.Id,
                CriteriosJson = null,
                NotaFinal = 0f,
                Observacoes = "",
                DataPreenchimento = null,
                Status = StatusBarema.Pendente
            },
            new()
            {
                CandidatoId = candidato5.Id,
                AvaliadorId = avaliador4.Id,
                CriteriosJson = """{"Originalidade":9.0,"Relevancia":8.5,"Metodologia":9.5,"Apresentacao":9.0}""",
                NotaFinal = 9.0f,
                Observacoes = "Excelente projeto. Candidato demonstra dominio do tema.",
                DataPreenchimento = new DateTime(2026, 8, 15, 0, 0, 0, DateTimeKind.Utc),
                Status = StatusBarema.Concluido
            }
        };

        _context.Baremas.AddRange(baremas);
        await _context.SaveChangesAsync();

        Console.WriteLine("[SeedData] Preenchimento automatico concluido com sucesso!");
        Console.WriteLine("[SeedData] 3 Processos, 4 Avaliadores, 6 Candidatos, 15 Documentos, 4 Baremas");
    }
}
