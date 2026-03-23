@echo off
setlocal enabledelayedexpansion

echo ========================================
echo   Build Full - ProcessoSelecao
echo ========================================
echo.

echo [1/3] Parando containers existentes...
podman compose down

echo.
echo [2/3] Building containers sem cache...
podman compose build --no-cache backend frontend

echo.
echo [3/3] Iniciando containers...
podman compose up -d

echo ========================================
echo   Build concluido
echo ========================================
echo.
