# ==============================================
# Backend - ASP.NET Core 10
# Dockerfile — ambiente de desenvolvimento
# Executa dotnet watch com hot-reload
# ==============================================

FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /src

# Copia apenas os .csproj primeiro (cache do restore)
COPY ["ProcessoSelecao.Api/ProcessoSelecao.Api.csproj", "ProcessoSelecao.Api/"]
COPY ["ProcessoSelecao.Application/ProcessoSelecao.Application.csproj", "ProcessoSelecao.Application/"]
COPY ["ProcessoSelecao.Infrastructure/ProcessoSelecao.Infrastructure.csproj", "ProcessoSelecao.Infrastructure/"]
COPY ["ProcessoSelecao.Domain/ProcessoSelecao.Domain.csproj", "ProcessoSelecao.Domain/"]

RUN dotnet restore "ProcessoSelecao.Api/ProcessoSelecao.Api.csproj"

# dotnet-ef instalado antes do COPY . . para aproveitar cache
RUN dotnet tool install dotnet-ef --tool-path /usr/local/bin

COPY . .

WORKDIR /src/ProcessoSelecao.Api

EXPOSE 5002

CMD ["dotnet", "watch", "run", "--no-restore", "--urls", "http://0.0.0.0:5002"]
