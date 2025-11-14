using BlazorApp.Data;
using BlazorApp.Data.Helpers;
using BlazorApp.Data.Services.Import;
public class FortuneoCsvParser : IBanqueCsvParser
{
    public string BankName => "Fortuneo";

    public List<OperationCC> ParseCsv(TextReader reader)
    {
        var ListOperations = new List<OperationCC>();

        string? line;
        bool header = true;

        // Instancie le hashContext qui mémorisera les Hash de cet import en particulier
        var hashContext = new ImportHashContext();

        while ((line = reader.ReadLine()) != null)
        {
            if (header) { header = false; continue; }

            Console.WriteLine($"line = {line}");

            var values = line.Split(';');

            var operation = new OperationCC
            {
                Date = DateTimeHelper.ToIsoString(values[0]),
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

        

        Console.WriteLine("Fin du parsing");

        return ListOperations;
    }
}
