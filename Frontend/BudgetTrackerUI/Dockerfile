FROM node:20 AS build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . ./
RUN npm run build

FROM nginx:alpine
# Injecter la configuration proxy
COPY nginx.conf /etc/nginx/conf.d/default.conf
# Copier l'application Angular compilée
COPY --from=build /app/dist/app/browser/ /usr/share/nginx/html/

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]