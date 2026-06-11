FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia apenas os .csproj primeiro para aproveitar o cache do Docker.
COPY ["ProcessoSelecao.Api/ProcessoSelecao.Api.csproj", "ProcessoSelecao.Api/"]
COPY ["ProcessoSelecao.Application/ProcessoSelecao.Application.csproj", "ProcessoSelecao.Application/"]
COPY ["ProcessoSelecao.Infrastructure/ProcessoSelecao.Infrastructure.csproj", "ProcessoSelecao.Infrastructure/"]
COPY ["ProcessoSelecao.Domain/ProcessoSelecao.Domain.csproj", "ProcessoSelecao.Domain/"]

RUN dotnet restore "ProcessoSelecao.Api/ProcessoSelecao.Api.csproj"

COPY . .
WORKDIR "/src/ProcessoSelecao.Api"

# --no-restore: o restore ja foi feito acima, evita duplicacao
# /p:UseAppHost=false: nao gera executavel nativo (desnecessario em container Linux)
RUN dotnet publish "ProcessoSelecao.Api.csproj" \
      -c Release \
      -o /app/publish \
      --no-restore \
      /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 5002

# Criar usuario e diretorios ANTES dos COPYs
RUN groupadd --system --gid 1000 appuser && \
    useradd  --system --uid 1000 --gid 1000 appuser && \
    mkdir -p /app/documentos /app/logs

# Copiar arquivos publicados e entrypoint
COPY --from=build /app/publish .
COPY entrypoint.sh /entrypoint.sh

# CORRECAO: chown feito APOS todos os COPYs para garantir ownership correto
RUN chmod +x /entrypoint.sh && \
    chown -R appuser:appuser /app /entrypoint.sh

USER appuser

ENTRYPOINT ["/entrypoint.sh"]
