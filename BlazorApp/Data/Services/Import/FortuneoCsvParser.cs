using BlazorApp.Data;
using BlazorApp.Data.Services.Import;

public class FortuneoCsvParser : IBanqueCsvParser
{
    public List<OperationCC> ParseCsv(Stream csvStream)
    {
        var ListOperations = new List<OperationCC>();

        using var reader = new StreamReader(csvStream);
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
                Date = values[0],
                Description = values[2],
                Montant = double.Parse(string.IsNullOrWhiteSpace(values[3]) ? "0" : values[3]) + double.Parse(string.IsNullOrWhiteSpace(values[4]) ? "0" : values[4]),
                Banque = "Fortuneo",
                Commentaire = "Hash",
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
