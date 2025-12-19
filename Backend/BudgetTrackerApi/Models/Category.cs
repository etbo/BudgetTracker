namespace BudgetTrackerApp.Data
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Type { get; set; } // Optionnel
    }
}