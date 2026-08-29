# 1. Image SDK .NET 9 pour la compilation
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copie et restaure les dépendances
COPY *.csproj ./
RUN dotnet restore

# Copie le reste des fichiers et publie l'API
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# 2. Image Runtime .NET 9 pour l'exécution
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "BudgetTrackerApi.dll"]