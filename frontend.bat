@echo off
setlocal enabledelayedexpansion

echo ========================================
echo   Build Frontend - ProcessoSelecao
echo ========================================
echo.

echo [1/3] Parando containers existentes...
podman compose down frontend

echo.
echo [2/3] Building frontend sem cache...
podman compose build --no-cache frontend

echo.
echo [3/3] Iniciando containers...
podman compose up -d frontend

echo ========================================
echo   Build concluido
echo ========================================
echo.
