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
EXPOSE 8080

RUN mkdir -p /app/documentos

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProcessoSelecao.Api.dll"]
