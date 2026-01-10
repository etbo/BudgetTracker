namespace BudgetTrackerApi.Models
{
    public class CcCategoryRule
    {
        public int Id { get; set; }
        public bool IsUsed { get; set; }
        public string? Pattern { get; set; }
        public double? MinAmount { get; set; }
        public double? MaxAmount { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public string? Category { get; set; } = "";
        public string? Comment { get; set; } = "";
    }
}