#!/bin/bash
set -euo pipefail

DEV_MODE=false
COMPOSE_FILE_ARGS=()

# Auto-detectar Docker ou Podman
if command -v docker &>/dev/null && docker compose version &>/dev/null; then
  COMPOSE_CMD="docker compose"
  echo "Runtime detectado: Docker"
elif command -v podman &>/dev/null && podman compose version &>/dev/null; then
  COMPOSE_CMD="podman compose"
  echo "Runtime detectado: Podman"
else
  echo "Erro: Nenhum runtime encontrado (docker compose ou podman compose)"
  exit 1
fi

while [[ $# -gt 0 ]]; do
  case "$1" in
    -d|--dev) DEV_MODE=true; shift ;;
    *) echo "Uso: $0 [-d|--dev]"; exit 1 ;;
  esac
done

if [ "$DEV_MODE" = true ]; then
  COMPOSE_FILE_ARGS+=("-f" "docker-compose.yml" "-f" "docker-compose.dev.yml")
  echo "Modo: DESENVOLVIMENTO"
elif [ -f "docker-compose.prod.yml" ]; then
  COMPOSE_FILE_ARGS+=("-f" "docker-compose.yml" "-f" "docker-compose.prod.yml")
  echo "Modo: PRODUÇÃO"
else
  echo "Modo: PRODUÇÃO (docker-compose.yml)"
fi

echo "========================================"
echo "  Build Full - ProcessoSelecao"
echo "========================================"
echo ""

echo "[1/3] Parando containers existentes..."
$COMPOSE_CMD "${COMPOSE_FILE_ARGS[@]}" down

echo ""
echo "[2/3] Building containers sem cache..."
$COMPOSE_CMD "${COMPOSE_FILE_ARGS[@]}" build --no-cache backend frontend

echo ""
echo "[3/3] Iniciando containers..."
$COMPOSE_CMD "${COMPOSE_FILE_ARGS[@]}" up -d

echo ""
echo "========================================"
echo "  Build concluido"
echo "========================================"
