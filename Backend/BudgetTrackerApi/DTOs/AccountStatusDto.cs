public class AccountStatusDto
{
    public string AccountName { get; set; }
    public string Type { get; set; } // "CC" ou "Livret"
    public DateTime? LastEntryDate { get; set; }
    public bool ActionRequired { get; set; }
    public string Message { get; set; }
}