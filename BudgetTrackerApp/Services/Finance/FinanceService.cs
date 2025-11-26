using BudgetTrackerApp.Data;
using BudgetTrackerApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
// N'oubliez pas d'importer votre namespace de modèles si nécessaire
// using BudgetTrackerApp.Data; // Exemple pour votre DbContext

public class FinanceService
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _dbContext; // Remplacer par votre DbContext réel
    private const string ApiKey = "R7X494CD5T90RSEE";
    private const int CacheDurationHours = 24; // Durée de validité du cache

    // Mise à jour du constructeur pour accepter le DbContext
    public FinanceService(HttpClient httpClient, AppDbContext dbContext) 
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
    }

    public async Task<List<StockPrice>> GetHistoricalPricesAsync(string ticker, double quantity, DateTime startDate)
    {
        // ----------------------------------------------------
        // ÉTAPE 1 : VÉRIFICATION DU CACHE
        // ----------------------------------------------------
        var lastCacheEntry = await _dbContext.CachedStockPrices
            .Where(c => c.Ticker == ticker)
            .OrderByDescending(c => c.CacheTimestamp)
            .FirstOrDefaultAsync();

        // Si des données existent ET qu'elles sont récentes (moins de 24h)
        if (lastCacheEntry != null && 
            (DateTime.Now - lastCacheEntry.CacheTimestamp).TotalHours < CacheDurationHours)
        {
            // Lire toutes les données du cache et retourner
            var cachedData = await _dbContext.CachedStockPrices
                .Where(c => c.Ticker == ticker && c.Date >= startDate.Date)
                .OrderBy(c => c.Date)
                .Select(c => new StockPrice
                {
                    Date = c.Date,
                    Price = (double)c.Price,
                    TotalValue = (double)c.Price * quantity
                })
                .ToListAsync();

            if (cachedData.Any())
            {
                // Cache HIT ! Retourne les données du cache et évite l'API
                return cachedData;
            }
        }
        
        // ----------------------------------------------------
        // ÉTAPE 2 : APPEL DE L'API (Cache MISS ou expiré)
        // ----------------------------------------------------
        
        // (Le code de l'appel API que nous avions précédemment)
        string apiUrl = $"https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY&symbol={ticker}&apikey={ApiKey}";
        var httpResponse = await _httpClient.GetAsync(apiUrl);
        // ... (votre gestion des erreurs HTTP 4xx/5xx) ...
        
        var jsonContent = await httpResponse.Content.ReadAsStringAsync();
        
        Console.WriteLine($"jsonContent = {jsonContent}");

        if (jsonContent.Contains("API rate limit"))
        {
            throw new Exception($"Nombre de requêtes maximum quotidiennes atteint");
        }
        // ... (votre gestion de l'erreur JSON "Information" ou "Error Message") ...
        
        var response = JsonSerializer.Deserialize<AlphaVantageMonthly>(jsonContent);

        
        if (response?.MonthlyTimeSeries == null)
        {
            throw new Exception($"Impossible de trouver les données pour le ticker {ticker}. Vérifiez le symbole ou l'API.");
        }
        
        // ----------------------------------------------------
        // ÉTAPE 3 : TRAITEMENT ET MISE À JOUR DU CACHE
        // ----------------------------------------------------

        // 3.1. Préparer les données pour le cache et l'affichage
        var newCacheEntries = new List<CachedStockPrice>();
        var finalData = new List<StockPrice>();
        var now = DateTime.Now;

        // Effacer l'ancien cache pour le ticker avant d'insérer les nouvelles données
        _dbContext.CachedStockPrices.RemoveRange(_dbContext.CachedStockPrices.Where(c => c.Ticker == ticker));

        foreach (var entry in response.MonthlyTimeSeries)
        {
            if (DateTime.TryParse(entry.Key, out DateTime date) && 
                double.TryParse(entry.Value.Close, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
            {
                if (date.Date >= startDate.Date)
                {
                    // 3.2. Stocker la donnée pour la BDD (cache)
                    newCacheEntries.Add(new CachedStockPrice
                    {
                        Ticker = ticker,
                        Date = date,
                        Price = (decimal)price, // Convertir en decimal pour la BDD
                        CacheTimestamp = now // Enregistrer l'heure de la mise en cache
                    });

                    // 3.3. Stocker la donnée pour le composant (affichage)
                    finalData.Add(new StockPrice
                    {
                        Date = date,
                        Price = price,
                        TotalValue = price * quantity
                    });
                }
            }
        }

        // 3.4. Sauvegarder dans la base de données
        await _dbContext.CachedStockPrices.AddRangeAsync(newCacheEntries);
        await _dbContext.SaveChangesAsync();

        return finalData.OrderBy(d => d.Date).ToList();
    }
}