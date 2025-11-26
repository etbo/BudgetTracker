namespace BudgetTrackerApp.Models
{
    public class TransactionMaybankCsv
    {
        public required string Date { get; set; }
        public required string TransactionType { get; set; }
        public required string Description { get; set; }

        // Decimal car cela caste mieux les espace entre milliers (le format fr en général)
        public decimal Montant { get; set; }
        public decimal? StatementBalance { get; set; }
        public string Flow { get; set; } = "";

    }
}
