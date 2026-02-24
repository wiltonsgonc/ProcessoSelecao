@echo off
echo === Iniciando ambiente com Podman Compose ===

REM Verifica se o podman-compose está disponível
where podman-compose >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo ERRO: podman-compose nao encontrado.
    echo Instale com: pip install podman-compose
    exit /b 1
)

podman-compose up -d

echo.
echo === Ambiente iniciado! ===
echo Frontend: http://localhost:4200
echo Backend:  http://localhost:5000
echo API Docs: http://localhost:5000/swagger
echo.
echo Para parar: podman-compose down
