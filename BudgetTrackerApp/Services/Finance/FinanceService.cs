using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using BudgetTrackerApp.Models;

public class FinanceService
{
    private readonly HttpClient _httpClient;
    private const string ApiKey = "K0HU3AMV8KITYWX6";
    
    public FinanceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ⭐ METHODE CLÉ : Récupère la série complète en UNE seule requête.
    public async Task<List<StockPrice>> GetHistoricalPricesAsync(string ticker, double quantity, DateTime startDate)
    {
        // Construction de l'URL pour la série mensuelle
        string apiUrl = $"https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY&symbol={ticker}&apikey={ApiKey}";

        Console.WriteLine($"Requête API : {apiUrl}");

        var httpResponse = await _httpClient.GetAsync(apiUrl);

        if (!httpResponse.IsSuccessStatusCode)
        {
            // Gérer les erreurs de connexion ou les statuts HTTP explicites
            throw new Exception($"Erreur HTTP de l'API ({httpResponse.StatusCode}). Le service est peut-être inaccessible.");
        }

        var jsonContent = await httpResponse.Content.ReadAsStringAsync();

        if (jsonContent.Contains("API rate limit"))
        {
            throw new Exception($"Limite de requêtes API Alpha Vantage atteinte.");
        }

        var response = System.Text.Json.JsonSerializer.Deserialize<AlphaVantageMonthly>(jsonContent);

        if (response?.MonthlyTimeSeries == null)
        {
             // Gérer une erreur d'API ou un ticker non trouvé
             throw new Exception("Impossible de récupérer les données historiques pour le ticker " + ticker);
        }

        var historicalData = new List<StockPrice>();

        // Boucle sur les données retournées par l'API
        foreach (var entry in response.MonthlyTimeSeries)
        {
            DateTime date;
            if (DateTime.TryParse(entry.Key, out date) && double.TryParse(entry.Value.Close, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
            {
                // On s'assure que la date est après ou égale à la date d'achat
                if (date.Date >= startDate.Date)
                {
                    historicalData.Add(new StockPrice
                    {
                        Date = date,
                        Price = price,
                        TotalValue = price * quantity
                    });
                }
            }
        }
        
        // Trier par ordre chronologique croissant
        return historicalData.OrderBy(d => d.Date).ToList();
    }
}