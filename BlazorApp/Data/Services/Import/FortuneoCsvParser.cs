using BlazorApp.Data;

public class FortuneoCsvParser : IBanqueCsvParser
{
    public List<OperationCC> ParseCsv(Stream csvStream)
    {
        var ListOperations = new List<OperationCC>();

        using var reader = new StreamReader(csvStream);
        string? line;
        bool header = true;

        while ((line = reader.ReadLine()) != null)
        {
            if (header) { header = false; continue; }

            var values = line.Split(';');

            ListOperations.Add(new OperationCC
            {
                Date = values[0],
                Description = values[2],
                Montant = double.Parse(string.IsNullOrWhiteSpace(values[3]) ? "0" : values[3]) + double.Parse(string.IsNullOrWhiteSpace(values[4]) ? "0" : values[4]),
                Banque = "Fortuneo"
            });
        }

        Console.WriteLine("Fin du parsing");

        return ListOperations;
    }
}
