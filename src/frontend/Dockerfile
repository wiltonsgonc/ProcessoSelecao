FROM node:24-alpine AS development
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .

EXPOSE 4200
CMD ["sh", "-c", "npm run start -- --poll 2000"]

FROM node:24-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build -- --configuration production

FROM nginx:alpine AS production
COPY --from=build /app/dist/processo-selecao-frontend/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
