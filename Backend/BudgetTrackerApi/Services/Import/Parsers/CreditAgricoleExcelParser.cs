using BudgetTrackerApi.Data.Helpers;
using BudgetTrackerApi.Services.Import;
using BudgetTrackerApi.Models;
using OfficeOpenXml;

public class CreditAgricoleExcelParser : IBanqueParser
{
    public string BankName => "Crédit agricole";

    public List<CcOperation> Parse(ParserInputContext ctx)
    {
        if (ctx.FileStream == null)
            throw new ArgumentException("Le flux du fichier Excel est null.");

        var ListOperations = new List<CcOperation>();

        // Pour un usage non commercial personnel
        ExcelPackage.License.SetNonCommercialPersonal("TonNom");

        using var package = new ExcelPackage(ctx.FileStream);
        var workbook = package.Workbook;

        if (workbook.Worksheets.Count == 0)
            return ListOperations; // Excel vide

        var sheet = workbook.Worksheets[0]; // première feuille

        var i = 1;

        // Détection des lignes à extraire
        while (true)
        {
            if (sheet.Cells[i, 1].Text == "Date")
            {
                Console.WriteLine($"Début des données trouvé (en-tête ligne{i})");
                break;
            }

            i++;
        }

        // Instancie le hashContext qui mémorisera les Hash de cet import en particulier
        var hashContext = new ImportHashContext();

        while (true)
        {
            i++;

            // Console.WriteLine($"2e boucle sheet.Cells[{i}, 1].IsEmpty() = {sheet.Cells[i, 1].IsEmpty()}");
            if (sheet.Cells[i, 1].IsEmpty())
                break;
            else
            {
                // Verfication that the parsing is not null to provide the right value to Date
                if (!DateTime.TryParseExact(sheet.Cells[i, 1].Text, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime parsedDate))
                {
                    throw new FormatException("Echec du parsing des dates");
                }

                var operation = new CcOperation
                {
                    Date = parsedDate,
                    Description = sheet.Cells[i, 2].Text,
                    Montant = double.Parse(string.IsNullOrWhiteSpace(sheet.Cells[i, 4].Text) ? "0" : sheet.Cells[i, 4].Text) - double.Parse(string.IsNullOrWhiteSpace(sheet.Cells[i, 3].Text) ? "0" : sheet.Cells[i, 3].Text),
                    Banque = "CA",
                    Comment = "",
                };

                // Récupération du Hash de base pour cette ligne
                var baseHash = operation.GenerateBaseHash();

                // Parcours de tous les Hash de cet import pour ajouter un #2 si déjà existant
                operation.Hash = hashContext.GetUniqueHash(baseHash);

                // Ajout de l'operation à la liste
                ListOperations.Add(operation);
            }

        }

        return ListOperations;
    }
}
