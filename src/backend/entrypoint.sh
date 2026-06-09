#!/bin/bash
set -e

echo "==> Verificando diretórios runtime..."

# Garantir que diretórios de runtime existam (bind mount pode esconder os criados na imagem)
mkdir -p /app/documentos
chown -R appuser:appuser /app/documentos 2>/dev/null || chmod -R 777 /app/documentos

# Criar diretórios para cache e logs caso necessários
mkdir -p /app/logs
chown -R appuser:appuser /app/logs 2>/dev/null || chmod -R 777 /app/logs

echo "Aguardando o SQL Server ficar pronto..."
until /opt/mssql-tools18/bin/sqlcmd -S sqlserver -U sa -P "${SA_PASSWORD}" -C -Q "SELECT 1" > /dev/null 2>&1; do
    echo "SQL Server ainda não está pronto..."
    sleep 5
done

echo "SQL Server está pronto!"

exec dotnet ProcessoSelecao.Api.dll
