#!/bin/bash
set -e

echo "Aguardando o SQL Server ficar pronto..."
until /opt/mssql-tools18/bin/sqlcmd -S sqlserver -U sa -P "${SA_PASSWORD}" -C -Q "SELECT 1" > /dev/null 2>&1; do
    echo "SQL Server ainda não está pronto..."
    sleep 5
done

echo "SQL Server está pronto!"

exec dotnet ProcessoSelecao.Api.dll
