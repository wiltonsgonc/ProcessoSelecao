#!/bin/bash
set -euo pipefail

DEV_MODE=false
ENV_FILE=""
COMPOSE_FILE_ARGS=()

# ==============================================
# Auto-detectar runtime de container
# Suporta: docker compose, docker-compose, podman compose, podman-compose
# ==============================================
detect_runtime() {
  # Verifica se o daemon Docker realmente esta rodando (docker info precisa do socket).
  # Em VPS com podman-docker, o binario docker existe mas o daemon nao.
  if command -v docker &>/dev/null && docker info &>/dev/null 2>&1; then
    if docker compose version &>/dev/null 2>&1; then
      COMPOSE_CMD="docker compose"
      RUNTIME="docker"
      echo "Runtime detectado: Docker (compose plugin)"
      return
    fi
    if command -v docker-compose &>/dev/null && docker-compose version &>/dev/null 2>&1; then
      COMPOSE_CMD="docker-compose"
      RUNTIME="docker"
      echo "Runtime detectado: Docker (docker-compose standalone)"
      return
    fi
  fi

  if command -v podman &>/dev/null && podman info &>/dev/null 2>&1; then
    if podman compose version &>/dev/null 2>&1; then
      COMPOSE_CMD="podman compose"
      RUNTIME="podman"
      echo "Runtime detectado: Podman (compose plugin)"
      return
    fi
    if command -v podman-compose &>/dev/null && podman-compose version &>/dev/null 2>&1; then
      COMPOSE_CMD="podman-compose"
      RUNTIME="podman"
      echo "Runtime detectado: Podman (podman-compose standalone)"
      return
    fi
  fi

  echo "Erro: Nenhum runtime de container encontrado."
  echo "Instale um dos seguintes:"
  echo "  - Docker Desktop + Docker Compose plugin"
  echo "  - Docker + docker-compose standalone"
  echo "  - Podman + podman-compose"
  exit 1
}

# ==============================================
# Verifica a versao do podman-compose para
# advertir sobre suporte a 'condition: service_healthy'
# ==============================================
check_podman_compose_version() {
  if [[ "$RUNTIME" != "podman" ]]; then
    return
  fi

  local version
  version=$(podman-compose version 2>/dev/null | grep -oP '\d+\.\d+' | head -1 || echo "0.0")
  local major minor
  major=$(echo "$version" | cut -d. -f1)
  minor=$(echo "$version" | cut -d. -f2)

  if [[ "$major" -lt 1 ]] || { [[ "$major" -eq 1 ]] && [[ "$minor" -lt 2 ]]; }; then
    echo ""
    echo "AVISO: podman-compose $version detectado."
    echo "  Versoes < 1.2 nao suportam 'condition: service_healthy' no depends_on."
    echo "  O script vai aguardar o SQL Server manualmente antes de subir o backend."
    echo "  Para suporte completo, atualize: pip install --upgrade podman-compose"
    echo ""
    PODMAN_COMPOSE_NO_HEALTHCHECK=true
  else
    PODMAN_COMPOSE_NO_HEALTHCHECK=false
  fi
}

# ==============================================
# Pre-baixa imagens base necessarias para o build.
# Evita falhas em ambientes sem as imagens em cache.
# ==============================================
pull_base_images() {
  echo ""
  echo ">>> Pre-baixando imagens base necessarias..."

  local images=(
    "mcr.microsoft.com/mssql/server:2022-latest"
    "mcr.microsoft.com/dotnet/sdk:10.0"
    "mcr.microsoft.com/dotnet/aspnet:10.0"
  )

  local pull_cmd
  if [[ "$RUNTIME" == "podman" ]]; then
    pull_cmd="podman pull"
  else
    pull_cmd="docker pull"
  fi

  for img in "${images[@]}"; do
    echo "  Baixando $img ..."
    if $pull_cmd "$img" 2>&1 | tail -1; then
      echo "  OK: $img"
    else
      echo "  AVISO: Falha ao baixar $img (pode ja estar em cache ou nao ser usada)"
    fi
  done
  echo ""
}

# ==============================================
# Aguarda o SQL Server ficar pronto apos o 'up -d'.
# Necessario para Podman sem suporte a service_healthy
# e como verificacao extra no Docker.
# ==============================================
wait_for_sqlserver() {
  local sa_password="$1"
  local max_attempts=90   # 90 * 5s = 7.5 minutos
  local attempt=0
  local container="processo-selecao-sqlserver"

  echo ""
  echo ">>> Aguardando SQL Server ficar pronto (pode levar ate 7 min no primeiro start)..."

  # Determina o exec command
  local exec_cmd
  if [[ "$RUNTIME" == "podman" ]]; then
    exec_cmd="podman exec $container"
  else
    exec_cmd="docker exec $container"
  fi

  while [[ $attempt -lt $max_attempts ]]; do
    attempt=$((attempt + 1))

    if $exec_cmd /opt/mssql-tools18/bin/sqlcmd \
        -S localhost -U sa -P "$sa_password" -C \
        -Q "SELECT 1" > /dev/null 2>&1; then
      echo "  SQL Server pronto apos ${attempt} tentativas!"
      return 0
    fi

    echo "  Tentativa ${attempt}/${max_attempts}..."
    sleep 5
  done

  echo ""
  echo "ERRO: SQL Server nao ficou pronto em tempo habil."
  echo "Verifique os logs: ${RUNTIME} logs processo-selecao-sqlserver"
  exit 1
}

