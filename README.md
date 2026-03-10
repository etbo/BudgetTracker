# BudgetTracker
ASP .NET 9 + Angular

## Installation & Configuration

### 1. Clé API (AlphaVantage)

Le projet utilise l'API AlphaVantage pour les cours de bourse. Pour des raisons de sécurité, la clé n'est pas incluse dans le code.

Pour configurer votre propre clé :
1. Obtenez une clé gratuite sur [AlphaVantage](https://www.alphavantage.co/support/#api-key).
2. Ouvrez un terminal dans le dossier `Backend/BudgetTrackerApi`.
3. Exécutez la commande suivante en remplaçant `VOTRE_CLE` :
   ```bash
   dotnet user-secrets set "AlphaVantage:ApiKey" "VOTRE_CLE"
   ```

### 2. Base de données
Le projet utilise SQLite. Les fichiers de base de données se trouvent dans le dossier `Database/` à la racine.
