using System.Globalization;
using BudgetTrackerApp.Data;
using BudgetTrackerApp.Data.Helpers;
using BudgetTrackerApp.Services.Import;
using BudgetTrackerApp.Models;
using CsvHelper;
using SQLitePCL;

public class GsheetsCsvParser : IBanqueParser
{
    public string BankName => "Extraction Google Sheets";
    
    public List<CcOperation> Parse(ParserInputContext ctx)
    {
        var ListOperations = new List<CcOperation>();
        var reader = ctx.GetTextReader();

        
        if (reader is null)
            return ListOperations;

        var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.GetCultureInfo("fr-FR"))
        {
            Delimiter = ";",
            IgnoreBlankLines = true,
        };

        using var csv = new CsvReader(reader, config);

        Console.WriteLine($"Breakpoint");
        var rows = csv.GetRecords<TransactionGsheetsCsv>().ToList();
        Console.WriteLine($"Breakpoint2");

        // Instancie le hashContext qui mémorisera les Hash de cet import en particulier
        var hashContext = new ImportHashContext();

        Console.WriteLine($"test");

        foreach (var row in rows)
        {
            if (!DateTime.TryParseExact(row.Date, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime parsedDate))
            {
                throw new FormatException("Echec du parsing des dates");
            }
            var operation = new CcOperation
            {
                Date = parsedDate,
                Description = row.Description,
                Montant = (double)row.Montant,
                Categorie = row.Type,
                Banque = row.Banque,
                Comment = row.Commentaires,
                DateImport = DateTime.UtcNow,
            };

            // Récupération du Hash de base pour cette ligne
            var baseHash = operation.GenerateBaseHash();

            // Parcours de tous les Hash de cet import pour ajouter un #2 si déjà existant
            operation.Hash = hashContext.GetUniqueHash(baseHash);

            // Ajout de l'operation à la liste
            ListOperations.Add(operation);
        }

        return ListOperations;
    }
}
