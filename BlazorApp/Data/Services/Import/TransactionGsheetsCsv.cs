public class TransactionGsheetsCsv
{
    public required  string Date { get; set; }
    public required  string Description { get; set; }

    // Decimal car cela caste mieux les espace entre milliers (le format fr en général)
    public decimal Montant { get; set; }
    public required  string Type { get; set; }
    public required  string Commentaires { get; set; }
    public required  string Catégorie { get; set; }
    public required string Banque { get; set; }
}
