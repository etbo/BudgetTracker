using BlazorApp.Data;
using BlazorApp.Data.Helpers;
using BlazorApp.Data.Services.Import;
using CsvHelper;

public class GsheetsCsvParser : IBanqueParser
{
    public string BankName => "Extraction Google Sheets";
    
    public List<OperationCC> Parse(ParserInputContext ctx)
    {
        var ListOperations = new List<OperationCC>();
        var reader = ctx.GetTextReader();
        
        if (reader is null)
            return ListOperations;

        var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.GetCultureInfo("fr-FR"))
        {
            Delimiter = ";",
            IgnoreBlankLines = true,
        };

        Console.WriteLine("Création CsvReader");
        using var csv = new CsvReader(reader, config);

        Console.WriteLine("get records to do");
        var rows = csv.GetRecords<TransactionGsheetsCsv>().ToList();

        Console.WriteLine("get records done");

        // Instancie le hashContext qui mémorisera les Hash de cet import en particulier
        var hashContext = new ImportHashContext();


        foreach (var row in rows)
        {
            // Verfication that the parsing is not null to provide the right value to Date
            var DateIso = DateTimeHelper.ToIsoString(row.Date);

            if (string.IsNullOrWhiteSpace(DateIso))
                throw new FormatException("La colonne Date est vide dans le CSV.");

            var operation = new OperationCC
            {
                Date = DateIso,
                Description = row.Description,
                Montant = (double)row.Montant,
                Type = row.Type,
                Banque = row.Banque,
                Commentaire = row.Commentaires,
                DateImport = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
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
