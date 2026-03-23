#!/bin/bash
set -e

# Aguardar o SQL Server ficar pronto usando uma verificação TCP simples
echo "Aguardando o SQL Server ficar pronto..."
until nc -z sqlserver 1433; do
    echo "SQL Server ainda não está pronto..."
    sleep 5
    done

echo "SQL Server está pronto!"

# Executar migrations do banco de dados
if [ "$ASPNETCORE_ENVIRONMENT" = "Development" ]; then
    echo "Executando migrations do banco de dados..."
    dotnet ef database update --connection "Server=sqlserver;Database=${DB_NAME};User Id=${DB_USER};Password=${DB_PASSWORD};TrustServerCertificate=True;" || true
    echo "Migrations do banco de dados concluídas (ou já atualizadas)"
fi

# Iniciar a aplicação
exec dotnet ProcessoSelecao.Api.dll