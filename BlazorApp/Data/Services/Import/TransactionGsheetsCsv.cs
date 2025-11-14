public class TransactionGsheetsCsv
{
    public string xxDate { get; set; }
    public string Description { get; set; }

    // Decimal car cela caste mieux les espace entre milliers (le format fr en général)
    public decimal Montant { get; set; }
    public string Type { get; set; }
    public string Commentaires { get; set; }
    public string Catégorie { get; set; }
    public string Banque { get; set; }
}
