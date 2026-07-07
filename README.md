# Sistema de Selecao IC/Pesquisa

Sistema para gerenciamento de processos de selecao de Iniciacao Cientifica e Pesquisa.

## Arquitetura

- **Backend**: .NET 10 Web API com Entity Framework Core
- **Frontend**: Angular 19 com Angular Material
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
│   │   └── ProcessoSelecao.Api/           # Controllers e Configuracao
│   │       ├── Dockerfile.dev             # Dev: dotnet watch (single-stage)
│   │       └── Dockerfile.prod            # Prod: build + runtime (multi-stage)
│   └── frontend/                          # Angular App
│       ├── Dockerfile.dev                 # Dev: ng serve (node)
│       ├── Dockerfile.prod                # Prod: build + nginx
│       └── nginx.conf                     # Configuracao nginx (producao)
├── docker/
│   ├── nginx/conf.d/                      # Configuracoes Nginx (dev/prod)
│   └── crontab                            # Cron para supercronic
├── scripts/
│   ├── build-full.sh                      # Build completo (backend + frontend)
│   ├── build-backend.sh                   # Build apenas do backend
│   ├── build-frontend.sh                  # Build apenas do frontend
│   ├── start-containers.sh               # Iniciar containers sem rebuild
│   ├── down-containers.sh                # Parar e remover containers
│   └── reset-db.sh                        # Reset do banco de dados
├── docker-compose.yml                     # Compose base (prod)
├── docker-compose.dev.yml                 # Override para desenvolvimento
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
- Node.js 24+ e npm (para desenvolvimento local sem containers)
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

**1. SQL Server via Docker:**

```bash
docker run -d --name processo-selecao-sqlserver \
  -e ACCEPT_EULA=Y \
  -e 'MSSQL_SA_PASSWORD=Str0ng!Pass2026' \
  -e MSSQL_PID=Developer \
  -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest
```

Aguardar ~60 segundos para inicializacao. Verificar:

```bash
docker exec -it processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'Str0ng!Pass2026' -C -Q "SELECT 1"
```

**2. Inicializar banco de dados:**

```bash
# Copiar init.sql para dentro do container
docker cp init.sql processo-selecao-sqlserver:/tmp/init.sql

# Executar o script
docker exec -it processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'Str0ng!Pass2026' -C \
  -d master -i /tmp/init.sql \
  -v DB_EXTERNAL_USER='db_user' \
  -v DB_EXTERNAL_PASSWORD='ExtP@ssw0rd2026' \
  -v DB_NAME='ProcessoSelecaoDb'
```

**3. Backend (.NET):**

```bash
cd src/backend

# Restaurar pacotes
dotnet restore

# Executar com hot-reload
dotnet watch run --project ProcessoSelecao.Api --urls http://localhost:5002

# Executar sem hot-reload
dotnet run --project ProcessoSelecao.Api --urls http://localhost:5002
```

**4. Frontend (Angular):**

```bash
cd src/frontend

# Instalar dependencias
npm install

# Iniciar servidor de desenvolvimento
npm start
# Acessa: http://localhost:4200
```

**5. Variaveis de ambiente:**

Edite `src/backend/ProcessoSelecao.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ProcessoSelecaoDb;User Id=sa;Password=Str0ng!Pass2026;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "SUA_CHAVE_SECRETA_MINIMO_32_CARACTERES",
    "Issuer": "ProcessoSelecaoApi",
    "Audience": "ProcessoSelecaoWeb"
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

### Comando manual - Docker

```bash
# Desenvolvimento
docker compose -f docker-compose.yml -f docker-compose.dev.yml --env-file .env.dev up -d

# Producao
docker compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.prod up -d --build
```

### Comando manual - Podman

```bash
# Desenvolvimento
podman compose -f docker-compose.yml -f docker-compose.dev.yml --env-file .env.dev up -d

# Producao
podman compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.prod up -d --build
```

### Comando manual - docker-compose (standalone)

```bash
# Desenvolvimento
docker-compose -f docker-compose.yml -f docker-compose.dev.yml --env-file .env.dev up -d

# Producao
docker-compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.prod up -d --build
```

### Comando manual - podman-compose (standalone)

```bash
# Desenvolvimento
podman-compose -f docker-compose.yml -f docker-compose.dev.yml --env-file .env.dev up -d

