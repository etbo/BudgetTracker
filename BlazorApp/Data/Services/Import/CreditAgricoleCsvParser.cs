using System.Text;
using BlazorApp.Data;

public class CreditAgricoleCsvParser : IBanqueCsvParser
{
    public string BankName => "Crédit agricole";

    public List<OperationCC> ParseCsv(TextReader reader)
    {
        var ListOperations = new List<OperationCC>();

        string? line;

        var headersCandidates = new[] { "Date", "Libell�", "D�bit euros", "Cr�dit euros", "" };

        int headerIndex = 0;

        while ((line = reader.ReadLine()) != null)
        {
            var cols = line.Split(';').Select(c => c.Trim().ToLower()).ToList();

            Console.WriteLine("cols = " + string.Join("/", cols));

            // Compare si la ligne contient les colonnes attendues
            if (headersCandidates.All(h => cols.Contains(h)) || headerIndex > 20)
                break;
            else
                headerIndex++;
        }

        Console.WriteLine($"headerIndex = {headerIndex}");

        if (headerIndex == -1)
            throw new Exception("Impossible de trouver l'entête du CSV");

        return ListOperations;
    }
}
