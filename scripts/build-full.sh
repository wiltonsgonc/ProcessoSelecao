#!/bin/bash
set -euo pipefail

DEV_MODE=false
COMPOSE_FILE_ARGS=()

while [[ $# -gt 0 ]]; do
  case "$1" in
    -d|--dev) DEV_MODE=true; shift ;;
    *) echo "Uso: $0 [-d|--dev]"; exit 1 ;;
  esac
done

if [ "$DEV_MODE" = true ]; then
  COMPOSE_FILE_ARGS+=("-f" "docker-compose.yml" "-f" "docker-compose.dev.yml")
  echo "Modo: DESENVOLVIMENTO"
else
  echo "Modo: PRODUÇÃO"
fi

echo "========================================"
echo "  Build Full - ProcessoSelecao"
echo "========================================"
echo ""

echo "[1/3] Parando containers existentes..."
podman compose "${COMPOSE_FILE_ARGS[@]}" down

echo ""
echo "[2/3] Building containers sem cache..."
podman compose "${COMPOSE_FILE_ARGS[@]}" build --no-cache backend frontend

echo ""
echo "[3/3] Iniciando containers..."
podman compose "${COMPOSE_FILE_ARGS[@]}" up -d

echo ""
echo "========================================"
echo "  Build concluido"
echo "========================================"
