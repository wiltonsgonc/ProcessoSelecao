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
echo "  Build Frontend - ProcessoSelecao"
echo "========================================"
echo ""

echo "[1/2] Building frontend sem cache..."
podman compose "${COMPOSE_FILE_ARGS[@]}" build --no-cache frontend

echo ""
echo "[2/2] Iniciando containers..."
podman compose "${COMPOSE_FILE_ARGS[@]}" up -d frontend

echo ""
echo "========================================"
echo "  Build concluido"
echo "========================================"
