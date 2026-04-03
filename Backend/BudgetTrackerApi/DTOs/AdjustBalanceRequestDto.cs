namespace BudgetTrackerApi.DTOs;

public class AdjustBalanceRequestDto
{
    public string Bank { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public double ActualBalance { get; set; }
}
