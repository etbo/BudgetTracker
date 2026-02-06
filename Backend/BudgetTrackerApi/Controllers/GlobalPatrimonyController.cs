using BudgetTrackerApi.Data;
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
        // 1. Récupération des données avec cast en (DateTime) pour garantir le type non-nullable
        var savings = await _db.SavingStatements
            .Select(s => new { Date = s.Date!, Amount = (decimal)s.Amount, Cat = "Savings", Id = s.SavingAccountId.ToString() }).ToListAsync();

        var lifeInsurance = await _db.LifeInsuranceStatements
            .Select(s => new { Date = s.Date!, Amount = (decimal)(s.UnitCount * s.UnitValue), Cat = "AV", Id = s.LifeInsuranceLineId.ToString() }).ToListAsync();

        var ccOperations = await _db.CcOperations
            .Select(s => new { Date = s.Date!, Amount = (decimal)s.Amount }).ToListAsync();

        var peaStocks = await _db.PeaOperations
            .Where(s => s.Date != null)
            .Select(s => new { Date = (DateTime)s.Date!, Ticker = s.Code, Quantity = (decimal)s.Quantity }).ToListAsync();

        var peaPrices = await _db.PeaCachedStockPrices
            .Select(s => new { Date = s.Date!, Ticker = s.Ticker, Price = (decimal)s.Price }).ToListAsync();

        // 2. Création de la liste de dates (Maintenant que tout est DateTime, plus d'erreur)
        var allDates = new List<DateTime>();
        allDates.AddRange(savings.Select(x => x.Date));
        allDates.AddRange(lifeInsurance.Select(x => x.Date));
        allDates.AddRange(ccOperations.Select(x => x.Date));
        allDates.AddRange(peaStocks.Select(x => x.Date));

        if (!allDates.Any()) return Ok(new List<GlobalHistoryDto>());

        // 3. Reste de la logique identique...
        var startDate = new DateTime(allDates.Min().Year, allDates.Min().Month, 1);
        var history = new List<GlobalHistoryDto>();
        var lastBalances = new Dictionary<string, decimal>();

        for (var date = startDate; date <= DateTime.Now; date = date.AddMonths(1))
        {
            var endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);

            // --- CALCUL PEA (Somme des flux de quantité x Dernier prix connu) ---
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

                currentPeaValue += (stock.Value * priceAtDate);
            }

            // --- CALCUL CC ---
            decimal currentCcBalance = ccOperations.Where(op => op.Date <= endOfMonth).Sum(op => op.Amount);

            // --- CALCUL AUTRES ---
            var monthEntries = savings.Concat(lifeInsurance)
                .Where(x => x.Date.Year == date.Year && x.Date.Month == date.Month)
                .ToList();

            foreach (var entry in monthEntries)
            {
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