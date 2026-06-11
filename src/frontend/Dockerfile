FROM node:24-alpine AS development
WORKDIR /app

COPY package*.json ./
# CORRECAO: tenta npm ci primeiro; se falhar (memoria, cache), usa npm install como fallback.
# Comum no Docker Desktop WSL2 com recursos limitados.
RUN npm ci || npm install

COPY . .

# CORRECAO: porta 80 para bater com o mapeamento 4200:80 do docker-compose.dev.yml
EXPOSE 80

# CORRECAO: --host 0.0.0.0 e obrigatorio para o ng serve ser acessivel fora do container
# --port 80 bate com o EXPOSE acima e com o compose (4200:80)
# --poll 2000 necessario em volumes montados (WSL2, VMs) para hot-reload funcionar
CMD ["sh", "-c", "npm run start -- --host 0.0.0.0 --port 80 --poll 2000 --disable-host-check"]

FROM node:24-alpine AS build
WORKDIR /app

COPY package*.json ./
# CORRECAO: fallback para npm install se npm ci falhar (memoria limitada no WSL2)
RUN npm ci || npm install

COPY . .
RUN npm run build -- --configuration production

FROM nginx:alpine AS production
COPY --from=build /app/dist/processo-selecao-frontend/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
