namespace BudgetTrackerApp.Models
{
    public class OperationCC
    {
        public int Id { get; set; }
        public string Date { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double Montant { get; set; }
        public string? Categorie { get; set; }
        public string? Commentaire { get; set; }
        public string? Banque { get; set; }
        public string? DateImport { get; set; }
        public string? Hash { get; set; }
        public bool IsModified { get; set; } = false;
    }
}