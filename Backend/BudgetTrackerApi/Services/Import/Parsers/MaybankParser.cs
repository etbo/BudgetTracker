using System.Globalization;
using BudgetTrackerApi.CsvMappings;
using BudgetTrackerApi.Data;
using BudgetTrackerApi.Data.Helpers;
using BudgetTrackerApi.Services.Import;
using BudgetTrackerApi.Models;
using CsvHelper;

public class MaybankCsvParser : IBanqueParser
{
    public string BankName => "Maybank";

    public List<CcOperation> Parse(ParserInputContext ctx)
    {
        var ListOperations = new List<CcOperation>();
        var reader = ctx.GetTextReader();

        if (reader is null)
            return ListOperations;

        var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            IgnoreBlankLines = true,
        };


        Console.WriteLine("Création CsvReader");
        using var csv = new CsvReader(reader, config);

        Console.WriteLine("get records to do");

        csv.Context.RegisterClassMap<TransactionMaybankCsvMap>();
        var rows = csv.GetRecords<TransactionMaybankCsv>().ToList();

        Console.WriteLine("get records done");

        // Instancie le hashContext qui mémorisera les Hash de cet import en particulier
        var hashContext = new ImportHashContext();

        var CurrentDateTime = DateTime.UtcNow;

        foreach (var row in rows)
        {
            if (!DateTime.TryParseExact(row.Date, "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime parsedDate))
            {
                throw new FormatException("Echec du parsing des dates");
            }

            // Calcul montant
            double MontantReel = 0;

            double TauxChangeMyrToEur = 0.208;

            if (row.Flow.Contains("Withdrawal"))
                MontantReel = (double)-row.Montant * TauxChangeMyrToEur;
            else
                MontantReel = (double)row.Montant * TauxChangeMyrToEur;


            var operation = new CcOperation
            {
                Date = parsedDate,
                Description = row.Description,
                Montant = MontantReel,
                Banque = "Maybank",
                DateImport = CurrentDateTime,
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
