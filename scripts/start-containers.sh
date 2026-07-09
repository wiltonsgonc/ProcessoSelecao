#!/bin/bash
set -euo pipefail

DEV_MODE=false
ENV_FILE=""
RUNTIME=""

# ==============================================
# Auto-detectar runtime de container
# ==============================================
detect_runtime() {
  if command -v docker &>/dev/null && docker info &>/dev/null 2>&1; then
    RUNTIME="docker"
    echo "Runtime detectado: Docker"
    return
  fi
  if command -v podman &>/dev/null && podman info &>/dev/null 2>&1; then
    RUNTIME="podman"
    echo "Runtime detectado: Podman"
    return
  fi
  echo "Erro: Nenhum runtime de container encontrado (docker ou podman)."
  exit 1
}

# ==============================================
# Aguarda SQL Server ficar pronto via sqlcmd
# ==============================================
wait_for_sqlserver() {
  local sa_password="$1"
  local max_attempts=90
  local attempt=0
  local container="processo-selecao-sqlserver"

  echo ""
  echo ">>> Aguardando SQL Server ficar pronto..."

  while [[ $attempt -lt $max_attempts ]]; do
    attempt=$((attempt + 1))
    if ${RUNTIME} exec "$container" \
        /opt/mssql-tools18/bin/sqlcmd \
        -S localhost -U sa -P "$sa_password" -C \
        -Q "SELECT 1" > /dev/null 2>&1; then
      echo "  SQL Server pronto apos ${attempt} tentativas!"
      return 0
    fi
    echo "  Tentativa ${attempt}/${max_attempts}..."
    sleep 5
  done

  echo "ERRO: SQL Server nao ficou pronto em tempo habil."
  echo "Verifique: ${RUNTIME} logs processo-selecao-sqlserver"
  exit 1
}

# ==============================================
# Remove container se existir (stop + rm)
# ==============================================
remove_container() {
  local name="$1"
  if ${RUNTIME} ps -a --format '{{.Names}}' 2>/dev/null | grep -q "^${name}$"; then
    echo "  Removendo container existente: $name"
    ${RUNTIME} stop "$name" 2>/dev/null || true
    ${RUNTIME} rm "$name" 2>/dev/null || true
  fi
}

# ==============================================
# Cria volume nomeado se nao existir
# ==============================================
create_volume() {
  local name="$1"
  if ${RUNTIME} volume exists "$name" 2>/dev/null; then
    echo "  Volume ja existe: $name"
  else
    echo "  Criando volume: $name"
    ${RUNTIME} volume create "$name"
  fi
}

# ==============================================
# Cria rede bridge se nao existir
# ==============================================
create_network() {
  local name="$1"
  if ${RUNTIME} network exists "$name" 2>/dev/null; then
    echo "  Rede ja existe: $name"
  else
    echo "  Criando rede: $name"
    ${RUNTIME} network create "$name"
  fi
}

# ==============================================
# Main
# ==============================================

detect_runtime

while [[ $# -gt 0 ]]; do
  case "$1" in
    -d|--dev)
      DEV_MODE=true
      shift
      ;;
    *)
      echo "Uso: $0 [-d|--dev]"
      echo ""
      echo "  -d, --dev    Modo desenvolvimento (usa .env.dev, volumes de src)"
      echo "  (padrao)     Modo producao (usa .env.prod)"
      exit 1
      ;;
  esac
done

BACKEND_IMAGE="localhost/processoselecao_backend:latest"
FRONTEND_IMAGE="localhost/processoselecao_frontend:latest"

if [ "$DEV_MODE" = true ]; then
  ENV_FILE=".env.dev"
  echo "Modo: DESENVOLVIMENTO"
else
  ENV_FILE=".env.prod"
  echo "Modo: PRODUCAO"
fi

# Verificar se as imagens existem (evita pull de registry)
for img in "$BACKEND_IMAGE" "$FRONTEND_IMAGE"; do
  if ! ${RUNTIME} image exists "$img" 2>/dev/null; then
    echo "ERRO: Imagem '$img' nao encontrada."
    echo "Execute primeiro: ./scripts/build-full.sh --dev"
    exit 1
  fi
done

if [ ! -f "$ENV_FILE" ]; then
  echo "Erro: Arquivo $ENV_FILE nao encontrado."
  echo "Copie o .env.example para $ENV_FILE e preencha os valores."
  echo "  cp .env.example $ENV_FILE"
  exit 1
fi

SA_PASSWORD_VALUE=$(grep -E '^SA_PASSWORD=' "$ENV_FILE" | cut -d= -f2- | tr -d '"'"'" || true)
if [[ -z "$SA_PASSWORD_VALUE" ]]; then
  echo "Erro: SA_PASSWORD nao encontrado em $ENV_FILE"
  exit 1
fi

echo "========================================"
echo "  Start Containers - ProcessoSelecao"
echo "========================================"
echo ""

# -------------------------------------------------
# [1/5] Rede e volumes
# -------------------------------------------------
echo "[1/5] Preparando rede e volumes..."
create_network processo-selecao-network
create_volume sqlserver_data
create_volume sqlserver_log
create_volume sqlserver_backup

# -------------------------------------------------
# [2/5] SQL Server
# -------------------------------------------------
echo ""
echo "[2/5] (Re)criando SQL Server..."
remove_container processo-selecao-sqlserver

