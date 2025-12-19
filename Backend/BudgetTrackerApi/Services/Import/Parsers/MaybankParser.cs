using System.Globalization;
using BudgetTrackerApp.CsvMappings;
using BudgetTrackerApp.Data;
using BudgetTrackerApp.Data.Helpers;
using BudgetTrackerApp.Services.Import;
using BudgetTrackerApp.Models;
using CsvHelper;

public class MaybankCsvParser : IBanqueParser
{
    public string BankName => "Maybank";

    public List<OperationCC> Parse(ParserInputContext ctx)
    {
        var ListOperations = new List<OperationCC>();
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


        foreach (var row in rows)
        {
            // Verfication that the parsing is not null to provide the right value to Date
            var DateIso = DateTimeHelper.ToIsoString(row.Date);

            if (string.IsNullOrWhiteSpace(DateIso))
                throw new FormatException("La colonne Date est vide dans le CSV.");

            // Calcul montant
            double MontantReel = 0;

            double TauxChangeMyrToEur = 0.208;
            
            if (row.Flow.Contains("Withdrawal"))
                MontantReel = (double) -row.Montant * TauxChangeMyrToEur;
            else
                MontantReel = (double) row.Montant * TauxChangeMyrToEur;
            

            var operation = new OperationCC
            {
                Date = DateIso,
                Description = row.Description,
                Montant = MontantReel,
                Banque = "Maybank",
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
