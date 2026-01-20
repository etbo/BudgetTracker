public class GlobalHistoryDto
{
    public string Label { get; set; } = "";
    public DateTime Date { get; set; }
    public decimal Cash { get; set; }       // Comptes Courants
    public decimal Savings { get; set; }    // Livrets
    public decimal LifeInsurance { get; set; }
    public decimal Pea { get; set; }
}