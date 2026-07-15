# Sistema de Selecao IC/Pesquisa

Sistema para gerenciamento de processos de selecao de Iniciacao Cientifica e Pesquisa.

## Arquitetura

- **Backend**: .NET 10 Web API com Entity Framework Core
- **Frontend**: Blazor Web App (Server-Side) com Tailwind CSS
- **Banco de Dados**: SQL Server 2022
- **Container**: Docker ou Podman (compativel com ambos)
- **Autenticacao**: JWT

## Estrutura do Projeto

```
ProcessoSelecao/
├── src/
│   ├── backend/
│   │   ├── ProcessoSelecao.Domain/        # Entidades e Interfaces
│   │   ├── ProcessoSelecao.Infrastructure/ # DbContext e Repositorios
│   │   ├── ProcessoSelecao.Application/   # DTOs e Services
│   │   ├── ProcessoSelecao.Api/           # Controllers e Configuracao
│   │   │   ├── Dockerfile                 # Dev: dotnet watch (single-stage)
│   │   │   └── Dockerfile.prod            # Prod: build + runtime (multi-stage)
│   │   └── ProcessoSelecao.Tests/         # Testes unitarios (xUnit + Moq)
│   │       ├── Domain/                    # Testes de regras de negocio
│   │       └── Application/               # Testes de services com mocks
│   ├── frontend/
│   │   └── ProcessoSelecao.Blazor/        # Frontend Blazor
│   │       ├── Components/
│   │       │   ├── App.razor              # Shell HTML
│   │       │   ├── Routes.razor           # Router
│   │       │   ├── Layouts/               # AdminLayout, AvaliadorLayout e PublicLayout
│   │       │   └── Pages/
│   │       │       ├── Public/            # Home, ProcessoPublicList
│   │       │       ├── Formulario/        # Wizard de inscricao (4 paginas)
│   │       │       ├── Admin/             # CRUD + Avaliacao
│   │       │       └── Avaliador/         # Portal do avaliador (login, painel, avaliacao)
│   │       ├── Models/                    # DTOs C# (ProcessoSelecao, Candidato, etc.)
│   │       ├── Services/                  # HTTP clients para a API
│   │       ├── wwwroot/css/app.css        # Estilos
│   │       ├── Dockerfile                 # Dev: dotnet watch
│   │       └── Dockerfile.prod            # Prod: build + runtime
│   └── Directory.Build.props              # NuGetAudit habilitado
├── scripts/
│   ├── build-full.sh                      # Build completo (backend + frontend)
│   ├── build-backend.sh                   # Build apenas do backend
│   ├── build-frontend.sh                  # Build apenas do frontend
│   ├── start-containers.sh               # Iniciar containers sem rebuild
│   ├── down-containers.sh                # Parar e remover containers
│   └── reset-db.sh                        # Reset do banco de dados
├── docker-compose.yml                     # Compose base (dev padrao)
├── docker-compose.prod.yml                # Override para producao
├── .env.dev                               # Variaveis de ambiente (dev)
├── .env.prod                              # Variaveis de ambiente (prod)
├── .env.example                           # Template de variaveis
└── README.md
```

## Pre-requisitos

- **Docker** (Docker Desktop ou Docker Engine + Docker Compose) **OU**
- **Podman** (Podman + podman-compose)
- .NET 10 SDK (para desenvolvimento local sem containers)
- Bash (Linux, macOS, ou WSL/Git Bash no Windows)


### Instalacao rapida

**Debian/Ubuntu (Podman):**
```bash
sudo apt update && sudo apt install -y podman podman-compose
```

**Debian/Ubuntu (Docker):**
```bash
# Seguir documentacao oficial: https://docs.docker.com/engine/install/
curl -fsSL https://get.docker.com | sh
```

**Alma Linux/RHEL (Podman):**
```bash
sudo dnf install -y podman podman-compose
# Configurar Podman rootless (se houver erro de permissao):
chmod +x scripts/setup-podman-rootless.sh
./scripts/setup-podman-rootless.sh
```

**Windows:**
- Instalar Docker Desktop OU WSL com Debian/Ubuntu + Podman/Docker

## Configuracao de Ambiente

### 1. Criar arquivos de ambiente

```bash
# Copiar templates
cp .env.example .env.dev
cp .env.example .env.prod
```

### 2. Preencher credenciais

Edite `.env.dev` (desenvolvimento) e `.env.prod` (producao) com seus valores.

