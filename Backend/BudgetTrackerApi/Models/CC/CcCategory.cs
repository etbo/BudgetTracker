namespace BudgetTrackerApi.Data
{
    public class CcCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? MacroCategory { get; set; } // Optionnel
    }
}