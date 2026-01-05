namespace BudgetTrackerApi.DTOs
{
    // C'est tout ce dont vous avez besoin pour le Record !
    public record TickerPurchaseDate(string Ticker, DateTime? OldestDate);
}