**IMPORTANTE**: Nunca use as mesmas senhas em desenvolvimento e producao.

| Variavel | Descricao | Exemplo |
|----------|-----------|---------|
| `SA_PASSWORD` | Senha do usuario SA do SQL Server | Senha forte, min. 8 caracteres |
| `DB_EXTERNAL_PASSWORD` | Senha do usuario externo (db_user) | Diferente de SA_PASSWORD |
| `JWT_SECRETKEY` | Chave secreta JWT (min. 32 caracteres) | String aleatoria longa |
| `SMTP_PASSWORD` | Senha do servidor SMTP | Senha do servico de email |

## Executar o Ambiente

### Desenvolvimento Local (sem containers - Bare Metal)

Para desenvolver fora de containers, apenas o SQL Server roda em container.
O backend e frontend rodam nativamente no sistema host.

**1. SQL Server via Docker ou Podman:**

> **IMPORTANTE - Complexidade da senha:** o SQL Server exige que a senha do `sa`
> tenha no minimo 8 caracteres e contenha caracteres de **pelo menos 3 dos 4**
> conjuntos: maiusculas, minusculas, digitos e simbolos. Placeholders como
> `YOUR_STRONG_PASSWORD` **nao passam** na politica (o container sobe e cai com
> `Exited (255)` e a mensagem `Password validation failed ... not complex enough`).
> Defina uma senha forte propria e substitua `sua_senha` nos comandos abaixo.

```bash
# Substitua 'sua_senha' pela mesma senha definida em SA_PASSWORD no .env (raiz do repo)
podman run -d --name processo-selecao-sqlserver \
  -e ACCEPT_EULA=Y \
  -e 'MSSQL_SA_PASSWORD=sua_senha' \
  -e MSSQL_PID=Developer \
  -p 1433:1433 \
  -v mssql_data:/var/opt/mssql \
  mcr.microsoft.com/mssql/server:2022-latest
```

> Com `podman`, basta trocar `docker` por `podman` no comando acima. O volume
> nomeado `mssql_data` mantem os dados entre recriacoes do container.

Aguardar ~60 segundos para inicializacao. Verificar que o container permanece `Up`
(e nao `Exited`) e que aceita conexao:

```bash
podman ps   # STATUS deve ser "Up" e PORTS deve mostrar 0.0.0.0:1433->1433/tcp

podman exec -it processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'sua_senha' -C -Q "SELECT 1"
```

**2. Inicializar banco de dados (opcional):**

> O backend executa `db.Database.Migrate()` na inicializacao, criando o banco
> `ProcessoSelecaoDb` e todas as tabelas automaticamente usando o usuario `sa`.
> O `init.sql` so e necessario se voce quiser provisionar o usuario externo
> `db_user` (para acesso via DBeaver/SSMS).

```bash
# Copiar init.sql para dentro do container
podman cp init.sql processo-selecao-sqlserver:/tmp/init.sql

# Executar o script
podman exec -it processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'sua_senha' -C \
  -d master -i /tmp/init.sql \
  -v DB_EXTERNAL_USER='db_user' \
  -v DB_EXTERNAL_PASSWORD='sua_senha' \
  -v DB_NAME='ProcessoSelecaoDb'
```

**3. Backend (.NET):**

```bash
cd src/backend

# Restaurar pacotes
dotnet restore

# Executar com hot-reload
dotnet watch run --project ProcessoSelecao.Api

# Executar sem hot-reload
dotnet run --project ProcessoSelecao.Api

# Para testar a API
curl http://localhost:5002/api/health → OK
```

> **Nota:** O `Properties/launchSettings.json` define automaticamente `ASPNETCORE_ENVIRONMENT=Development` e a porta 5002. O `ConfigureKestrel` no `Program.cs` tambem escuta na porta 5000.

**4. Frontend (Blazor):**

```bash
cd src/frontend

# Executar com hot-reload
dotnet watch --project ProcessoSelecao.Blazor
# Acessa: http://localhost:5119

# Executar sem hot-reload
dotnet run --project ProcessoSelecao.Blazor
```

> **Nota:** O Blazor roda em http://localhost:5119 (HTTP) ou https://localhost:7209 (HTTPS).

**5. Variaveis de ambiente (connection string):**

