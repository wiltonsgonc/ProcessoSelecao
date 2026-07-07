#!/bin/bash
set -euo pipefail

echo "========================================"
echo "  Configurar Podman Rootless"
echo "  (Alma Linux / RHEL / CentOS / Debian / Ubuntu / WSL2)"
echo "========================================"
echo ""

if [ "$(id -u)" -eq 0 ]; then
  echo "ERRO: Nao execute como root. Execute como o usuario do Podman."
  exit 1
fi

if ! command -v podman &>/dev/null; then
  echo "ERRO: Podman nao instalado."
  echo "Instale com um dos comandos abaixo:"
  echo "  Alma/RHEL/CentOS: sudo dnf install -y podman podman-compose"
  echo "  Debian/Ubuntu:    sudo apt install -y podman podman-compose"
  exit 1
fi

UID_NUM=$(id -u)
RUNTIME_DIR="/run/user/${UID_NUM}"
CUSER=$(whoami)
SHELL_RC="${HOME}/.bashrc"
# Usa .bash_profile se for login shell sem .bashrc (comum em RHEL/Alma)
[ -f "${HOME}/.bash_profile" ] && SHELL_RC="${HOME}/.bash_profile"

# ==============================================
# [1/9] containers.conf — desabilita aardvark-dns
# que depende de D-Bus para funcionar.
# Sem isso: "aardvark-dns failed to start: Failed to connect to bus"
# ==============================================
echo "[1/9] Criando containers.conf (desabilita aardvark-dns)..."
mkdir -p ~/.config/containers
cat > ~/.config/containers/containers.conf << 'EOF'
[engine]
cgroup_manager = "cgroupfs"
events_logger = "file"

[network]
# Desabilitar aardvark-dns:
# O aardvark-dns exige D-Bus do usuario (systemd --user) para iniciar.
# Em sessoes SSH sem PAM completo ou sem lingering ativo,
# o socket /run/user/<uid>/bus nao existe e o container falha ao subir.
# Com dns_bind_port = 0 o Podman usa resolucao de rede sem aardvark.
dns_bind_port = 0
EOF
echo "  OK: ~/.config/containers/containers.conf criado"

# ==============================================
# [2/9] crun.conf
# ==============================================
echo ""
echo "[2/9] Criando crun.conf (runtime OCI)..."
mkdir -p ~/.config/crun
cat > ~/.config/crun/crun.conf << 'EOF'
cgroup:
  manager: "cgroupfs"
EOF
echo "  OK: ~/.config/crun/crun.conf criado"

# ==============================================
# [3/9] subuid / subgid
# ==============================================
echo ""
echo "[3/9] Verificando subuid/subgid..."
if ! grep -q "^${CUSER}:" /etc/subuid 2>/dev/null; then
  echo "  Configurando subuid/subgid para $CUSER..."
  sudo usermod --add-subuids 100000-165535 --add-subgids 100000-165535 "$CUSER"
  echo "  OK"
else
  echo "  OK: subuid/subgid ja configurados para $CUSER"
fi

# ==============================================
# [4/9] Lingering — mantem sessao do usuario ativa
# mesmo sem login ativo. Necessario para que
# systemd --user e o D-Bus persistam.
# ==============================================
echo ""
echo "[4/9] Habilitando lingering..."
sudo loginctl enable-linger "$CUSER" 2>/dev/null || true
echo "  OK: loginctl enable-linger $CUSER"

# ==============================================
# [5/9] XDG_RUNTIME_DIR — cria e garante permissao
# O Podman rootless EXIGE esta variavel para
# localizar sockets (D-Bus, Podman API, etc).
# Sem ela: "no such file or directory" nos sockets.
# ==============================================
echo ""
echo "[5/9] Configurando XDG_RUNTIME_DIR (${RUNTIME_DIR})..."
sudo mkdir -p "${RUNTIME_DIR}"
sudo chown "${CUSER}:${CUSER}" "${RUNTIME_DIR}"
chmod 700 "${RUNTIME_DIR}"
echo "  OK: ${RUNTIME_DIR} criado com permissao 700"

# Exporta para a sessao atual
export XDG_RUNTIME_DIR="${RUNTIME_DIR}"
export DBUS_SESSION_BUS_ADDRESS="unix:path=${RUNTIME_DIR}/bus"
echo "  OK: variaveis exportadas para sessao atual"

# ==============================================
# [6/9] Persiste as variaveis no shell do usuario
# ==============================================
echo ""
echo "[6/9] Persistindo variaveis no ${SHELL_RC}..."

