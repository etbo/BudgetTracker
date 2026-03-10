using BudgetTrackerApi.Data;
using BudgetTrackerApi.Models;
using Microsoft.EntityFrameworkCore;
using BudgetTrackerApi.DTOs;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BudgetTrackerApi.Services
{
    public class FinanceService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;
        private readonly string _apiKey;
        private const int CacheDurationHours = 24; // Durée de validité du cache

        // Mise à jour du constructeur pour accepter le DbContext et IConfiguration
        public FinanceService(HttpClient httpClient, AppDbContext context, IConfiguration config)
        {
            _httpClient = httpClient;
            _context = context;
            _apiKey = config["AlphaVantage:ApiKey"] ?? string.Empty;
        }

        public enum UpdateStatus
        {
            NotAttempted, // 0
            Success,      // 1
            Failed        // 2
        }

        public record UpdateStocksValuesResult(
            UpdateStatus Status,
            string Message
        );

        public async Task<List<TickerPurchaseDate>> GetTickerList()
        {
            return await _context.PeaOperations
                .GroupBy(o => o.Code)
                .Select(g => new TickerPurchaseDate(g.Key, g.Min(o => o.Date)))
                .ToListAsync();
        }

        public async Task<UpdateStocksValuesResult> UpdatePeaCachedStockPrice(string ticker, DateTime? startDate, bool simulateJson)
        {
            if (string.IsNullOrEmpty(ticker) || !startDate.HasValue)
                throw new Exception($"Ticker vide");

            var dateMinimale = await _context.PeaCachedStockPrices
                    .Where(o => o.Ticker == ticker)
                    .OrderBy(o => o.CacheTimestamp) // Trie par ordre croissant (du plus ancien au plus récent)
                    .FirstOrDefaultAsync();


            if (dateMinimale != null && DateTime.Now.Date == dateMinimale.CacheTimestamp.Date)
            {
                Console.WriteLine($"Pas de MàJ pour {ticker}");
                // There is a dateMinimale found and it is already today : no update needed
                return new UpdateStocksValuesResult(UpdateStatus.NotAttempted, string.Empty);
            }

            Console.WriteLine($"MàJ nécessaire pour {ticker}");


            string jsonContent = "{}";

            if (simulateJson)
            {
                string filePath = "../Database/PA_CW8_Monthly.json";
                jsonContent = await File.ReadAllTextAsync(filePath);
            }
            else
            {
                // Construction requête API
                string apiUrl = $"https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY&symbol={ticker}&apikey={_apiKey}";
                Console.WriteLine($"apiUrl = {apiUrl}");
                // Récupération des données via l'API
                var httpResponse = await _httpClient.GetAsync(apiUrl);
                jsonContent = await httpResponse.Content.ReadAsStringAsync();
            }
            // Pour essai : simulation retour Json
            // string filePath = "../Database/PA_CW8_Monthly.json";
            // jsonContent = await File.ReadAllTextAsync(filePath);




            if (string.IsNullOrEmpty(jsonContent))
                return new UpdateStocksValuesResult(UpdateStatus.Failed, $"Nombre de requêtes maximum quotidiennes atteint");

            if (jsonContent.Contains("Error Message"))
            {
                // On peut même essayer d'extraire le message exact si on veut être précis
                return new UpdateStocksValuesResult(UpdateStatus.Failed, "Symbole boursier invalide ou introuvable sur Alpha Vantage.");
            }

            if (jsonContent.Contains("API rate limit"))
            {
                return new UpdateStocksValuesResult(UpdateStatus.Failed, $"Nombre de requêtes maximum quotidiennes atteint");
            }

            var response = JsonSerializer.Deserialize<AlphaVantageMonthly>(jsonContent);

            if (response?.MonthlyTimeSeries == null)
            {
                return new UpdateStocksValuesResult(UpdateStatus.Failed, $"Impossible de trouver les données. Vérifiez le symbole ou l'API.");
            }

            // 3.1. Préparer les données pour le cache et l'affichage
            var newCacheEntries = new List<PeaCachedStockPrice>();
            var now = DateTime.Now;

            // Effacer l'ancien cache pour le ticker avant d'insérer les nouvelles données
            _context.PeaCachedStockPrices.RemoveRange(_context.PeaCachedStockPrices.Where(c => c.Ticker == ticker));

            foreach (var entry in response.MonthlyTimeSeries)
            {
                if (DateTime.TryParse(entry.Key, out DateTime date) &&
                    double.TryParse(entry.Value.Close, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
                {
                    if (date.Date >= startDate)
                    {
                        // 3.2. Stocker la donnée pour la BDD (cache)
                        newCacheEntries.Add(new PeaCachedStockPrice
                        {
                            Ticker = ticker,
                            Date = date,
                            Price = (decimal)price, // Convertir en decimal pour la BDD
                            CacheTimestamp = now // Enregistrer l'heure de la mise en cache
                        });
                    }
                }
            }

            // Sauvegarde dans la base de données
            await _context.PeaCachedStockPrices.AddRangeAsync(newCacheEntries);
            await _context.SaveChangesAsync();

            // retour info Succès
            return new UpdateStocksValuesResult(UpdateStatus.Success, $"Mise à jour effectuée");
        }

    }
}