O backend carrega automaticamente um arquivo **`.env` na raiz do repositorio**
(via `DotNetEnv`). Esse carregamento acontece **antes** de `WebApplication.CreateBuilder`,
para que `ConnectionStrings__DefaultConnection` seja lido pela configuracao e
**sobrescreva** o valor de `appsettings.Development.json`. O `Program.cs` procura o
`.env` subindo a partir do diretorio da aplicacao ate encontra-lo, entao funciona
tanto no Windows quanto no WSL/Linux, independente do diretorio de trabalho.

Crie/edite o arquivo `.env` na **raiz do repositorio** com, no minimo:

```env
SA_PASSWORD=sua_senha
# A senha na connection string DEVE ser identica ao SA_PASSWORD do container
ConnectionStrings__DefaultConnection=Server=localhost,1433;Database=ProcessoSelecaoDb;User Id=sa;Password=sua_senha;TrustServerCertificate=True;
```

> **Atencao:** se a senha na connection string nao bater com a senha `sa` do
> container, o backend falha na migration com `Login failed for user 'sa'`
> (SQL error 18456) e o `dotnet watch` fica em "Waiting for a file to change".
> Nesse caso, o frontend Blazor mostra `Connection refused (localhost:5002)`.

O `appsettings.Development.json` mantem a connection string com senha vazia
(placeholder) de proposito -- o valor real vem do `.env` (nunca versionado):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ProcessoSelecaoDb;User Id=sa;Password=;TrustServerCertificate=True;"
  }
}
```

### Usando os scripts (recomendado)

Os scripts detectam automaticamente se voce tem Docker ou Podman instalado.

```bash
# Desenvolvimento (hot-reload)
./scripts/build-full.sh --dev

# Producao (imagens otimizadas)
./scripts/build-full.sh
```

### Comando manual - Contêiner

```bash
# Desenvolvimento
podman compose --env-file .env.dev up -d

# Producao
podman compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.prod up -d --build
```

### Comando manual - Podman

```bash
# Desenvolvimento
podman compose --env-file .env.dev up -d

# Producao
podman compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.prod up -d --build
```

### Comando manual - docker-compose (standalone)

```bash
# Desenvolvimento
podman compose --env-file .env.dev up -d

# Producao
podman compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.prod up -d --build
```

### Comando manual - podman-compose (standalone)

```bash
# Desenvolvimento
podman compose --env-file .env.dev up -d

# Producao
podman compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.prod up -d --build
```

### Modo Desenvolvimento

Neste modo:
- **Backend**: usa `Dockerfile` com `dotnet watch run` -- alteracoes no codigo reiniciam o servidor automaticamente
- **Frontend**: usa `Dockerfile` com `dotnet run` -- Blazor Server com SignalR
- **Volumes**: montam o codigo fonte diretamente, sem necessidade de rebuild a cada alteracao
- **SQL Server**: porta 1433 exposta externamente para acesso via DBeaver/SSMS

### Build individual

```bash
# Apenas backend (producao)
./scripts/build-backend.sh

# Apenas backend (desenvolvimento)
./scripts/build-backend.sh --dev

# Apenas frontend (producao)
./scripts/build-frontend.sh

# Apenas frontend (desenvolvimento)
./scripts/build-frontend.sh --dev
```

### Iniciar containers sem rebuild

Caso as imagens ja tenham sido buildadas anteriormente, use este script para recriar
os containers sem executar o build:

```bash
# Desenvolvimento
./scripts/start-containers.sh --dev

# Producao
./scripts/start-containers.sh
```

O script:
- Cria rede e volumes necessarios se nao existirem
- Remove e recria os containers SQL Server, Backend e Frontend
- Aguarda o SQL Server ficar pronto antes de iniciar o backend
- Em `--dev`: monta o codigo fonte como volume (hot-reload ativo)

### Parar e remover containers

```bash
# Para e remove apenas os containers
./scripts/down-containers.sh

# Remove tambem os volumes nomeados (dados do SQL Server)
./scripts/down-containers.sh --volumes

# Remove volumes e rede
./scripts/down-containers.sh --all
```

### Reset do banco de dados

```bash
./scripts/reset-db.sh
```

Acessos do banco:
- **Admin (sa)**: senha definida em `SA_PASSWORD` no `.env.dev` ou `.env.prod`
- **App (db_user)**: senha definida em `DB_EXTERNAL_PASSWORD` no `.env.dev` ou `.env.prod`

## Comandos Uteis

```bash
# Ver status dos contêineres
podman ps

