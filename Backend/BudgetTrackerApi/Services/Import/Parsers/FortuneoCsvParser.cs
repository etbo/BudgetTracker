using BudgetTrackerApp.Data;
using BudgetTrackerApp.Data.Helpers;
using BudgetTrackerApp.Services.Import;
using BudgetTrackerApp.Models;
public class FortuneoCsvParser : IBanqueParser
{
    public string BankName => "Fortuneo";

    public List<OperationCC> Parse(ParserInputContext ctx)
    {
        var ListOperations = new List<OperationCC>();
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

            // Verfication that the parsing is not null to provide the right value to Date
            var DateIso = DateTimeHelper.ToIsoString(values[0]);

            if (string.IsNullOrWhiteSpace(DateIso))
                throw new FormatException("La colonne Date est vide dans le CSV.");

            var operation = new OperationCC
            {
                Date = DateIso, // Sur que ce n'est pas null
                Description = values[2],
                Montant = double.Parse(string.IsNullOrWhiteSpace(values[3]) ? "0" : values[3]) + double.Parse(string.IsNullOrWhiteSpace(values[4]) ? "0" : values[4]),
                Banque = "Fortuneo",
                Commentaire = "",
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