EXPORT_BLOCK="
# --- Podman rootless (adicionado por setup-podman-rootless.sh) ---
export XDG_RUNTIME_DIR=/run/user/\$(id -u)
export DBUS_SESSION_BUS_ADDRESS=unix:path=/run/user/\$(id -u)/bus
# ----------------------------------------------------------------"

if ! grep -q "Podman rootless" "${SHELL_RC}" 2>/dev/null; then
  echo "${EXPORT_BLOCK}" >> "${SHELL_RC}"
  echo "  OK: variaveis adicionadas em ${SHELL_RC}"
else
  echo "  OK: variaveis ja presentes em ${SHELL_RC}"
fi

# ==============================================
# [7/9] Reinicia logind para propagar lingering
# Necessario no Alma/RHEL para o runtime dir
# ser criado automaticamente pelo PAM na proxima sessao.
# ==============================================
echo ""
echo "[7/9] Reiniciando systemd-logind para propagar lingering..."
if sudo systemctl restart systemd-logind 2>/dev/null; then
  echo "  OK"
  sleep 2
else
  echo "  AVISO: systemd-logind nao reiniciou (pode ser normal em Debian/Ubuntu/WSL2)"
fi

# ==============================================
# [8/9] Migra storage do Podman
# ==============================================
echo ""
echo "[8/9] Migrando Podman (storage e rede)..."
podman system migrate 2>/dev/null || true
echo "  OK"

# ==============================================
# [9/9] podman-compose
# ==============================================
echo ""
echo "[9/9] Verificando podman-compose..."
if command -v podman-compose &>/dev/null; then
  CURRENT_VER=$(podman-compose version 2>/dev/null | grep -oP '\d+\.\d+' | head -1 || echo "0.0")
  echo "  Versao instalada: $CURRENT_VER"
  MAJOR=$(echo "$CURRENT_VER" | cut -d. -f1)
  MINOR=$(echo "$CURRENT_VER" | cut -d. -f2)
  if [[ "$MAJOR" -lt 1 ]] || { [[ "$MAJOR" -eq 1 ]] && [[ "$MINOR" -lt 2 ]]; }; then
    echo "  AVISO: versao $CURRENT_VER nao suporta 'condition: service_healthy'."
    echo "  O build-full.sh vai compensar automaticamente."
    echo "  Para atualizar: pip3 install --upgrade podman-compose"
  fi
else
  if command -v pip3 &>/dev/null; then
    echo "  Instalando podman-compose via pip3..."
    pip3 install --user podman-compose
    export PATH="${HOME}/.local/bin:${PATH}"
    echo "  OK"
  else
    echo "  AVISO: pip3 nao encontrado."
    echo "  sudo dnf install -y python3-pip && pip3 install --user podman-compose"
  fi
fi

# ==============================================
# Verificacao final
# ==============================================
echo ""
echo "========================================"
echo "  Verificando configuracao..."
echo "========================================"

CGROUP_MGR=$(podman info --format '{{.Host.CgroupManager}}' 2>/dev/null || echo "desconhecido")
OCI_RT=$(podman info --format '{{.Host.OCIRuntime.Name}}' 2>/dev/null || echo "desconhecido")
echo "Cgroup Manager : $CGROUP_MGR"
echo "OCI Runtime    : $OCI_RT"
echo "XDG_RUNTIME_DIR: ${XDG_RUNTIME_DIR}"

echo ""
echo "========================================"
echo "  Testando Podman..."
echo "========================================"

if podman run --rm docker.io/library/alpine echo "Podman rootless OK!" 2>/dev/null; then
  echo ""
  echo "========================================"
  echo "  Configuracao concluida com sucesso!"
  echo "========================================"
  echo ""
  echo "IMPORTANTE: Abra uma nova sessao SSH (ou execute 'source ${SHELL_RC}')"
  echo "para carregar as variaveis de ambiente persistidas."
  echo ""
  echo "Proximo passo:"
  echo "  ./scripts/build-full.sh --dev"
else
  echo ""
  echo "========================================"
  echo "  AVISO: Teste falhou."
  echo "========================================"
  echo ""
  echo "Tente numa nova sessao SSH e re-execute o teste:"
  echo "  podman run --rm alpine echo ok"
  echo ""
  echo "Se persistir, verifique:"
  echo "  podman info 2>&1 | grep -i error"
  echo "  journalctl --user -n 50"
fi