# Logs
podman logs processo-selecao-backend
podman logs processo-selecao-frontend
podman logs processo-selecao-sqlserver

# Parar tudo
podman compose down

# Reconstruir do zero (sem cache)
podman compose build --no-cache && podman compose up -d
```

## Acessos

- **Frontend (local)**: http://localhost:5119
- **Frontend (Docker)**: http://localhost:4200
- **Backend API**: http://localhost:5002
- **Swagger**: http://localhost:5002/swagger
- **SQL Server**: localhost:1433
  - **Admin**: sa / definido no `.env`
  - **App**: db_user / definido no `.env`
  - **Database**: ProcessoSelecaoDb

## Funcionalidades

### Modulo Processo de Selecao
- Criar, editar, iniciar e finalizar processos
- Definir numero de vagas disponiveis
- Copiar link de inscricao

### Modulo Candidatos
- Cadastrar candidatos com matricula e email
- Associar candidatos a processos
- Visualizar pontuacao media e detalhes

### Modulo Documentos
- Upload de documentos (Historico, Comprovante, Cartas, etc.)
- Validacao de documentos (aprovar/rejeitar com motivo)
- Visualizacao de PDF inline
- Download multiplo (ZIP)

### Modulo Avaliadores
- Cadastrar avaliadores internos e externos
- Associar avaliadores a processos
- Login com CPF + senha (JWT)
- Prevencao de auto-avaliação (CPF do avaliador != CPF do candidato)

### Modulo Portal do Avaliador
- Login autenticado via CPF + senha
- Lista de baremas atribuídos ao avaliador
- Visualização de dados do candidato
- Visualização e download de documentos (PDF inline)
- Avaliação com 4 critérios (Originalidade, Relevancia, Metodologia, Apresentacao)
- Finalização da avaliação com nota final calculada

### Modulo Baremas
- Criar baremas de avaliacao
- Definir criterios (Originalidade, Relevancia, Metodologia, Apresentacao)
- Calcular nota final

### Modulo Avaliacao (Admin)
- Visualização consolidada de todas as avaliações
- Filtro por status (Pendente, Em Andamento, Concluido, Cancelado)
- Resumo geral com contagem e média das notas

### Modulo Inscricao (Publico)
- Formulario multi-step (4 paginas)
- Upload de documentos
- Confirmacao e termos de uso

## Acesso ao Banco de Dados

### Conexao via DBeaver ou SSMS

**Configuracoes de conexao:**
- **Servidor**: localhost,1433
- **Autenticacao**: SQL Server Authentication
- **Usuario**: db_user
- **Senha**: definida em `DB_EXTERNAL_PASSWORD` no `.env`
- **Banco de dados**: ProcessoSelecaoDb

### Conexao via Command Line

```bash
# Podman
podman exec -it processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U db_user -P "$(grep DB_EXTERNAL_PASSWORD .env.dev | cut -d= -f2)" -C \
  -d ProcessoSelecaoDb
```

### Backup e Restauracao

```bash
# Criar backup (substitua .env.dev por .env.prod em producao)
podman exec processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$(grep SA_PASSWORD .env.dev | cut -d= -f2)" -C \
  -Q "BACKUP DATABASE [ProcessoSelecaoDb] TO DISK = '/var/opt/mssql/backup/backup.bak' WITH COMPRESSION"

# Restaurar backup
podman exec processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$(grep SA_PASSWORD .env.dev | cut -d= -f2)" -C \
  -Q "RESTORE DATABASE [ProcessoSelecaoDb] FROM DISK = '/var/opt/mssql/backup/backup.bak' WITH REPLACE"
