# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ProcessoSelecao.Api/ProcessoSelecao.Api.csproj", "ProcessoSelecao.Api/"]
COPY ["ProcessoSelecao.Application/ProcessoSelecao.Application.csproj", "ProcessoSelecao.Application/"]
COPY ["ProcessoSelecao.Infrastructure/ProcessoSelecao.Infrastructure.csproj", "ProcessoSelecao.Infrastructure/"]
COPY ["ProcessoSelecao.Domain/ProcessoSelecao.Domain.csproj", "ProcessoSelecao.Domain/"]

RUN dotnet restore "ProcessoSelecao.Api/ProcessoSelecao.Api.csproj"

COPY . .
WORKDIR "/src/ProcessoSelecao.Api"
RUN dotnet publish "ProcessoSelecao.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage (production)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

RUN mkdir -p /app/documentos

# Instalar netcat para garantir que o backend só tente acessar o banco de dados quando o SQL Server estiver realmente pronto
RUN apt-get update && apt-get install -y netcat-openbsd && rm -rf /var/lib/apt/lists/*

# Instalar ferramentas EF Core para migrations
RUN dotnet tool install --global dotnet-ef

COPY --from=build /app/publish .
ENV PATH="${PATH}:/root/.dotnet/tools"

# Script de entrypoint para inicialização do banco de dados
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]
