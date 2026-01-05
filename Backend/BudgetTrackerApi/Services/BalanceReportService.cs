// Fichier : BudgetTrackerApp/Services/Data/BankService.cs (ou Services/BankService.cs)

using BudgetTrackerApp.Data;
using BudgetTrackerApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BudgetTrackerApp.Services
{
    public class BalanceReportService
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public BalanceReportService(IDbContextFactory<AppDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<DailyBalance>> GetCumulatedBalanceAsync()
        {
            using var _dbContext = _dbFactory.CreateDbContext();

            // Étape 1 : Récupérer toutes les opérations
            var operations = await _dbContext.CcOperations
                .ToListAsync();

            if (!operations.Any())
            {
                Console.WriteLine($"New DailyBalance");
                return new List<DailyBalance>();
            }
            else
            {
                Console.WriteLine($"DailyBalance existant");
            }
            
            Console.WriteLine($"operations = {operations.Count()}");

            // Étape 2 : Préparer les données
            var dailyBalances = new SortedDictionary<DateTime, double>();

            var orderedOperations = operations
                .OrderBy(o => o.Date)
                .ToList();

            Console.WriteLine($"orderedOperations = {orderedOperations.Count()}");
            // Étape 3 : Calculer le cumul (Balance Initiale)
            double currentBalance = 0;

            foreach (var op in orderedOperations)
            {
                // Ajouter le montant à la balance cumulée
                currentBalance += op.Montant;

                // Si une opération a déjà eu lieu à cette date, on met à jour la balance finale de la journée.
                // Sinon, on ajoute un nouveau point.
                dailyBalances[op.Date] = currentBalance;
            }

            // Étape 4 : Conversion en liste pour le graphique
            // Nous pourrions aussi remplir les jours "vides" si besoin, mais le SortedDictionary gère l'ordre chronologique.
            return dailyBalances
                .Select(kvp => new DailyBalance
                {
                    Date = kvp.Key,
                    CumulatedBalance = kvp.Value
                })
                .ToList();
        }
    }
}