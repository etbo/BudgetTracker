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
                .Select(g => new
                {
                    Ticker = g.Key,
                    Quantity = g.Sum(o => (decimal)o.Quantity)
                })
                .ToListAsync();

            decimal totalPea = 0;
            foreach (var q in quantities)
            {
                if (q.Ticker == "Appro")
                {
                    totalPea += q.Quantity * 1; // 1€ par unité d'Appro
                }
                else
                {
                    var latestPrice = await _db.PeaCachedStockPrices
                        .Where(p => p.Ticker == q.Ticker)
                        .OrderByDescending(p => p.Date)
                        .Select(p => p.Price)
                        .FirstOrDefaultAsync();

                    totalPea += q.Quantity * latestPrice;
                }
            }
            summary.Pea = totalPea;

            // Remplir les détails
            foreach (var s in latestSavings.Where(s => s != null))
            {
                summary.Details[$"Savings_{s!.AccountId}"] = s.Amount;
            }
            foreach (var s in latestAV.Where(s => s != null))
            {
                summary.Details[$"AV_{s!.LifeInsuranceLineId}"] = s.UnitCount * s.UnitValue;
            }

            return summary;
        }

        public async Task<IEnumerable<GlobalHistoryDto>> GetGlobalHistoryAsync()
        {
            Console.WriteLine($"____________________________");

            // 1. Récupération des données unifiées
            var savings = await _db.SavingStatements
                .Select(s => new { s.Date, s.Amount, Cat = "Savings", Id = s.AccountId.ToString() })
                .ToListAsync();

            var lifeInsurance = await _db.LifeInsuranceStatements
                .Select(s => new { s.Date, Amount = s.UnitCount * s.UnitValue, Cat = "AV", Id = s.LifeInsuranceLineId.ToString() })
                .ToListAsync();

            var ccOperations = await _db.CcOperations
                .Select(s => new { s.Date, s.Amount })
                .ToListAsync();

            var peaStocks = await _db.PeaOperations
                .Select(s => new { s.Date, Ticker = s.Code, Quantity = (decimal)s.Quantity })
                .ToListAsync();

            var peaPrices = await _db.PeaCachedStockPrices
                .Select(s => new { s.Date, s.Ticker, s.Price })
                .ToListAsync();

            // 2. Collecte des dates
            var allDates = savings.Select(x => x.Date)
                .Concat(lifeInsurance.Select(x => x.Date))
                .Concat(ccOperations.Select(x => x.Date))
                .Concat(peaStocks.Where(x => x.Date.HasValue).Select(x => x.Date!.Value))
                .ToList();

            if (!allDates.Any()) return new List<GlobalHistoryDto>();

            var startDate = new DateTime(allDates.Min().Year, allDates.Min().Month, 1);
            var history = new List<GlobalHistoryDto>();
            var lastBalances = new Dictionary<string, decimal>();

            for (var date = startDate; date <= DateTime.Now; date = date.AddMonths(1))
            {
                var endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);

                // --- PEA (Calcul par ticker pour gérer l'Appro) ---
                var currentQuantities = peaStocks.Where(x => x.Date <= endOfMonth)
                    .GroupBy(x => x.Ticker)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

                decimal currentPeaValue = 0;
                foreach (var kvp in currentQuantities)
                {
                    var ticker = kvp.Key;
                    var qte = kvp.Value;
                    decimal priceAtDate = 0;

                    if (ticker == "Appro")
                    {
                        priceAtDate = 1;
                    }
                    else
                    {
                        priceAtDate = peaPrices
                            .Where(p => p.Ticker == ticker && p.Date <= endOfMonth)
                            .OrderByDescending(p => p.Date)
                            .FirstOrDefault()?.Price ?? 0;
                    }
                    currentPeaValue += qte * priceAtDate;
                }

                // --- CASH ---
                decimal currentCcBalance = ccOperations.Where(op => op.Date <= endOfMonth).Sum(op => (decimal)op.Amount);

                // --- SAVINGS & AV ---
                var monthEntries = savings.Concat(lifeInsurance)
                    .Where(x => x.Date.Year == date.Year && x.Date.Month == date.Month)
                    .ToList();

                foreach (var entry in monthEntries)
                {
                    lastBalances[$"{entry.Cat}_{entry.Id}"] = entry.Amount;
                }

                history.Add(new GlobalHistoryDto
                {
                    Label = $"{date.Month:D2}/{date.Year}",
                    Date = date,
                    Cash = currentCcBalance,
                    Savings = lastBalances.Where(x => x.Key.StartsWith("Savings_")).Sum(x => x.Value),
                    LifeInsurance = lastBalances.Where(x => x.Key.StartsWith("AV_")).Sum(x => x.Value),
                    Pea = currentPeaValue
                });
            }

            return history;
        }
    }
}