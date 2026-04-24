#!/bin/bash

echo "========================================"
echo "  Build Full - ProcessoSelecao"
echo "========================================"
echo ""

echo "[1/4] Parando containers existentes..."
podman compose down

echo ""
echo "[2/4] Removendo volumes do banco de dados..."
podman volume rm processo-selecao_dbdata 2>/dev/null

echo ""
echo "[3/4] Building containers sem cache..."
podman compose build --no-cache backend frontend

echo ""
echo "[4/4] Iniciando containers..."
podman compose up -d

echo "========================================"
echo "  Build concluido"
echo "========================================"
echo ""