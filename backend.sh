#!/bin/bash

echo "========================================"
echo "  Build Backend - ProcessoSelecao"
echo "========================================"
echo ""

echo "[1/3] Parando containers existentes..."
podman compose down backend

echo ""
echo "[2/3] Building containers sem cache..."
podman compose build --no-cache backend

echo ""
echo "[3/3] Iniciando containers..."
podman compose up -d backend

echo "========================================"
echo "  Build concluido"
echo "========================================"
echo ""