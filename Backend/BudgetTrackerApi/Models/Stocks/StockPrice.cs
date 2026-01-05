// Fichier : MonProjetBlazor/Models/StockPrice.cs

namespace BudgetTrackerApi.Models // Votre namespace du projet
{
    public class StockPrice
    {
        public DateTime Date { get; set; }
        public double Price { get; set; }
        public double TotalValue { get; set; }
    }
}