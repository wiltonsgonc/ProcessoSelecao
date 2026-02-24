#!/bin/bash

echo "=== Iniciando ambiente de desenvolvimento ==="

echo "1. Verificando Podman..."
if ! command -v podman &> /dev/null; then
    echo "ERRO: Podman não encontrado. Instale o Podman no WSL."
    exit 1
fi

echo "2. Criando rede..."
podman network create processo-selecao-network 2>/dev/null || true

echo "3. Criando volumes..."
podman volume create sqlserver_data 2>/dev/null || true
podman volume create documentos_data 2>/dev/null || true

echo "4. Iniciando SQL Server..."
podman run -d \
    --name processo-selecao-sqlserver \
    --network processo-selecao-network \
    -e ACCEPT_EULA=Y \
    -e "SA_PASSWORD=Processo@123" \
    -e MSSQL_PID=Developer \
    -p 1433:1433 \
    -v sqlserver_data:/var/opt/mssql/data \
    mcr.microsoft.com/mssql/server:2022-latest

echo "5. Aguardando SQL Server iniciar (30s)..."
sleep 30

echo "6. Construindo e iniciando Backend..."
cd src/backend
podman build -t processo-selecao-backend -f ProcessoSelecao.Api/Dockerfile .
podman run -d \
    --name processo-selecao-backend \
    --network processo-selecao-network \
    -e ASPNETCORE_ENVIRONMENT=Development \
    -e "ConnectionStrings__DefaultConnection=Server=processo-selecao-sqlserver;Database=ProcessoSelecaoDb;User Id=sa;Password=Processo@123;TrustServerCertificate=True;" \
    -p 5000:8080 \
    -v documentos_data:/app/documentos \
    processo-selecao-backend

cd ../..

echo "7. Construindo e iniciando Frontend..."
cd src/frontend
podman build -t processo-selecao-frontend --target development .
podman run -d \
    --name processo-selecao-frontend \
    --network processo-selecao-network \
    -p 4200:4200 \
    -v $(pwd):/app \
    processo-selecao-frontend

cd ../..

echo ""
echo "=== Ambiente iniciado com sucesso! ==="
echo "Frontend: http://localhost:4200"
echo "Backend:  http://localhost:5000"
echo "API Docs: http://localhost:5000/swagger"
echo ""
echo "Para parar os containers:"
echo "  podman stop processo-selecao-frontend processo-selecao-backend processo-selecao-sqlserver"
