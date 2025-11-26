// Fichier : MonProjetBlazor/Models/AlphaVantageModels.cs

using System.Text.Json.Serialization;

namespace BudgetTrackerApp.Models
{
    // Utilisé pour mapper le champ "Monthly Time Series" principal
    public class AlphaVantageMonthly
    {
        [JsonPropertyName("Monthly Time Series")]
        public Dictionary<string, AlphaVantageData> MonthlyTimeSeries { get; set; }
    }

    // Utilisé pour mapper les données de prix pour chaque date
    public class AlphaVantageData
    {
        [JsonPropertyName("4. close")]
        public string Close { get; set; } = "";
    }
}