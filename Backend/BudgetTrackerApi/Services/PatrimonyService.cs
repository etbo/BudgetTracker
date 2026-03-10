using BudgetTrackerApi.Data;
using BudgetTrackerApi.DTOs;
using BudgetTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTrackerApi.Services
{
    public class PatrimonyService
    {
        private readonly AppDbContext _db;

        public PatrimonyService(AppDbContext context)
        {
            _db = context;
        }

        public async Task<PatrimonySummaryDto> GetCurrentSummaryAsync()
        {
            var summary = new PatrimonySummaryDto();

            // 1. Cash (Comptes Courants)
            summary.Cash = (decimal)await _db.CcOperations.SumAsync(op => op.Amount);

            // 2. Livrets (Dernière valeur connue pour chaque compte)
            // On récupère le dernier relevé de chaque livret
            var latestSavings = await _db.SavingStatements
                .GroupBy(s => s.AccountId)
                .Select(g => g.OrderByDescending(s => s.Date).FirstOrDefault())
                .ToListAsync();

            summary.Savings = latestSavings
                .Where(s => s != null)
                .Sum(s => s!.Amount);

            // 3. Assurance Vie (Dernière valeur connue pour chaque ligne)
            var latestAV = await _db.LifeInsuranceStatements
                .GroupBy(s => s.LifeInsuranceLineId)
                .Select(g => g.OrderByDescending(s => s.Date).FirstOrDefault())
                .ToListAsync();

            summary.LifeInsurance = latestAV
                .Where(s => s != null)
                .Sum(s => s!.UnitCount * s!.UnitValue);

            // 4. PEA (Quantité totale * Dernier prix)
            var quantities = await _db.PeaOperations
                .GroupBy(o => o.Code)
                .Select(g => new { 
                    Ticker = g.Key, 
                    Quantity = g.Sum(o => (decimal)o.Quantity) 
                })
                .ToListAsync();

            decimal totalPea = 0;
            foreach (var q in quantities)
            {
                var latestPrice = await _db.PeaCachedStockPrices
                    .Where(p => p.Ticker == q.Ticker)
                    .OrderByDescending(p => p.Date)
                    .Select(p => p.Price)
                    .FirstOrDefaultAsync();

                totalPea += q.Quantity * latestPrice;
            }
            summary.Pea = totalPea;

            // Optionnel : Remplir les détails
            foreach(var s in latestSavings.Where(s => s != null))
            {
                summary.Details[$"Savings_{s!.AccountId}"] = s.Amount;
            }
            // AV details...
            foreach(var s in latestAV.Where(s => s != null))
            {
                summary.Details[$"AV_{s!.LifeInsuranceLineId}"] = s.UnitCount * s.UnitValue;
            }

            return summary;
        }
    }
}
