# 1. Compilation Angular
FROM node:20 AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . ./
RUN npm run build -- --configuration production

# 2. Serveur Nginx léger
FROM nginx:alpine
# Copie des fichiers statiques générés vers le dossier de Nginx
COPY --from=build /app/dist/budget-tracker-ui/browser /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]