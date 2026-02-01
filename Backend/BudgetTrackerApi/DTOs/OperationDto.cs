namespace BudgetTrackerApi.DTOs
{
    public class CcOperationDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public required string Label { get; set; }
        public string? Category { get; set; }
        // Ce champ n'existe qu'ici, pas en BDD !
        public string? MacroCategory { get; set; }
        public bool IsSuggested { get; set; }
        public string? Comment { get; set; }
        public string? Banque { get; set; }
    }
}