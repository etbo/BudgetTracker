using BudgetTrackerApi.Data;
using BudgetTrackerApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class GlobalPatrimonyController : ControllerBase
{
    private readonly AppDbContext _db;

    public GlobalPatrimonyController(AppDbContext context)
    {
        _db = context;
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<GlobalHistoryDto>>> GetGlobalHistory()
    {
        // 1. Récupération des données unifiées

        // Pour les livrets (Savings) : on utilise AccountId
        var savings = await _db.SavingStatements
            .Select(s => new
            {
                Date = s.Date,
                Amount = s.Amount,
                Cat = "Savings",
                Id = s.AccountId.ToString() // Changement ici : AccountId
            }).ToListAsync();

        // Pour l'Assurance Vie : on calcule le montant (UnitCount * UnitValue)
        var lifeInsurance = await _db.LifeInsuranceStatements
            .Select(s => new
            {
                Date = s.Date,
                Amount = s.UnitCount * s.UnitValue,
                Cat = "AV",
                Id = s.Line.AccountId.ToString() // Accès via la navigation vers Account
            }).ToListAsync();

        var ccOperations = await _db.CcOperations
            .Select(s => new { Date = s.Date, Amount = s.Amount }).ToListAsync();

        var peaStocks = await _db.PeaOperations
            .Select(s => new
            {
                Date = s.Date,
                Ticker = s.Code,
                Quantity = (decimal)s.Quantity
            }).ToListAsync();

        var peaPrices = await _db.PeaCachedStockPrices
            .Select(s => new { Date = s.Date, Ticker = s.Ticker, Price = s.Price }).ToListAsync();

        // 2. Collecte de toutes les dates pour définir la plage du graphique
        var allDates = new List<DateTime>();

        // On utilise .Where(x => x.HasValue).Select(x => x.Value) pour garantir du DateTime non-nullable
        allDates.AddRange(savings.Select(x => x.Date)); // Déjà DateTime
        allDates.AddRange(lifeInsurance.Select(x => x.Date)); // Déjà DateTime

        allDates.AddRange(ccOperations.Select(x => x.Date));

        // Ici on filtre les nulls car PeaOperation.Date est souvent nullable en BDD
        allDates.AddRange(peaStocks
            .Where(x => x.Date.HasValue)
            .Select(x => x.Date!.Value));

        if (!allDates.Any()) return Ok(new List<GlobalHistoryDto>());

        // 3. Construction de l'historique mois par mois
        var startDate = new DateTime(allDates.Min().Year, allDates.Min().Month, 1);
        var history = new List<GlobalHistoryDto>();
        var lastBalances = new Dictionary<string, decimal>();

        for (var date = startDate; date <= DateTime.Now; date = date.AddMonths(1))
        {
            var endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);

            // --- CALCUL PEA ---
            var peaOpsUntilNow = peaStocks.Where(x => x.Date <= endOfMonth).ToList();
            var currentQuantities = peaOpsUntilNow
                .GroupBy(x => x.Ticker)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

            decimal currentPeaValue = 0;
            foreach (var stock in currentQuantities)
            {
                var priceAtDate = peaPrices
                    .Where(p => p.Ticker == stock.Key && p.Date <= endOfMonth)
                    .OrderByDescending(p => p.Date)
                    .FirstOrDefault()?.Price ?? 0;

                currentPeaValue += stock.Value * priceAtDate;
            }

            // --- CALCUL CC (Cash) ---
            // On cast chaque montant en decimal avant de faire la somme
            decimal currentCcBalance = ccOperations
                .Where(op => op.Date <= endOfMonth)
                .Sum(op => (decimal)op.Amount);

            // --- CALCUL LIVRETS & AV (Last known value logic) ---
            var monthEntries = savings.Concat(lifeInsurance)
                .Where(x => x.Date.Year == date.Year && x.Date.Month == date.Month)
                .ToList();

            foreach (var entry in monthEntries)
            {
                // La clé permet de garder la dernière valeur connue par compte/ligne
                string key = $"{entry.Cat}_{entry.Id}";
                lastBalances[key] = entry.Amount;
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

        return Ok(history);
    }
}