#!/bin/bash
set -e

echo "==> Verificando configuracao do container..."

# Garantir que diretorios de runtime existam (bind mount pode esconder os criados na imagem)
mkdir -p /app/documentos /app/logs
chown -R appuser:appuser /app/documentos /app/logs 2>/dev/null || chmod -R 777 /app/documentos /app/logs

echo "==> Aguardando SQL Server ficar disponivel..."
RETRIES=30
until /opt/mssql-tools18/bin/sqlcmd -S sqlserver -U sa -P "${SA_PASSWORD}" -C -Q "SELECT 1" > /dev/null 2>&1; do
    RETRIES=$((RETRIES - 1))
    if [ $RETRIES -le 0 ]; then
        echo "ERRO: SQL Server nao ficou disponivel apos 30 tentativas"
        exit 1
    fi
    echo "SQL Server ainda nao esta pronto... (tentativa $((30 - RETRIES))/30)"
    sleep 2
done

echo "==> SQL Server esta pronto!"
echo "==> Iniciando aplicacao..."

exec dotnet ProcessoSelecao.Api.dll
