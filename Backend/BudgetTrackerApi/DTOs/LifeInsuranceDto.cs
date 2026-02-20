namespace BudgetTrackerApi.DTOs
{
    public class LifeInsuranceSaisieDto
    {
        public int LineId { get; set; }
        public required string Label { get; set; }
        public bool IsScpi { get; set; }

        public DateTime? LastStatementDate { get; set; }
        public decimal LastUnitCount { get; set; }
        public decimal LastUnitValue { get; set; }
    }

    public class SaveStatementDto
    {
        public int LifeInsuranceLineId { get; set; }
        public DateTime Date { get; set; } // <--- L'erreur CS1061 venait d'ici
        public decimal UnitCount { get; set; }
        public decimal UnitValue { get; set; }

        // On ajoute string.Empty pour régler le warning de non-nullable
        public string Label { get; set; } = string.Empty;
        public bool IsScpi { get; set; }
    }

    public class GlobalSaveStatementDto
    {
        public int AccountId { get; set; }
        public DateTime Date { get; set; }
        public List<SaveStatementDto> Items { get; set; } = new();
    }

    public class UpdateGroupDateDto
    {
        public string GroupKey { get; set; } = string.Empty;
        public DateTime NewDate { get; set; }
    }
}