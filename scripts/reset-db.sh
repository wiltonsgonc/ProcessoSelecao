#!/bin/bash
set -euo pipefail

echo "========================================"
echo "  Reset Banco de Dados - ProcessoSelecao"
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
# Tenta na ordem: .env > .env.dev > .env.prod
if [ -f ".env" ]; then
  ENV_FILE=".env"
elif [ -f ".env.dev" ]; then
  ENV_FILE=".env.dev"
elif [ -f ".env.prod" ]; then
  ENV_FILE=".env.prod"
else
  echo "Erro: Nenhum arquivo .env encontrado."
  echo "Crie um arquivo .env a partir do .env.example:"
  echo "  cp .env.example .env"
  exit 1
fi

echo "Usando: $ENV_FILE"

# Exportar variaveis do arquivo env (ignora comentarios e linhas vazias)
set -a
# shellcheck disable=SC1090
source <(grep -E '^[A-Z_]+=.' "$ENV_FILE" | sed 's/#.*//')
set +a

if [ -z "${SA_PASSWORD:-}" ]; then
  echo "Erro: SA_PASSWORD nao esta definido em $ENV_FILE"
  exit 1
fi

SQLCMD="$RUNTIME exec processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd"
DB="${DB_NAME:-ProcessoSelecaoDb}"

# Verificar se o container esta rodando
if ! $RUNTIME ps --format '{{.Names}}' 2>/dev/null | grep -q "processo-selecao-sqlserver" && \
   ! $RUNTIME ps --format '{{.Name}}' 2>/dev/null | grep -q "processo-selecao-sqlserver"; then
  echo "Erro: Container 'processo-selecao-sqlserver' nao esta rodando."
  echo "Inicie os containers primeiro: ./scripts/build-full.sh [--dev]"
  exit 1
fi

echo ""
echo "ATENCAO: Esta operacao vai apagar TODOS os dados do banco '$DB'."
read -rp "Confirma? (digite 'sim' para continuar): " CONFIRM
if [[ "$CONFIRM" != "sim" ]]; then
  echo "Operacao cancelada."
  exit 0
fi

echo ""

# CORRECAO: Ordem de delecao respeitando Foreign Keys.
# Tabelas filhas devem ser deletadas antes das tabelas pai.
# Ajuste esta ordem conforme o schema real da aplicacao.

run_sql() {
  local step="$1"
  local query="$2"
  echo "$step"
  $SQLCMD -S localhost -U sa -P "${SA_PASSWORD}" -C -Q "$query"
}

# Desabilitar FK checks temporariamente para simplificar o reset
run_sql "[0/6] Desabilitando constraints..." \
  "USE ${DB}; EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';"

run_sql "[1/6] Deletando Documentos..." \
  "USE ${DB}; DELETE FROM Documentos; DBCC CHECKIDENT ('Documentos', RESEED, 0);"

run_sql "[2/6] Deletando Baremas..." \
  "USE ${DB}; DELETE FROM Baremas; DBCC CHECKIDENT ('Baremas', RESEED, 0);"

run_sql "[3/6] Deletando ProcessosSelecao..." \
  "USE ${DB}; DELETE FROM ProcessosSelecao; DBCC CHECKIDENT ('ProcessosSelecao', RESEED, 0);"

run_sql "[4/6] Deletando Candidatos..." \
  "USE ${DB}; DELETE FROM Candidatos; DBCC CHECKIDENT ('Candidatos', RESEED, 0);"

run_sql "[5/6] Deletando Avaliadores..." \
  "USE ${DB}; DELETE FROM Avaliadores; DBCC CHECKIDENT ('Avaliadores', RESEED, 0);"

run_sql "[6/6] Reabilitando constraints..." \
  "USE ${DB}; EXEC sp_msforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL';"

echo ""
echo "========================================"
echo "  Reset concluido com sucesso!"
echo "========================================"