```

## APIs Disponiveis

| Endpoint | Descricao | Autenticacao |
|----------|-----------|--------------|
| GET/POST/PUT/DELETE /api/candidatos | Gestao de candidatos | Admin |
| GET/POST/PUT/DELETE /api/documentos | Gestao de documentos | Admin |
| GET/POST/PUT/DELETE /api/avaliadores | Gestao de avaliadores | Admin |
| GET/POST/PUT/DELETE /api/baremas | Gestao de baremas | Admin |
| GET/POST/PUT/DELETE /api/processosselecao | Gestao de processos | Admin |
| POST /api/formulario/completa | Inscricao publica | Nao |
| POST /api/avaliador-auth/login | Login do avaliador (CPF + senha) | Nao |
| POST /api/avaliador-auth/definir-senha | Definir senha do avaliador | Nao |
| GET /api/avaliador-painel/baremas | Baremas do avaliador autenticado | JWT |
| GET /api/avaliador-painel/candidato/{id} | Dados do candidato | JWT |
| GET /api/avaliador-painel/documentos/{id} | Documentos do candidato | JWT |
| GET /api/avaliador-painel/documentos/{id}/download | Download de documento | JWT |
| POST /api/avaliador-painel/baremas/{id}/finalizar | Finalizar avaliação | JWT |

## Variaveis de Ambiente

### Como funciona

O projeto utiliza **arquivos `.env` separados** para desenvolvimento e producao:

- `.env.dev` -- Variaveis para ambiente de desenvolvimento
- `.env.prod` -- Variaveis para ambiente de producao
- `.env.example` -- Template para criacao dos acima

Os arquivos `.env.dev` e `.env.prod` **NAO sao versionados** por conterem credenciais.

### Configuracao

```bash
# Criar a partir do template
cp .env.example .env.dev
cp .env.example .env.prod

# Editar com suas credenciais
# IMPORTANTE: Use senhas diferentes para dev e prod
```

### Referencia de variaveis

| Variavel | Descricao | Padrao |
|----------|-----------|--------|
| `ACCEPT_EULA` | Aceite da EULA do SQL Server | Y |
| `SA_PASSWORD` | Senha do usuario SA do SQL Server | -- |
| `MSSQL_PID` | Licenca do SQL Server | Developer |
| `MSSQL_MEMORY_LIMIT_MB` | Limite de RAM do SQL Server (MB) | 2048 |
| `DB_HOST` | Host do banco de dados | sqlserver |
| `DB_NAME` | Nome do banco de dados | ProcessoSelecaoDb |
| `DB_USER` | Usuario do banco | sa |
| `DB_PASSWORD` | Senha do usuario do banco | -- |
| `DB_EXTERNAL_USER` | Usuario externo (DBeaver) | db_user |
| `DB_EXTERNAL_PASSWORD` | Senha do usuario externo | -- |
| `JWT_SECRETKEY` | Chave secreta JWT (min. 32 caracteres) | -- |
| `JWT_ISSUER` | Emissor do JWT | ProcessoSelecaoApi |
| `JWT_AUDIENCE` | Audiencia do JWT | ProcessoSelecaoWeb |
| `SMTP_HOST` | Servidor SMTP | smtp.example.com |
| `SMTP_PORT` | Porta SMTP | 587 |
| `SMTP_USER` | Usuario SMTP | -- |
| `SMTP_PASSWORD` | Senha SMTP | -- |
| `FROM_EMAIL` | Email remetente | noreply@processoselecao.com |
| `ASPNETCORE_ENVIRONMENT` | Ambiente .NET | Development/Production |

## Testes Automatizados

O projeto possui testes unitários na camada backend utilizando **xUnit**, **Moq** e **FluentAssertions**.

### Estrutura

```
src/backend/ProcessoSelecao.Tests/
├── Domain/
│   ├── ProcessoSelecaoTests.cs      # 13 testes - máquina de estados
│   ├── CandidatoTests.cs            # 5 testes - validação documentos e pontuação
│   ├── BaremaTests.cs               # 6 testes - cálculo de nota e completude
│   └── AvaliadorTests.cs            # 2 testes - avaliações pendentes
└── Application/
    ├── AvaliadorServiceTests.cs     # 4 testes - CRUD + validação CPF
    ├── BaremaServiceTests.cs        # 5 testes - criação, finalização, auto-avaliação
    └── AvaliadorAuthServiceTests.cs # 6 testes - login JWT, BCrypt, definição de senha
```

### Executar os testes

```bash
# Rodar todos os testes
dotnet test src/backend/ProcessoSelecao.Tests

# Rodar com verbosity
dotnet test src/backend/ProcessoSelecao.Tests --verbosity normal

