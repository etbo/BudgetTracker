using System.Text.Json.Serialization;

namespace BudgetTrackerApi.Models
{
    public class PeaOperation
    {
        public int Id { get; set; }
        public string? Owner { get; set; }
        public DateTime? Date { get; set; }
        public string Code { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double GrossUnitAmount { get; set; }
        public double NetAmount { get; set; }
    }
}