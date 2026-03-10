using BudgetTrackerApi.Data;
using BudgetTrackerApi.Data.Helpers;
using BudgetTrackerApi.Services.Import;
using BudgetTrackerApi.Models;
public class FortuneoCsvParser : IBankParser
{
    public string BankName => "Fortuneo";

    public List<CcOperation> Parse(ParserInputContext ctx)
    {
        var ListOperations = new List<CcOperation>();
        var reader = ctx.GetTextReader();

        if (reader is null)
            return ListOperations;

        string? line;
        bool header = true;

        // Instancie le hashContext qui mémorisera les Hash de cet import en particulier
        var hashContext = new ImportHashContext();

        while ((line = reader.ReadLine()) != null)
        {
            if (header) { header = false; continue; }

            var values = line.Split(';');

            // Parsing de la date :
            if (!DateTime.TryParseExact(values[0], "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedDate))
            {
                throw new FormatException("Echec du parsing des dates");
            }

            var amountDebit = values[3].Replace(',', '.');
            var amountCredit = values[4].Replace(',', '.');

            var operation = new CcOperation
            {
                Date = parsedDate, 
                Description = values[2],
                Amount = double.Parse(string.IsNullOrWhiteSpace(amountDebit) ? "0" : amountDebit, System.Globalization.CultureInfo.InvariantCulture) 
                       + double.Parse(string.IsNullOrWhiteSpace(amountCredit) ? "0" : amountCredit, System.Globalization.CultureInfo.InvariantCulture),
                Bank = "Fortuneo",
                Comment = "",
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