# Rodar testes de uma classe específica
dotnet test src/backend/ProcessoSelecao.Tests --filter "FullyQualifiedName~ProcessoSelecaoTests"
```

### Pacotes de teste

| Pacote | Versão | Uso |
|--------|--------|-----|
| xUnit | 2.9.3 | Framework de testes |
| Moq | 4.20.72 | Mocking de interfaces |
| FluentAssertions | 8.10.0 | Assertions legíveis |
| BCrypt.Net-Next | 4.0.3 | Testes de autenticação |

## Compatibilidade

Este projeto foi testado e funciona com:

| Sistema | Runtime | Versao Testada |
|---------|---------|----------------|
| Debian 13 (WSL2) | Podman | 5.4.2 + podman-compose 1.3.0 |
| Ubuntu 24.04 LTS (WSL2) | Docker | 29.5.3 |
| Alma Linux 9 (VPS) | Podman | 5.8.2 + podman-compose 1.5.0 |
| Windows 11 (WSL2) | Docker Desktop | Qualquer versao |

### Notas de compatibilidade

- Os scripts de build detectam automaticamente o runtime instalado
- Suporta `docker compose` (plugin), `docker-compose` (standalone), `podman compose` (plugin) e `podman-compose` (standalone)
- Mounts usam `:Z` para compatibilidade com SELinux (Podman) -- Docker ignora silenciosamente
- Nomes de imagens usam FQIN (`docker.io/library/...`) para compatibilidade com Podman

## Auditoria de Seguranca

### Backend (.NET)

O arquivo `Directory.Build.props` habilita `dotnet nuget audit` automaticamente em todo o solution.

```bash
cd src/backend

# Auditoria de pacotes NuGet (vulnerabilidades conhecidas)
dotnet list package --vulnerable --include-transitive

# Build com auditoria (reporta warnings NU1903)
dotnet build

# Corrigir pacote vulneravel (exemplo: atualizar Swashbuckle)
dotnet add ProcessoSelecao.Api/ProcessoSelecao.Api.csproj package <nome-do-pacote>
```

## Troubleshooting

### Alma Linux 9 no WSL: "sd-bus call: Permission denied" + "aardvark-dns failed to start"

O WSL2 nao suporta completamente sessoes de usuario do systemd. O Podman em modo rootless depende do socket D-Bus (`/run/user/1000/bus`) que nao e criado. Isso afeta:
- **Build**: `crun` nao consegue usar systemd para cgroups
- **Containers**: `netavark`/`aardvark-dns` nao conseguem iniciar

**Solucao completa** (executar UMA vez):

```bash
# Script automatico (recomendado)
chmod +x scripts/setup-podman-rootless.sh
./scripts/setup-podman-rootless.sh
```

Ou manualmente:

```bash
# 1. Configurar Podman para usar cgroupfs
mkdir -p ~/.config/containers
cat > ~/.config/containers/containers.conf << 'EOF'
[engine]
cgroup_manager = "cgroupfs"
events_logger = "file"

[network]
# Desabilitar aardvark-dns (evita erro D-Bus)
dns_bind_port = 0
EOF

# 2. Configurar crun (runtime OCI) para usar cgroupfs
mkdir -p ~/.config/crun
cat > ~/.config/crun/crun.conf << 'EOF'
cgroup:
  manager: "cgroupfs"
EOF

# 3. Configurar subuid/subgid
sudo usermod --add-subuids 100000-165535 --add-subgids 100000-165535 $(whoami)

# 4. Habilitar lingering
sudo loginctl enable-linger $(whoami)

# 5. Ajustar permissoes
sudo mkdir -p /run/user/$(id -u)
sudo chown $(whoami):$(whoami) /run/user/$(id -u)

# 6. Migrar e testar
podman system migrate
podman run --rm docker.io/library/alpine echo "funcionou"
```

Se ainda falhar, use `podman compose` (standalone Python) diretamente:

```bash
# O standalone nao usa netavark/aardvark-dns
podman compose --env-file .env.dev up -d
```

Ou instale o Docker no WSL Ubuntu e use-o no lugar do Podman no Alma.

### Erro: "No space left on device" (Nao ha espaco em disco)

```bash
# Verificar espaco
df -h

# Limpar imagens e containers nao utilizados
podman system prune -a
```

### Erro: Containers nao iniciam apos rebuild

```bash
# Verificar logs
podman logs processo-selecao-backend
podman logs processo-selecao-sqlserver

# Limpar volumes e reconstruir
podman compose --env-file .env.dev down -v
podman compose --env-file .env.dev up -d --build
```

### Erro: SQL Server nao fica pronto (healthcheck timeout)

O SQL Server 2022 precisa de no minimo 2GB de RAM. Em WSL ou VPS com pouca memoria, ele pode nao iniciar a tempo ou crashar.

```bash
# Verificar logs do SQL Server
podman logs processo-selecao-sqlserver