# Producao
podman-compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.prod up -d --build
```

### Modo Desenvolvimento

Neste modo:
- **Backend**: usa `Dockerfile.dev` com `dotnet watch run` -- alteracoes no codigo reiniciam o servidor automaticamente
- **Frontend**: usa `target: development` com `ng serve --poll 2000` -- alteracoes refletem em tempo real (HMR)
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
# Ver status dos containers
docker ps          # ou: podman ps

# Logs
docker logs processo-selecao-backend      # ou: podman logs ...
docker logs processo-selecao-frontend
docker logs processo-selecao-sqlserver

# Parar tudo
docker compose down          # ou: podman compose down

# Reconstruir do zero (sem cache)
docker compose build --no-cache && docker compose up -d
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

### Modulo Processo de Selecao
- Criar, editar, iniciar e finalizar processos
- Definir numero de vagas disponiveis

### Modulo Candidatos
- Cadastrar candidatos com matricula e email
- Associar candidatos a processos
- Visualizar pontuacao media

### Modulo Documentos
- Upload de documentos (Historico, Comprovante, Cartas, etc.)
- Validacao de documentos
- Download de arquivos

### Modulo Avaliadores
- Cadastrar avaliadores internos e externos
- Associar avaliadores a processos

### Modulo Baremas
- Criar baremas de avaliacao
- Definir criterios e notas
- Calcular nota final

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
# Docker
docker exec -it processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U db_user -P "$(grep DB_EXTERNAL_PASSWORD .env.dev | cut -d= -f2)" -C \
  -d ProcessoSelecaoDb

# Podman
podman exec -it processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U db_user -P "$(grep DB_EXTERNAL_PASSWORD .env.dev | cut -d= -f2)" -C \
  -d ProcessoSelecaoDb
```

### Backup e Restauracao

```bash
# Criar backup (substitua .env.dev por .env.prod em producao)
docker exec processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$(grep SA_PASSWORD .env.dev | cut -d= -f2)" -C \
  -Q "BACKUP DATABASE [ProcessoSelecaoDb] TO DISK = '/var/opt/mssql/backup/backup.bak' WITH COMPRESSION"

# Restaurar backup
docker exec processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$(grep SA_PASSWORD .env.dev | cut -d= -f2)" -C \
  -Q "RESTORE DATABASE [ProcessoSelecaoDb] FROM DISK = '/var/opt/mssql/backup/backup.bak' WITH REPLACE"
```

## APIs Disponiveis

| Endpoint | Descricao |
|----------|-----------|
| GET/POST/PUT/DELETE /api/candidatos | Gestao de candidatos |
| GET/POST/PUT/DELETE /api/documentos | Gestao de documentos |
| GET/POST/PUT/DELETE /api/avaliadores | Gestao de avaliadores |
| GET/POST/PUT/DELETE /api/baremas | Gestao de baremas |
| GET/POST/PUT/DELETE /api/processosselecao | Gestao de processos |

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
- User no container usa `appuser` (UID 1000) para compatibilidade com Podman rootless

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

Se ainda falhar, use `podman-compose` (standalone Python) diretamente:

```bash
# O standalone nao usa netavark/aardvark-dns
podman-compose -f docker-compose.yml -f docker-compose.dev.yml --env-file .env.dev up -d
```

Ou instale o Docker no WSL Ubuntu e use-o no lugar do Podman no Alma.



### Erro: "No space left on device" (Nao ha espaco em disco)

```bash
# Verificar espaco
df -h

# Limpar imagens e containers nao utilizados
podman system prune -a
# ou
docker system prune -a
```

### Erro: Containers nao iniciam apos rebuild

```bash
# Verificar logs
podman logs processo-selecao-backend
podman logs processo-selecao-sqlserver

# Limpar volumes e reconstruir
podman compose -f docker-compose.yml -f docker-compose.dev.yml --env-file .env.dev down -v
podman compose -f docker-compose.yml -f docker-compose.dev.yml --env-file .env.dev up -d --build
```

### Erro: SQL Server nao fica pronto (healthcheck timeout)

O SQL Server 2022 precisa de no minimo 2GB de RAM. Em WSL ou VPS com pouca memoria, ele pode nao iniciar a tempo ou crashar.

```bash
# Verificar logs do SQL Server
docker logs processo-selecao-sqlserver

# Verificar memoria disponivel
docker stats
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
