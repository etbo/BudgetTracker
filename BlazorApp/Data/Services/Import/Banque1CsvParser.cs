using BlazorApp.Data;

public class Banque1CsvParser : IBanqueCsvParser
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

            var values = line.Split(',');

            ListOperations.Add(new OperationCC
            {
                Date = values[0],
                Description = values[1] + " (B1)",
                Montant = double.Parse(values[2], CultureInfo.InvariantCulture),
                Type = values[3],
                Commentaire = "",
                Banque = "B1"
            });
        }

        return ListOperations;
    }
}
