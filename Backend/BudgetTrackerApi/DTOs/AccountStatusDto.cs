public class AccountStatusDto
{
    public required string AccountName { get; set; }
    public required string Type { get; set; } // "CC" ou "Livret"
    public DateTime? LastEntryDate { get; set; }
    public bool ActionRequired { get; set; }
    public required string Message { get; set; }
}