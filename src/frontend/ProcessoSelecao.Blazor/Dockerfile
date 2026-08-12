# ==============================================
# Frontend - Blazor Web App (Server-Side)
# Dockerfile — ambiente de desenvolvimento
# ==============================================

FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app

COPY ProcessoSelecao.Blazor.csproj .
RUN dotnet restore

COPY . .

EXPOSE 80

ENTRYPOINT ["dotnet", "run", "--urls", "http://+:80"]
