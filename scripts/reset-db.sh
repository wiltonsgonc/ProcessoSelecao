#!/bin/bash
set -euo pipefail

echo "========================================"
echo "  Reset Banco de Dados - ProcessoSelecao"
echo "========================================"
echo ""

source .env 2>/dev/null || true

SA_PASSWORD="${SA_PASSWORD:-Processo@123}"
SQLCMD="podman exec processo-selecao-sqlserver /opt/mssql-tools18/bin/sqlcmd"

echo "[1/5] Resetando ProcessosSelecao..."
$SQLCMD -S localhost -U sa -P "${SA_PASSWORD}" -C \
  -Q "USE ProcessoSelecaoDb; DELETE FROM ProcessosSelecao; DBCC CHECKIDENT ('ProcessosSelecao', RESEED, 0);"

echo "[2/5] Resetando Candidatos..."
$SQLCMD -S localhost -U sa -P "${SA_PASSWORD}" -C \
  -Q "USE ProcessoSelecaoDb; DELETE FROM Candidatos; DBCC CHECKIDENT ('Candidatos', RESEED, 0);"

echo "[3/5] Resetando Avaliadores..."
$SQLCMD -S localhost -U sa -P "${SA_PASSWORD}" -C \
  -Q "USE ProcessoSelecaoDb; DELETE FROM Avaliadores; DBCC CHECKIDENT ('Avaliadores', RESEED, 0);"

echo "[4/5] Resetando Documentos..."
$SQLCMD -S localhost -U sa -P "${SA_PASSWORD}" -C \
  -Q "USE ProcessoSelecaoDb; DELETE FROM Documentos; DBCC CHECKIDENT ('Documentos', RESEED, 0);"

echo "[5/5] Resetando Baremas..."
$SQLCMD -S localhost -U sa -P "${SA_PASSWORD}" -C \
  -Q "USE ProcessoSelecaoDb; DELETE FROM Baremas; DBCC CHECKIDENT ('Baremas', RESEED, 0);"

echo ""
echo "========================================"
echo "  Reset concluido com sucesso!"
echo "========================================"
