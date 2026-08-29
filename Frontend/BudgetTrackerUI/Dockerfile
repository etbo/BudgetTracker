# 1. Image de compilation Node.js
FROM node:20 AS build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . ./
RUN npm run build

# 2. Serveur Nginx léger
FROM nginx:alpine

# Copie le contenu généré par le build d'Angular vers Nginx
# Note le slash '/' à la fin du dossier de destination : /usr/share/nginx/html/
COPY --from=build /app/dist/budget-tracker-ui/browser/ /usr/share/nginx/html/

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]