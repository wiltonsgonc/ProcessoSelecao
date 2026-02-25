# Sistema de Seleção IC/Pesquisa

Sistema para gerenciamento de processos de seleção de Iniciação Científica e Pesquisa.

## Arquitetura

- **Backend**: .NET 10 Web API com Entity Framework Core
- **Frontend**: Angular 19 com Angular Material
- **Banco de Dados**: SQL Server 2022
- **Containers**: Docker
- **Autenticação**: JWT

## Estrutura do Projeto

```
ProcessoSelecao/
├── src/
│   ├── backend/
│   │   ├── ProcessoSelecao.Domain/       # Entidades e Interfaces
│   │   ├── ProcessoSelecao.Infrastructure/  # DbContext e Repositórios
│   │   ├── ProcessoSelecao.Application/  # DTOs e Services
│   │   └── ProcessoSelecao.Api/          # Controllers e Configuração
│   └── frontend/                         # Angular App
├── scripts/                              # Scripts de inicialização
├── docker-compose.yml                    # Orquestração dos containers
├── diagramas/                            # Diagramas UML (PUML)
└── README.md
```

## Pré-requisitos

- Docker instalado e em execução
- .NET 10 SDK (para desenvolvimento local)
- Node.js 18+ e npm (para desenvolvimento local)
- Angular CLI 19 (opcional): `npm install -g @angular/cli`

## Executar o Ambiente

### Opção 1: Usando docker-compose

```bash
docker-compose up -d
```

### Opção 2: Usando scripts

```bash
chmod +x scripts/*.sh
./scripts/start-dev.sh
```

### Opção 3: Windows (PowerShell/CMD)

```cmd
scripts\start.bat
```

## Acessos

- **Frontend**: http://localhost:4200
- **Backend API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **SQL Server**: localhost:1433 (user: sa, password: Processo@123)

## Funcionalidades

### Módulo Processo de Seleção
- Criar, editar, iniciar e finalizar processos
- Definir número de vagas disponíveis

### Módulo Candidatos
- Cadastrar candidatos com matrícula e email
- Associar candidatos a processos
- Visualizar pontuação média

### Módulo Documentos
- Upload de documentos (Histórico, Comprovante, Cartas, etc.)
- Validação de documentos
- Download de arquivos

### Módulo Avaliadores
- Cadastrar avaliadores internos e externos
- Associar avaliadores a processos

### Módulo Baremas
- Criar baremas de avaliação
- Definir critérios e notas
- Calcular nota final

## APIs Disponíveis

| Endpoint | Descrição |
|----------|-----------|
| GET/POST/PUT/DELETE /api/candidatos | Gestão de candidatos |
| GET/POST/PUT/DELETE /api/documentos | Gestão de documentos |
| GET/POST/PUT/DELETE /api/avaliadores | Gestão de avaliadores |
| GET/POST/PUT/DELETE /api/baremas | Gestão de baremas |
| GET/POST/PUT/DELETE /api/processosselecao | Gestão de processos |

## Parar o Ambiente

```bash
docker-compose down

# Ou com script
./scripts/stop.sh
```

## Desenvolvimento Local (sem containers)

### Backend

```bash
cd src/backend/ProcessoSelecao.Api
dotnet restore
dotnet ef database update
dotnet run
```

### Frontend

```bash
cd src/frontend
npm install
npm start
```

## Variáveis de Ambiente

| Variável | Descrição | Padrão |
|----------|-----------|--------|
| ConnectionStrings__DefaultConnection | String de conexão SQL Server | - |
| JwtSettings__SecretKey | Chave secreta JWT | - |
| JwtSettings__Issuer | Emissor do JWT | - |
| JwtSettings__Audience | Audiência do JWT | - |
| EmailSettings__SmtpHost | Servidor SMTP | smtp.example.com |
| EmailSettings__SmtpPort | Porta SMTP | 587 |
| EmailSettings__SmtpUser | Usuário SMTP | - |
| EmailSettings__SmtpPassword | Senha SMTP | - |
