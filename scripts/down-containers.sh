#!/bin/bash
set -euo pipefail

REMOVE_VOLUMES=false
REMOVE_NETWORK=false

RUNTIME=""

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

remove_container() {
  local name="$1"
  if ${RUNTIME} ps -a --format '{{.Names}}' 2>/dev/null | grep -q "^${name}$"; then
    echo "  Removendo $name..."
    ${RUNTIME} stop "$name" 2>/dev/null || true
    ${RUNTIME} rm "$name" 2>/dev/null || true
  else
    echo "  Container $name nao encontrado, ignorando."
  fi
}

detect_runtime

while [[ $# -gt 0 ]]; do
  case "$1" in
    -v|--volumes)
      REMOVE_VOLUMES=true
      shift
      ;;
    --network)
      REMOVE_NETWORK=true
      shift
      ;;
    --all)
      REMOVE_VOLUMES=true
      REMOVE_NETWORK=true
      shift
      ;;
    *)
      echo "Uso: $0 [-v|--volumes] [--network] [--all]"
      echo ""
      echo "  -v, --volumes   Remove tambem os volumes nomeados"
      echo "  --network       Remove a rede processo-selecao-network"
      echo "  --all           Remove volumes e rede (equivalente a -v --network)"
      exit 1
      ;;
  esac
done

echo "========================================"
echo "  Down Containers - ProcessoSelecao"
echo "========================================"
echo ""

echo "[1/3] Parando e removendo containers..."
remove_container processo-selecao-frontend
remove_container processo-selecao-backend
remove_container processo-selecao-sqlserver

if [ "$REMOVE_VOLUMES" = true ]; then
  echo ""
  echo "[2/3] Removendo volumes..."
  for vol in sqlserver_data sqlserver_log sqlserver_backup; do
    if ${RUNTIME} volume exists "$vol" 2>/dev/null; then
      echo "  Removendo volume: $vol"
      ${RUNTIME} volume rm "$vol"
    else
      echo "  Volume $vol nao encontrado, ignorando."
    fi
  done
fi

if [ "$REMOVE_NETWORK" = true ]; then
  echo ""
  echo "[3/3] Removendo rede..."
  if ${RUNTIME} network exists processo-selecao-network 2>/dev/null; then
    ${RUNTIME} network rm processo-selecao-network
    echo "  Rede processo-selecao-network removida."
  else
    echo "  Rede processo-selecao-network nao encontrada, ignorando."
  fi
fi

echo ""
echo "Concluido!"
${RUNTIME} ps -a --filter "name=processo-selecao" 2>/dev/null || true
