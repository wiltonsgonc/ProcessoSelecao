#!/bin/bash
set -euo pipefail

echo "========================================"
echo "  Aplicar Migrations - ProcessoSelecao"
echo "========================================"
echo ""

# ==============================================
# Auto-detectar runtime de container
# ==============================================
detect_runtime() {
  if command -v docker &>/dev/null && docker ps &>/dev/null 2>&1; then
    RUNTIME="docker"
    echo "Runtime detectado: Docker"
    return
  fi

  if command -v podman &>/dev/null && podman ps &>/dev/null 2>&1; then
    RUNTIME="podman"
    echo "Runtime detectado: Podman"
    return
  fi

  echo "Erro: Nenhum runtime de container encontrado (docker ou podman)"
  exit 1
}

detect_runtime

# Carregar variaveis do .env
if [ -f ".env" ]; then
  ENV_FILE=".env"
elif [ -f ".env.dev" ]; then
  ENV_FILE=".env.dev"
elif [ -f ".env.prod" ]; then
  ENV_FILE=".ENV.prod"
else
  echo "Erro: Nenhum arquivo .env encontrado."
  exit 1
fi

echo "Usando: $ENV_FILE"

# Exportar variaveis do arquivo env
set -a
source <(grep -E '^[A-Z_]+=.' "$ENV_FILE" | sed 's/#.*//' | tr -d '\r')
set +a

# Verificar se o container do backend esta rodando
if ! $RUNTIME ps --format '{{.Names}}' 2>/dev/null | grep -q "processo-selecao-backend" && \
   ! $RUNTIME ps --format '{{.Name}}' 2>/dev/null | grep -q "processo-selecao-backend"; then
  echo "Erro: Container 'processo-selecao-backend' nao esta rodando."
  echo "Inicie os containers primeiro: ./scripts/start-containers.sh"
  exit 1
fi

echo ""
echo "Aplicando migrations no banco de dados..."
echo ""

# Aplicar migrations via dotnet ef dentro do container backend
$RUNTIME exec processo-selecao-backend \
  dotnet ef database update \
  --project /src/ProcessoSelecao.Infrastructure \
  --startup-project /src/ProcessoSelecao.Api \
  --no-build

echo ""
echo "========================================"
echo "  Migrations aplicadas com sucesso!"
echo "========================================"
echo ""
echo "Para verificar as migrations aplicadas:"
echo "  $RUNTIME exec processo-selecao-backend dotnet ef migrations list"