${RUNTIME} run -d \
  --name processo-selecao-sqlserver \
  --network processo-selecao-network \
  --user root \
  --env-file "$ENV_FILE" \
  -e MSSQL_SA_PASSWORD="$SA_PASSWORD_VALUE" \
  -e MSSQL_AGENT_ENABLED="false" \
  -p 1433:1433 \
  -v sqlserver_data:/var/opt/mssql/data \
  -v sqlserver_log:/var/opt/mssql/log \
  -v sqlserver_backup:/var/opt/mssql/backup \
  -v "$(pwd)/init.sql:/docker-entrypoint-initdb.d/init.sql:Z" \
  --health-cmd='/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -Q "SELECT 1" || exit 1' \
  --health-interval=15s \
  --health-retries=30 \
  --health-start-period=120s \
  --health-timeout=15s \
  mcr.microsoft.com/mssql/server:2022-latest \
  bash -c '
/opt/mssql/bin/sqlservr &
SQLSERVER_PID=$!
echo "Aguardando SQL Server iniciar..."
for i in $(seq 1 90); do
  if /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "${MSSQL_SA_PASSWORD}" -C -Q "SELECT 1" > /dev/null 2>&1; then
    echo "SQL Server pronto apos ${i} tentativas"
    break
  fi
  echo "Tentativa ${i}/90 - aguardando 5s..."
  sleep 5
done
echo "Executando init.sql..."
/opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "${MSSQL_SA_PASSWORD}" -C -d master \
  -v DB_EXTERNAL_USER="${DB_EXTERNAL_USER:-db_user}" \
  -v DB_EXTERNAL_PASSWORD="${DB_EXTERNAL_PASSWORD}" \
  -v DB_NAME="${DB_NAME:-ProcessoSelecaoDb}" \
  -i /docker-entrypoint-initdb.d/init.sql && echo "init.sql executado com sucesso"
wait $SQLSERVER_PID
'

# -------------------------------------------------
# [3/5] Aguardar SQL Server
# -------------------------------------------------
echo ""
echo "[3/5] Aguardando SQL Server ficar saudavel..."
wait_for_sqlserver "$SA_PASSWORD_VALUE"

# -------------------------------------------------
# [4/5] Backend
# -------------------------------------------------
echo ""
echo "[4/5] (Re)criando Backend..."
remove_container processo-selecao-backend

if [ "$DEV_MODE" = true ]; then
  ${RUNTIME} run -d --pull never \
    --name processo-selecao-backend \
    --network processo-selecao-network \
    --env-file "$ENV_FILE" \
    -e ASPNETCORE_ENVIRONMENT=Development \
    -e ASPNETCORE_URLS="http://0.0.0.0:5002" \
    -e ConnectionStrings__DefaultConnection="Server=sqlserver;Database=${DB_NAME:-ProcessoSelecaoDb};User Id=${DB_USER:-sa};Password=${SA_PASSWORD_VALUE};TrustServerCertificate=True;" \
    -p 5002:5002 \
    -v "$(pwd)/src/backend:/src:Z" \
    -v "$(pwd)/documentos:/app/documentos:Z" \
    "$BACKEND_IMAGE"
else
  ${RUNTIME} run -d --pull never \
    --name processo-selecao-backend \
    --network processo-selecao-network \
    --env-file "$ENV_FILE" \
    -e ASPNETCORE_ENVIRONMENT=Production \
    -e ASPNETCORE_URLS="http://0.0.0.0:5002" \
    -e ConnectionStrings__DefaultConnection="Server=sqlserver;Database=${DB_NAME:-ProcessoSelecaoDb};User Id=${DB_USER:-sa};Password=${SA_PASSWORD_VALUE};TrustServerCertificate=True;" \
    -p 5002:5002 \
    -v "$(pwd)/documentos:/app/documentos:Z" \
    "$BACKEND_IMAGE"
fi

# -------------------------------------------------
# [5/5] Frontend
# -------------------------------------------------
echo ""
echo "[5/5] (Re)criando Frontend..."
remove_container processo-selecao-frontend

if [ "$DEV_MODE" = true ]; then
  ${RUNTIME} run -d --pull never \
    --name processo-selecao-frontend \
    --network processo-selecao-network \
    --env-file "$ENV_FILE" \
    -e Backend__BaseUrl=http://backend:5002/api \
    -e ASPNETCORE_ENVIRONMENT=Development \
    -p 4200:80 \
    -v "$(pwd)/src/frontend/ProcessoSelecao.Blazor:/app:Z" \
    "$FRONTEND_IMAGE"
else
  ${RUNTIME} run -d --pull never \
    --name processo-selecao-frontend \
    --network processo-selecao-network \
    --env-file "$ENV_FILE" \
    -e Backend__BaseUrl=http://backend:5002/api \
    -e ASPNETCORE_ENVIRONMENT=Production \
    -p 4200:80 \
    "$FRONTEND_IMAGE"
fi

echo ""
echo "========================================"
echo "  Containers iniciados com sucesso!"
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
echo "Logs:"
echo "  ${RUNTIME} logs -f processo-selecao-sqlserver"
echo "  ${RUNTIME} logs -f processo-selecao-backend"
echo "  ${RUNTIME} logs -f processo-selecao-frontend"
