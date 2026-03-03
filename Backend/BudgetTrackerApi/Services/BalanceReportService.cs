// Fichier : BudgetTrackerApi/Services/Data/BankService.cs (ou Services/BankService.cs)

using BudgetTrackerApi.Data;
using BudgetTrackerApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BudgetTrackerApi.Services
{
    public class BalanceReportService
    {
        private readonly AppDbContext _context;

        public BalanceReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CcDailyBalance>> GetCumulatedBalanceAsync()
        {

            // Étape 1 : Récupérer toutes les opérations
            var operations = await _context.CcOperations
                .ToListAsync();

            if (!operations.Any())
            {
                Console.WriteLine($"New CcDailyBalance");
                return new List<CcDailyBalance>();
            }
            else
            {
                Console.WriteLine($"CcDailyBalance existant");
            }
            
            Console.WriteLine($"operations = {operations.Count()}");

            // Étape 2 : Préparer les données
            var CcDailyBalances = new SortedDictionary<DateTime, double>();

            var orderedOperations = operations
                .OrderBy(o => o.Date)
                .ToList();

            Console.WriteLine($"orderedOperations = {orderedOperations.Count()}");
            // Étape 3 : Calculer le cumul (Balance Initiale)
            double currentBalance = 0;

            foreach (var op in orderedOperations)
            {
                // Ajouter le montant à la balance cumulée
                currentBalance += op.Amount;

                // Si une opération a déjà eu lieu à cette date, on met à jour la balance finale de la journée.
                // Sinon, on ajoute un nouveau point.
                CcDailyBalances[op.Date] = currentBalance;
            }

            // Étape 4 : Conversion en liste pour le graphique
            // Nous pourrions aussi remplir les jours "vides" si besoin, mais le SortedDictionary gère l'ordre chronologique.
            return CcDailyBalances
                .Select(kvp => new CcDailyBalance
                {
                    Date = kvp.Key,
                    CumulatedBalance = kvp.Value
                })
                .ToList();
        }
    }
}