# Verificar memoria disponivel
podman stats
```

**Solucoes:**

1. **Aumentar memoria do WSL**: Crie `%USERPROFILE%\.wslconfig`:
```ini
[wsl2]
memory=8GB
```

2. **Aumentar start_period no docker-compose.yml**:
```yaml
healthcheck:
  retries: 30
  start_period: 120s
```

3. **Reduzir memoria do SQL Server** no `.env.dev`:
```env
MSSQL_MEMORY_LIMIT_MB=1024
```

### WSL2: "inotify" limit reached (hot reload nao funciona)

O WSL2 tem limite padrao de 128 instancias inotify. Para aumentar:

```bash
# No terminal WSL:
sudo sh -c 'echo "fs.inotify.max_user_watches=524288" > /etc/sysctl.d/60-inotify.conf'
sudo sh -c 'echo "fs.inotify.max_user_instances=512" >> /etc/sysctl.d/60-inotify.conf'
sudo sysctl -p /etc/sysctl.d/60-inotify.conf
```

### Erro: "Login failed for user 'sa'" (SQL error 18456) / backend cai na migration

O backend nao consegue autenticar no SQL Server. Causas comuns:

1. **Senha divergente**: a senha em `ConnectionStrings__DefaultConnection` (no `.env`
   da raiz) esta diferente da senha `sa` do container. Confirme:
   ```bash
   # Senha real gravada no container:
   docker inspect processo-selecao-sqlserver \
     --format '{{range .Config.Env}}{{println .}}{{end}}' | grep -i PASSWORD

   # Testar login com a senha esperada:
   docker exec processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd \
     -S localhost -U sa -P 'sua_senha' -C -Q "SELECT name FROM sys.databases"
   ```
2. **`.env` nao carregado**: garanta que existe um `.env` na **raiz do repositorio**
   com `ConnectionStrings__DefaultConnection`. O `Program.cs` carrega esse arquivo
   antes de montar a configuracao.
3. Apos corrigir a senha, **reinicie o backend** -- o `dotnet watch` nao reinicia
   sozinho um processo que ja crashou; ele fica em "Waiting for a file to change".
   Pressione `Ctrl+R` no terminal do watch ou reinicie o `dotnet watch run`.

### Frontend Blazor: "Connection refused (localhost:5002)"

O Blazor Server faz as chamadas HTTP para a API **no lado do servidor**. Esse erro
significa que a API nao esta escutando na porta 5002 -- normalmente porque o backend
crashou (veja o erro de `sa` acima) ou nao foi iniciado. Verifique:

```bash
ss -tlnp | grep 5002        # deve listar o processo ProcessoSelecao
curl http://localhost:5002/api/health   # deve responder "OK"
```

### WSL2: acessar a aplicacao pelo navegador do Windows

Quando o backend e o frontend rodam **dentro do WSL2** (modo NAT), o navegador do
Windows pode nao alcancar `localhost:5119` automaticamente. Se `http://localhost:5119`
nao abrir no Windows, crie um portproxy (PowerShell como administrador):

```powershell
$wslIp = (wsl -d <distro> hostname -I).Trim().Split(' ')[0]
netsh interface portproxy add v4tov4 listenaddress=127.0.0.1 listenport=5119 connectaddress=$wslIp connectport=5119
# opcional, para acessar Swagger/API direto do Windows:
netsh interface portproxy add v4tov4 listenaddress=127.0.0.1 listenport=5002 connectaddress=$wslIp connectport=5002
```

> No modo NAT o IP do WSL muda a cada reinicio; recrie o portproxy quando isso ocorrer.
> A porta 1433 nao precisa de portproxy (o backend acessa o SQL Server dentro do WSL).
> Evite o modo `networkingMode=mirrored` no `.wslconfig`: ele quebra o DNAT do
> Docker/Podman (`Unable to enable DNAT rule: No chain/target/match`).

## Tecnologias

| Componente | Tecnologia |
|------------|------------|
| Backend | .NET 10, Entity Framework Core, ASP.NET Core Web API |
| Frontend | Blazor Web App (Server-Side), .NET 10 |
| CSS | Tailwind CSS (via CDN) |
| Banco de Dados | SQL Server 2022 |
| Autenticacao | JWT + BCrypt |
| Containers | Docker / Podman |
| Testes | xUnit, Moq, FluentAssertions |
| CI/CD | GitHub Actions (opcional) |
