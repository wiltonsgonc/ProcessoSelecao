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
  if command -v docker &>/dev/null && docker compose version &>/dev/null 2>&1; then
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

  if command -v podman &>/dev/null && podman compose version &>/dev/null 2>&1; then
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

  echo "Erro: Nenhum runtime de container encontrado."
  echo "Instale um dos seguintes:"
  echo "  - Docker Desktop + Docker Compose plugin"
  echo "  - Docker + docker-compose standalone"
  echo "  - Podman + podman-compose"
  exit 1
}

detect_runtime

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

if [ "$DEV_MODE" = true ]; then
  COMPOSE_FILE_ARGS+=("-f" "docker-compose.yml" "-f" "docker-compose.dev.yml")
  ENV_FILE=".env.dev"
  echo "Modo: DESENVOLVIMENTO"
else
  COMPOSE_FILE_ARGS+=("-f" "docker-compose.yml" "-f" "docker-compose.prod.yml")
  ENV_FILE=".env.prod"
  echo "Modo: PRODUCAO"
fi

if [ ! -f "$ENV_FILE" ]; then
  echo "Erro: Arquivo $ENV_FILE nao encontrado."
  echo "Copie o .env.example para $ENV_FILE e preencha os valores."
  echo "  cp .env.example $ENV_FILE"
  exit 1
fi

echo "========================================"
echo "  Build Frontend - ProcessoSelecao"
echo "========================================"
echo ""

echo "[1/2] Building frontend sem cache..."
$COMPOSE_CMD "${COMPOSE_FILE_ARGS[@]}" --env-file "$ENV_FILE" build --no-cache frontend

echo ""
echo "[2/2] Iniciando container frontend..."
$COMPOSE_CMD "${COMPOSE_FILE_ARGS[@]}" --env-file "$ENV_FILE" up -d frontend

echo ""
echo "========================================"
echo "  Build frontend concluido"
echo "========================================"
