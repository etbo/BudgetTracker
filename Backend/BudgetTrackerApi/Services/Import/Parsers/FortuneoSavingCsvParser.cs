using BudgetTrackerApi.Data;
using BudgetTrackerApi.Data.Helpers;
using BudgetTrackerApi.Services.Import;
using BudgetTrackerApi.Models;
using BudgetTrackerApi.Models.Savings;

namespace BudgetTrackerApi.Services.Import.Parsers
{
    /// <summary>
    /// Parser pour les fichiers CSV Fortuneo de type Livret (épargne).
    /// Calcule un solde cumulé par opération à partir du dernier relevé connu,
    /// puis crée un SavingStatement par ligne.
    /// </summary>
    public class FortuneoSavingCsvParser
    {
        public string BankName => "Fortuneo";

        /// <summary>
        /// Parse le contenu CSV et retourne une liste de SavingStatement.
        /// </summary>
        /// <param name="ctx">Contexte d'entrée (fichier ou texte)</param>
        /// <param name="accountId">ID du compte Livret cible</param>
        /// <param name="previousBalance">Solde de départ (dernier relevé connu avant la période du fichier)</param>
        public List<SavingStatement> Parse(ParserInputContext ctx, int accountId, decimal previousBalance)
        {
            var results = new List<SavingStatement>();
            var reader = ctx.GetTextReader();

            if (reader is null)
                return results;

            string? line;
            bool header = true;
            decimal runningBalance = previousBalance;

            // On parse d'abord toutes les lignes pour les trier chronologiquement
            var rawLines = new List<(DateTime Date, decimal Amount, string Description)>();

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (header) { header = false; continue; }

                var values = line.Split(';');
                if (values.Length < 4) continue;

                // Parsing de la date d'opération (colonne 0)
                if (!DateTime.TryParseExact(values[0].Trim(), "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime parsedDate))
                    continue;

                var debitStr = values[3].Trim().Replace(',', '.');
                var creditStr = values.Length > 4 ? values[4].Trim().Replace(',', '.') : "0";

                decimal debit = string.IsNullOrWhiteSpace(debitStr)
                    ? 0 : decimal.Parse(debitStr, System.Globalization.CultureInfo.InvariantCulture);
                decimal credit = string.IsNullOrWhiteSpace(creditStr)
                    ? 0 : decimal.Parse(creditStr, System.Globalization.CultureInfo.InvariantCulture);

                decimal amount = debit + credit;  // débit est déjà négatif dans le CSV Fortuneo
                string description = values[2].Trim();

                rawLines.Add((parsedDate, amount, description));
            }

            // Tri chronologique (le fichier est souvent du plus récent au plus ancien)
            rawLines.Sort((a, b) => a.Date.CompareTo(b.Date));

            // Calcul du solde cumulé
            foreach (var (date, amount, description) in rawLines)
            {
                runningBalance += amount;

                results.Add(new SavingStatement
                {
                    AccountId = accountId,
                    Date = date,
                    Amount = Math.Round(runningBalance, 2),
                    Note = description
                });
            }

            return results;
        }
    }
}
