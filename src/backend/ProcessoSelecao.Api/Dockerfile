# ==============================================
# Backend - ASP.NET Core 10
# Dockerfile — ambiente de desenvolvimento
# Executa dotnet watch com hot-reload
# ==============================================

FROM mcr.microsoft.com/dotnet/sdk:10.0

# Instala curl para healthcheck do compose
RUN apt-get update && apt-get install -y curl --no-install-recommends && rm -rf /var/lib/apt/lists/*

WORKDIR /src

# Copia apenas os .csproj primeiro (cache do restore)
COPY ["ProcessoSelecao.Api/ProcessoSelecao.Api.csproj", "ProcessoSelecao.Api/"]
COPY ["ProcessoSelecao.Application/ProcessoSelecao.Application.csproj", "ProcessoSelecao.Application/"]
COPY ["ProcessoSelecao.Infrastructure/ProcessoSelecao.Infrastructure.csproj", "ProcessoSelecao.Infrastructure/"]
COPY ["ProcessoSelecao.Domain/ProcessoSelecao.Domain.csproj", "ProcessoSelecao.Domain/"]

RUN dotnet restore "ProcessoSelecao.Api/ProcessoSelecao.Api.csproj"

COPY . .

# Copia entrypoint para fora do /src (volume mount substitui /src)
COPY entrypoint.sh /usr/local/bin/entrypoint.sh
RUN chmod +x /usr/local/bin/entrypoint.sh

WORKDIR /src/ProcessoSelecao.Api

EXPOSE 5002

ENTRYPOINT ["/usr/local/bin/entrypoint.sh"]
