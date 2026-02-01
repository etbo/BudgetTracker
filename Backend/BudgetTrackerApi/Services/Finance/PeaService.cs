using System.Globalization;
using BudgetTrackerApi.Data;
using BudgetTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTrackerApi.Services
{
    public interface IPeaService
    {
        Task<List<PeaOperation>> GetAllOperationsAsync();
        Task<List<CumulPea>> CalculerCumul();
    }

    public class PeaService : IPeaService
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public PeaService(IDbContextFactory<AppDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<PeaOperation>> GetAllOperationsAsync()
        {
            using var db = _dbFactory.CreateDbContext();
            return await db.PeaOperations
                           .OrderBy(c => c.Date)
                           .ToListAsync();
        }

        private Dictionary<(int Year, int Month), int> operationCumuleeParMois = new();

        public async Task<List<CumulPea>> CalculerCumul()
        {
            Console.WriteLine($"Début calcul cumul");
            using var db = _dbFactory.CreateDbContext();

            // 1. Récupération des opérations triées
            var orderedOperations = await db.PeaOperations
                .OrderBy(o => o.Date)
                .ToListAsync();

            if (orderedOperations == null || !orderedOperations.Any())
            {
                return new List<CumulPea>();
            }

            // Préparation des prix et tickers (inchangé)
            var allTickers = orderedOperations.Select(o => o.Code).Distinct().ToList();
            var tousLesPrixCaches = await db.PeaCachedStockPrices
                .Where(c => allTickers.Contains(c.Ticker))
                .ToListAsync() ?? new List<PeaCachedStockPrice>();

            var prixOrganises = tousLesPrixCaches
                .GroupBy(c => c.Ticker)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(
                        c => c.Date.Date,
                        c => c.Price
                    )
                );

            // --- 2. Calcul du cumul et valorisation (quotidienne temporaire, mais étendue) ---

            // Key: Date de l'opération
            // Value: (Cumul Achat, Quantités détenues pour valorisation)
            var historiqueQuotidien = new SortedDictionary<DateTime, (double Achat, Dictionary<string, double> Quantites)>();
            double cumulAchatGlobal = 0;
            var quantitesPossedees = new Dictionary<string, double>();

            foreach (var operation in orderedOperations)
            {
                // On ignore l'opération si la date est nulle pour le calcul du cumul
                if (!operation.Date.HasValue) continue;

                // Mise à jour des cumuls d'achats
                cumulAchatGlobal += (double)(operation.Quantité * operation.NetAmount);

                // Mise à jour des quantités détenues par Ticker
                if (quantitesPossedees.ContainsKey(operation.Code))
                {
                    quantitesPossedees[operation.Code] += operation.Quantité;
                }
                else
                {
                    quantitesPossedees[operation.Code] = operation.Quantité;
                }

                // Enregistrement de l'état du portefeuille pour cette date.
                // On clone les quantités pour avoir un snapshot de l'état à la date de l'opération.
                historiqueQuotidien[operation.Date.Value] = (cumulAchatGlobal, new Dictionary<string, double>(quantitesPossedees));
            }

            // --- 3. Filtrage et Valorisation au Dernier Jour Calendaire du Mois ---

            // On cherche la première opération qui a une date non nulle
            var premiereOpAvecDate = orderedOperations.FirstOrDefault(o => o.Date.HasValue);

            // Si aucune opération n'a de date, on s'arrête là
            if (premiereOpAvecDate?.Date == null)
            {
                return new List<CumulPea>();
            }

            // Le .Value! (avec point d'exclamation) dit au compilateur : 
            // "Je garantis que ce n'est pas null car j'ai vérifié juste au-dessus"
            var dateDebut = premiereOpAvecDate.Date.Value.Date;
            var dateFin = DateTime.Now.Date;
            var historiqueMensuel = new List<CumulPea>();

            // Itérer de mois en mois
            var d = new DateTime(dateDebut.Year, dateDebut.Month, 1);

            while (d <= dateFin)
            {
                // Trouver le dernier jour du mois
                var dernierJourDuMois = new DateTime(d.Year, d.Month, DateTime.DaysInMonth(d.Year, d.Month));

                // S'assurer de ne pas dépasser la date d'aujourd'hui
                if (dernierJourDuMois > dateFin)
                {
                    dernierJourDuMois = dateFin;
                }

                // Trouver la dernière entrée de portefeuille enregistrée AVANT ou À cette date de fin de mois.
                var dernierSnapshot = historiqueQuotidien
                    .Where(kvp => kvp.Key.Date <= dernierJourDuMois)
                    .LastOrDefault(); // Comme le dictionnaire est trié, LastOrDefault prend le plus récent.

                // Si nous avons des données pour ce mois
                if (dernierSnapshot.Key != default)
                {
                    var dateSnapshot = dernierSnapshot.Key;
                    var (cumulAchatSnapshot, quantitesSnapshot) = dernierSnapshot.Value;

                    double valeurTotaleJour = 0;

                    // Re-valoriser le snapshot avec les prix du dernier jour du mois (dernierJourDuMois)
                    foreach (var kvp in quantitesSnapshot)
                    {
                        var ticker = kvp.Key;
                        var qte = kvp.Value;

                        // Trouver le prix de clôture pour le dernier jour du mois (ou le jour de bourse le plus proche)
                        if (TryGetClosingPriceFromCache(prixOrganises, dernierJourDuMois, ticker, out double prixJour))
                        {
                            valeurTotaleJour += qte * prixJour;
                        }
                        // Si le prix n'est pas trouvé même après recherche arrière (TryGetClosingPriceFromCache),
                        // la valeur de ce titre est zéro pour ce bilan.
                    }

                    // Enregistrer le bilan
                    historiqueMensuel.Add(new CumulPea(
                        Date: dernierJourDuMois, // Utilise la date de fin de mois CALENDAIRE
                        AchatCumules: cumulAchatSnapshot,
                        ValeurTotale: valeurTotaleJour
                    ));
                }

                // Passer au mois suivant
                d = d.AddMonths(1);
            }

            return historiqueMensuel;
        }

        private bool TryGetClosingPriceFromCache(
                            Dictionary<string, Dictionary<DateTime, decimal>> prixOrganises,
                            DateTime dateOperation,
                            string ticker,
                            out double prix)
        {
            prix = 0.0;

            // 1. Chercher le dictionnaire des prix pour ce Ticker
            if (prixOrganises.TryGetValue(ticker, out var prixParJour))
            {
                DateTime currentDate = dateOperation.Date;

                // 2. Itérer à rebours pour trouver le prix de clôture le plus récent
                // C'est nécessaire car les marchés sont fermés le weekend/jours fériés.
                for (int i = 0; i < 7; i++) // Chercher jusqu'à 7 jours en arrière (pour couvrir le weekend)
                {
                    if (prixParJour.TryGetValue(currentDate, out var prixDecimal))
                    {
                        prix = (double)prixDecimal;
                        return true;
                    }
                    currentDate = currentDate.AddDays(-1);
                }
            }

            return false;
        }
    }
}