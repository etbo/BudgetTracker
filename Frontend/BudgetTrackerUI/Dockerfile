# 1. Compilation Angular
FROM node:20 AS build
WORKDIR /app

COPY package*.json ./
# Si npm ci pose problème en CI, npm install assure l'installation des deps
RUN npm install

COPY . ./
# Commande directe Angular
RUN npx ng build --configuration production

# 2. Serveur Nginx léger
FROM nginx:alpine
COPY --from=build /app/dist/budget-tracker-ui/browser /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]