using BudgetTrackerApi.Data.Helpers;
using BudgetTrackerApi.Services.Import;
using BudgetTrackerApi.Models;
using OfficeOpenXml;

public class RevolutParser : IBankParser
{
    public string BankName => "Revolut";

    public List<CcOperation> Parse(ParserInputContext ctx)
    {
        var listOperations = new List<CcOperation>();
        using var reader = ctx.GetTextReader(); // Utilise 'using' pour bien libérer le flux

        if (reader is null) return listOperations;

        string? line;
        bool isHeader = true;

        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (isHeader) { isHeader = false; continue; }

            // 1. Détecter le séparateur (Revolut FR utilise souvent ';')
            char separator = line.Contains(';') ? ';' : ',';

            // 2. Split et nettoyage des guillemets éventuels autour des valeurs
            var values = line.Split(separator)
                             .Select(v => v.Trim(' ', '"', '\t'))
                             .ToArray();

            // 3. Debug précis pour voir ce que le code "voit" vraiment
            if (values.Length < 4)
            {
                Console.WriteLine($"Ligne ignorée (trop courte) : {line}");
                continue;
            }

            if (values[7] != "EUR")
            {
                throw new FormatException($"Devise inconnue : '{values[7]}' dans la ligne : {values[4]}");
            }

            // Dans ton CSV exemple : Type(0), Produit(1), DateDébut(2), DateFin(3), Desc(4), Montant(5)
            string dateRaw = values[2];
            Console.WriteLine($"Tentative de parsing sur : '{dateRaw}'");

            // 4. Parsing de la date (Format 24h avec HH)
            if (!DateTime.TryParseExact(dateRaw, "yyyy-MM-dd HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedDate))
            {
                // Si ça échoue encore, on tente un parsing plus souple
                if (!DateTime.TryParse(dateRaw, out parsedDate))
                {
                    throw new FormatException($"Impossible de lire la date : '{dateRaw}' dans la ligne : {line}");
                }
            }

            var amountOperation = double.Parse(values[5], System.Globalization.CultureInfo.InvariantCulture);
            var amountFees = double.Parse(values[6], System.Globalization.CultureInfo.InvariantCulture);

            var operation = new CcOperation
            {
                Date = parsedDate,
                Description = values[4],
                Amount = amountOperation + amountFees,
                Bank = "Revolut",
                Comment = (amountFees > 0) ? $"dont frais = {amountFees}" : "",
            };

            listOperations.Add(operation);
        }

        return listOperations;
    }
}
