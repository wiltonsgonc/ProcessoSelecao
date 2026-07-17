#!/bin/bash
set -e

echo "==> Verificando configuracao do container..."

mkdir -p /app/documentos /app/logs
chmod -R 777 /app/documentos /app/logs 2>/dev/null || true

echo "==> Aguardando SQL Server ficar disponivel..."
RETRIES=30
until bash -c "echo > /dev/tcp/sqlserver/1433" 2>/dev/null; do
    RETRIES=$((RETRIES - 1))
    if [ "$RETRIES" -le 0 ]; then
        echo "ERRO: SQL Server nao ficou disponivel apos 30 tentativas"
        exit 1
    fi
    echo "SQL Server ainda nao esta pronto... (tentativa $((30 - RETRIES))/30)"
    sleep 2
done

echo "==> SQL Server esta pronto!"
echo "==> Restaurando pacotes..."
dotnet restore

echo "==> Iniciando aplicacao..."
exec dotnet watch run --no-restore --urls "http://0.0.0.0:5002"
