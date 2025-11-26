namespace BudgetTrackerApp.Models
{
    public class OperationPea
    {
        public int Id { get; set; }
        public string? Titulaire { get; set; }
        public DateTime? Date { get; set; }
        public string Code { get; set; } = string.Empty;
        public int Quantité { get; set; }
        public double MontantBrutUnitaire { get; set; }
        public double MontantNet { get; set; }
    }
}