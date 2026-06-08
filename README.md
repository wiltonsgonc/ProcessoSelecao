# Sistema de Seleção IC/Pesquisa

Sistema para gerenciamento de processos de seleção de Iniciação Científica e Pesquisa.

## Arquitetura

- **Backend**: .NET 10 Web API com Entity Framework Core
- **Frontend**: Angular 19 com Angular Material
- **Banco de Dados**: SQL Server 2022
- **Container**: Podman (compatível com Docker Compose)
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
├── scripts/
│   ├── build-full.sh                     # Build completo (backend + frontend)
│   ├── build-backend.sh                  # Build apenas do backend
│   ├── build-frontend.sh                 # Build apenas do frontend
│   └── reset-db.sh                       # Reset do banco de dados
├── docker-compose.yml                    # Orquestração produção
├── docker-compose.dev.yml                # Override para desenvolvimento
├── .env                                  # Variáveis de ambiente
└── README.md
```

## Pré-requisitos

- Podman 4+ e podman-compose instalados no Debian/WSL
- .NET 10 SDK (para desenvolvimento local sem containers)
- Node.js 22+ e npm (para desenvolvimento local sem containers)

### Instalação rápida do Podman no Debian

```bash
sudo apt update && sudo apt install -y podman podman-compose
```

## Executar o Ambiente

### Modo Produção (imagens otimizadas, sem hot-reload)

```bash
# Via script automatizado
./scripts/build-full.sh

# Ou manualmente
podman compose up -d
```

| Componente | Porta |
|------------|-------|
| Frontend   | http://localhost:4200 |
| Backend API | http://localhost:5002 |
| Swagger    | http://localhost:5002/swagger |
| SQL Server | localhost:1433 |

### Modo Desenvolvimento (hot-reload + volumes de código)

```bash
# Via script automatizado (flag -d ou --dev)
./scripts/build-full.sh --dev

# Ou manualmente com compose files
podman compose -f docker-compose.yml -f docker-compose.dev.yml up -d
```

Neste modo:
- **Backend**: usa `Dockerfile.dev` com `dotnet watch run` — alterações no código reiniciam o servidor automaticamente
- **Frontend**: usa `target: development` com `ng serve --poll 2000` — alterações refletem em tempo real (HMR)
- Volumes montam o código fonte diretamente, sem necessidade de rebuild a cada alteração

### Build individual

```bash
# Apenas backend (produção)
./scripts/build-backend.sh

# Apenas backend (desenvolvimento)
./scripts/build-backend.sh --dev

# Apenas frontend (produção)
./scripts/build-frontend.sh

# Apenas frontend (desenvolvimento)
./scripts/build-frontend.sh --dev
```

### Reset do banco de dados

```bash
./scripts/reset-db.sh
```

Acessos do banco:
- **Admin (sa)**: senha definida em `SA_PASSWORD` no `.env`
- **App (db_user)**: senha definida em `DB_EXTERNAL_PASSWORD` no `.env`

## Comandos Úteis

```bash
# Ver status dos containers
podman ps

# Logs
podman logs processo-selecao-backend
podman logs processo-selecao-frontend
podman logs processo-selecao-sqlserver

# Parar tudo
podman compose down

# Reconstruir do zero (sem cache)
podman compose build --no-cache && podman compose up -d

# Remover imagens antigas
podman rmi localhost/processoselecao_backend:latest localhost/processoselecao_frontend:latest
```

## Acessos

- **Frontend**: http://localhost:4200
- **Backend API**: http://localhost:5002
- **Swagger**: http://localhost:5002/swagger
- **SQL Server**: localhost:1433
  - **Admin**: sa / definido no `.env`
  - **App**: db_user / definido no `.env`
  - **Database**: ProcessoSelecaoDb

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

## Acesso ao Banco de Dados

### Conexão via DBeaver ou SSMS

**Configurações de conexão:**
- **Servidor**: localhost,1433
- **Autenticação**: SQL Server Authentication
- **Usuário**: db_user
- **Senha**: definida em `DB_EXTERNAL_PASSWORD` no `.env`
- **Banco de dados**: ProcessoSelecaoDb

### Conexão via Command Line

```bash
# Conectar usando sqlcmd
podman exec -it processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U db_user -P "$(grep DB_EXTERNAL_PASSWORD .env | cut -d= -f2)" -C \
  -d ProcessoSelecaoDb
```

### Backup e Restauração

```bash
# Criar backup
podman exec processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$(grep SA_PASSWORD .env | cut -d= -f2)" -C \
  -Q "BACKUP DATABASE [ProcessoSelecaoDb] TO DISK = '/var/opt/mssql/backup/backup.bak' WITH COMPRESSION"

# Restaurar backup
podman exec processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$(grep SA_PASSWORD .env | cut -d= -f2)" -C \
  -Q "RESTORE DATABASE [ProcessoSelecaoDb] FROM DISK = '/var/opt/mssql/backup/backup.bak' WITH REPLACE"
```

## APIs Disponíveis

| Endpoint | Descrição |
|----------|-----------|
| GET/POST/PUT/DELETE /api/candidatos | Gestão de candidatos |
| GET/POST/PUT/DELETE /api/documentos | Gestão de documentos |
| GET/POST/PUT/DELETE /api/avaliadores | Gestão de avaliadores |
| GET/POST/PUT/DELETE /api/baremas | Gestão de baremas |
| GET/POST/PUT/DELETE /api/processosselecao | Gestão de processos |

## Variáveis de Ambiente

Copie o `.env.example` para `.env` e preencha os valores:

```bash
cp .env.example .env
```

| Variável | Descrição | Padrão |
|----------|-----------|--------|
| `SA_PASSWORD` | Senha do usuário SA do SQL Server | — |
| `DB_NAME` | Nome do banco de dados | ProcessoSelecaoDb |
| `DB_USER` | Usuário do banco | sa |
| `DB_PASSWORD` | Senha do usuário do banco | — |
| `DB_EXTERNAL_USER` | Usuário externo (DBeaver) | db_user |
| `DB_EXTERNAL_PASSWORD` | Senha do usuário externo | — |
| `JWT_SECRETKEY` | Chave secreta JWT (mín. 32 caracteres) | — |
| `JWT_ISSUER` | Emissor do JWT | — |
| `JWT_AUDIENCE` | Audiência do JWT | — |
| `SMTP_HOST` | Servidor SMTP | smtp.example.com |
| `SMTP_PORT` | Porta SMTP | 587 |
| `SMTP_USER` | Usuário SMTP | — |
| `SMTP_PASSWORD` | Senha SMTP | — |
| `FROM_EMAIL` | Email remetente | noreply@processoselecao.com |
