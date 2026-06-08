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

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 5002

RUN groupadd --system --gid 1000 appuser && \
    useradd --system --uid 1000 --gid 1000 appuser && \
    mkdir -p /app/documentos && \
    chown -R appuser:appuser /app

COPY --from=build /app/publish .

USER appuser

ENTRYPOINT ["dotnet", "ProcessoSelecao.Api.dll"]
