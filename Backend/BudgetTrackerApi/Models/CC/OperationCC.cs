using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTrackerApi.Models
{
    public class CcOperation
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public double Montant { get; set; }
        public string? Categorie { get; set; }
        public string? Comment { get; set; }
        public string? Banque { get; set; }
        public int? ImportLogId { get; set; }
        public string? Hash { get; set; }

        // Clé étrangère vers l'import
        public CcImportLog? ImportLog { get; set; }
    }
}