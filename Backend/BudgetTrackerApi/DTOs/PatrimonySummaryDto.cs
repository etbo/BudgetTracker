namespace BudgetTrackerApi.DTOs
{
    public class PatrimonySummaryDto
    {
        public decimal TotalGlobal => Cash + Savings + LifeInsurance + Pea;
        public decimal Cash { get; set; }
        public decimal Savings { get; set; }
        public decimal LifeInsurance { get; set; }
        public decimal Pea { get; set; }
        
        // Optionnel : Détail par compte si besoin plus tard
        public Dictionary<string, decimal> Details { get; set; } = new();
    }
}