# ==============================================
# Sobe apenas sqlserver e aguarda, depois sobe o resto.
# Usado quando podman-compose nao suporta service_healthy.
# ==============================================
start_with_manual_wait() {
  local sa_password="$1"

  echo ""
  echo ">>> Modo compatibilidade: subindo SQL Server isolado primeiro..."
  $COMPOSE_CMD "${COMPOSE_FILE_ARGS[@]}" --env-file "$ENV_FILE" up -d sqlserver

  wait_for_sqlserver "$sa_password"

  echo ""
  echo ">>> Subindo backend e frontend..."
  $COMPOSE_CMD "${COMPOSE_FILE_ARGS[@]}" --env-file "$ENV_FILE" up -d backend frontend
}

# ==============================================
# Script principal
# ==============================================

detect_runtime
check_podman_compose_version

# Parsear argumentos
while [[ $# -gt 0 ]]; do
  case "$1" in
    -d|--dev)
      DEV_MODE=true
      shift
      ;;
    *)
      echo "Uso: $0 [-d|--dev]"
      exit 1
      ;;
  esac
done

# Montar argumentos dos compose files
if [ "$DEV_MODE" = true ]; then
  COMPOSE_FILE_ARGS+=("-f" "docker-compose.yml" "-f" "docker-compose.dev.yml")
  ENV_FILE=".env.dev"
  echo "Modo: DESENVOLVIMENTO"
else
  COMPOSE_FILE_ARGS+=("-f" "docker-compose.yml" "-f" "docker-compose.prod.yml")
  ENV_FILE=".env.prod"
  echo "Modo: PRODUCAO"
fi

# Verificar se o arquivo .env existe
if [ ! -f "$ENV_FILE" ]; then
  echo "Erro: Arquivo $ENV_FILE nao encontrado."
  echo "Copie o .env.example para $ENV_FILE e preencha os valores."
  echo "  cp .env.example $ENV_FILE"
  exit 1
fi

# Extrair SA_PASSWORD do env file para o wait manual
SA_PASSWORD_VALUE=$(grep -E '^SA_PASSWORD=' "$ENV_FILE" | cut -d= -f2- | tr -d '"'"'" || true)
if [[ -z "$SA_PASSWORD_VALUE" ]]; then
  echo "Erro: SA_PASSWORD nao encontrado em $ENV_FILE"
  exit 1
fi

echo "========================================"
echo "  Build Full - ProcessoSelecao"
echo "========================================"
echo ""

echo "[1/4] Parando containers existentes..."
$COMPOSE_CMD "${COMPOSE_FILE_ARGS[@]}" --env-file "$ENV_FILE" down

echo ""
echo "[2/4] Pre-baixando imagens base..."
pull_base_images

echo ""
echo "[3/4] Building imagens da aplicacao..."
$COMPOSE_CMD "${COMPOSE_FILE_ARGS[@]}" --env-file "$ENV_FILE" build --no-cache backend frontend

echo ""
echo "[4/4] Iniciando containers..."

# Verificar se deve usar modo de compatibilidade para Podman antigo
if [[ "${PODMAN_COMPOSE_NO_HEALTHCHECK:-false}" == "true" ]]; then
  start_with_manual_wait "$SA_PASSWORD_VALUE"
else
  $COMPOSE_CMD "${COMPOSE_FILE_ARGS[@]}" --env-file "$ENV_FILE" up -d

  # No Docker, ainda fazemos uma verificacao extra por seguranca
  if [[ "$RUNTIME" == "docker" ]]; then
    wait_for_sqlserver "$SA_PASSWORD_VALUE"
  fi
fi

echo ""
echo "========================================"
echo "  Build concluido com sucesso!"
echo "========================================"
echo ""
echo "Acessos:"
echo "  Frontend:   http://localhost:4200"
echo "  Backend:    http://localhost:5002"
echo "  Swagger:    http://localhost:5002/swagger"
if [ "$DEV_MODE" = true ]; then
  echo "  SQL Server: localhost:1433"
fi
echo ""
echo "Logs em tempo real:"
echo "  ${RUNTIME} logs -f processo-selecao-sqlserver"
echo "  ${RUNTIME} logs -f processo-selecao-backend"
