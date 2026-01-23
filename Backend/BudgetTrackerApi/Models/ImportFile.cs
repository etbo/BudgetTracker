namespace BudgetTrackerApi.Models
{
    public class CcImportLog
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public DateTime ImportDate { get; set; }
        public bool IsSuccessful { get; set; }
        public string MsgErreur { get; set; } = string.Empty;
        public string? BankName { get; set; }
        public int TotalRows { get; set; }
        public int InsertedRows { get; set; }

        // Dates de la période couverte par le fichier
        public DateTime? DateMin { get; set; }
        public DateTime? DateMax { get; set; }
        public double TempsDeTraitementMs { get; set; }

        // Relation : Un import a plusieurs opérations
        public ICollection<CcOperation> Operations { get; set; } = new List<CcOperation>();
    }
}