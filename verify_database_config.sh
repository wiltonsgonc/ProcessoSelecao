#!/bin/bash
set -euo pipefail

echo "=========================================="
echo "  Database Configuration Verification"
echo "=========================================="
echo ""

ERRORS=0

# ==============================================
# Auto-detectar runtime de container
# ==============================================
detect_runtime() {
  if command -v docker &>/dev/null && docker info &>/dev/null 2>&1; then
    if docker compose version &>/dev/null 2>&1; then
      COMPOSE_CMD="docker compose"
      return
    fi
    if command -v docker-compose &>/dev/null && docker-compose version &>/dev/null 2>&1; then
      COMPOSE_CMD="docker-compose"
      return
    fi
  fi

  if command -v podman &>/dev/null && podman info &>/dev/null 2>&1; then
    if podman compose version &>/dev/null 2>&1; then
      COMPOSE_CMD="podman compose"
      return
    fi
    if command -v podman-compose &>/dev/null && podman-compose version &>/dev/null 2>&1; then
      COMPOSE_CMD="podman-compose"
      return
    fi
  fi

  echo "ERRO: Nenhum runtime de container encontrado"
  exit 1
}

detect_runtime

# Check if docker-compose.yml exists
if [ -f "docker-compose.yml" ]; then
    echo "OK: docker-compose.yml encontrado"
else
    echo "ERRO: docker-compose.yml nao encontrado"
    ERRORS=$((ERRORS + 1))
fi

# Check if docker-compose.prod.yml exists
if [ -f "docker-compose.prod.yml" ]; then
    echo "OK: docker-compose.prod.yml encontrado"
else
    echo "ERRO: docker-compose.prod.yml nao encontrado"
    ERRORS=$((ERRORS + 1))
fi

# Check if init.sql exists
if [ -f "init.sql" ]; then
    echo "OK: init.sql encontrado"
else
    echo "ERRO: init.sql nao encontrado"
    ERRORS=$((ERRORS + 1))
fi

# Check if entrypoint.sh exists
if [ -f "src/backend/entrypoint.sh" ]; then
    echo "OK: entrypoint.sh encontrado"
else
    echo "ERRO: entrypoint.sh nao encontrado"
    ERRORS=$((ERRORS + 1))
fi

# Check if .env files exist
for env_file in .env.dev .env.prod .env.example; do
  if [ -f "$env_file" ]; then
    echo "OK: $env_file encontrado"
  else
    echo "ERRO: $env_file nao encontrado"
    ERRORS=$((ERRORS + 1))
  fi
done

# Check Dockerfiles
for dockerfile in \
  "src/backend/ProcessoSelecao.Api/Dockerfile" \
  "src/backend/ProcessoSelecao.Api/Dockerfile.prod" \
  "src/frontend/ProcessoSelecao.Blazor/Dockerfile" \
  "src/frontend/ProcessoSelecao.Blazor/Dockerfile.prod"; do
  if [ -f "$dockerfile" ]; then
    echo "OK: $dockerfile encontrado"
  else
    echo "ERRO: $dockerfile nao encontrado"
    ERRORS=$((ERRORS + 1))
  fi
done

echo ""
echo "=========================================="
echo "  Validando docker-compose configuration"
echo "=========================================="
echo ""

# Validate docker-compose configuration for each environment
for compose_args in \
  "-f docker-compose.yml --env-file .env.dev" \
  "-f docker-compose.yml -f docker-compose.prod.yml --env-file .env.prod"; do
  if $COMPOSE_CMD $compose_args config > /dev/null 2>&1; then
    echo "OK: Compose config valido: $compose_args"
  else
    echo "ERRO: Compose config invalido: $compose_args"
    $COMPOSE_CMD $compose_args config 2>&1 || true
    ERRORS=$((ERRORS + 1))
  fi
done

echo ""
echo "=========================================="
echo "  Verificando database volumes"
echo "=========================================="
echo ""

# Check if volumes are defined
if grep -q "sqlserver_data:" docker-compose.yml && \
   grep -q "sqlserver_log:" docker-compose.yml && \
   grep -q "sqlserver_backup:" docker-compose.yml; then
    echo "OK: Todos os volumes do banco estao definidos"
else
    echo "ERRO: Alguns volumes do banco estao faltando"
    ERRORS=$((ERRORS + 1))
fi

echo ""
echo "=========================================="
echo "  Verificando database user configuration"
echo "=========================================="
echo ""

# Check if external user is configured in .env files
for env_file in .env.dev .env.prod; do
  if [ -f "$env_file" ]; then
    if grep -q "DB_EXTERNAL_USER" "$env_file" && grep -q "DB_EXTERNAL_PASSWORD" "$env_file"; then
      echo "OK: Usuario externo configurado em $env_file"
    else
      echo "ERRO: Usuario externo nao configurado em $env_file"
      ERRORS=$((ERRORS + 1))
    fi
  fi
done

echo ""
echo "=========================================="
echo "  Verificando init.sql content"
echo "=========================================="
echo ""

# Check if init.sql has required content
if grep -q "CREATE LOGIN" init.sql && \
   grep -q "CREATE USER" init.sql && \
   grep -q "sp_addrolemember" init.sql; then
    echo "OK: init.sql contem configuracao de usuario"
else
    echo "ERRO: init.sql esta faltando configuracao de usuario"
    ERRORS=$((ERRORS + 1))
fi

echo ""
echo "=========================================="
echo "  Verificando entrypoint.sh content"
echo "=========================================="
echo ""

# Check if entrypoint.sh has required content
if grep -q "sqlcmd" src/backend/entrypoint.sh; then
    echo "OK: entrypoint.sh contem aguarda SQL Server"
else
    echo "ERRO: entrypoint.sh esta faltando aguarda SQL Server"
    ERRORS=$((ERRORS + 1))
fi

echo ""
echo "=========================================="
echo "  Resultado"
echo "=========================================="
echo ""

if [ $ERRORS -eq 0 ]; then
  echo "Todas as verificacoes passaram!"
  echo ""
  echo "Resumo:"
  echo "- Database persistence: Configurado com 3 volumes"
  echo "- Usuario externo: Configurado (db_user)"
  echo "- Script de inicializacao: Pronto (init.sql)"
  echo "- Script de entrada: Pronto (entrypoint.sh)"
  echo "- Configuracao Docker: Validos para dev e producao"
  echo ""
  echo "Proximos passos:"
  echo "1. Copie .env.example para .env.dev e .env.prod"
  echo "2. Preencha as credenciais em cada arquivo .env"
  echo "3. Execute: ./scripts/build-full.sh --dev (desenvolvimento)"
  echo "4. Ou: ./scripts/build-full.sh (producao)"
else
  echo " $ERRORS erro(s) encontrado(s)"
  exit 1
fi
