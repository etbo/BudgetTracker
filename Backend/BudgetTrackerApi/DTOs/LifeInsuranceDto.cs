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
        public DateTime Date { get; set; }
        public decimal UnitCount { get; set; }
        public decimal UnitValue { get; set; }
    }
}