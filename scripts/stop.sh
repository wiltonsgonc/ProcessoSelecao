#!/bin/bash

echo "Parando containers..."
podman stop processo-selecao-frontend processo-selecao-backend processo-selecao-sqlserver 2>/dev/null

echo "Removendo containers..."
podman rm processo-selecao-frontend processo-selecao-backend processo-selecao-sqlserver 2>/dev/null

echo "Ambiente parado."
