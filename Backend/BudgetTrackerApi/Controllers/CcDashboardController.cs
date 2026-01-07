using BudgetTrackerApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CcDashboardController : ControllerBase
{
    private readonly AppDbContext _db;

    public CcDashboardController(AppDbContext context)
    {
        _db = context;
    }


    [HttpGet("evolution")]
    public async Task<IActionResult> GetEvolution(
    [FromQuery] DateTime? start,
    [FromQuery] DateTime? end,
    [FromQuery] string? excludedCategories)
    {
        try
        {
            var query = _db.CcOperations.AsQueryable();

            // 1. Filtres
            if (start.HasValue) query = query.Where(op => op.Date >= start.Value);
            if (end.HasValue) query = query.Where(op => op.Date <= end.Value);

            if (!string.IsNullOrEmpty(excludedCategories))
            {
                var excludedList = excludedCategories.Split(',').ToList();
                query = query.Where(op => !excludedList.Contains(op.Categorie));
            }

            // 2. Groupement par date (ne retourne que les dates avec data)
            var dailyTotals = await query
                .GroupBy(op => op.Date.Date)
                .Select(g => new { Date = g.Key, DailyAmount = g.Sum(x => x.Montant) })
                .OrderBy(x => x.Date)
                .ToListAsync();

            // 3. Calcul du cumulé
            double runningTotal = 0;
            var finalResult = dailyTotals.Select(d =>
            {
                runningTotal += d.DailyAmount;
                return new
                {
                    date = d.Date.ToString("yyyy-MM-dd"), // Format propre pour JS
                    cumulatedBalance = runningTotal       // Nom exact attendu par le chart
                };
            }).ToList();

            return Ok(finalResult); // On renvoie bien le résultat transformé
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("expenses-by-category")]
    public async Task<IActionResult> GetExpensesByCategory(
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? end,
        [FromQuery] string? excludedCategories)
    {
        {
            var query = _db.CcOperations.AsQueryable();

            if (start.HasValue && end.HasValue)
            {
                query = query.Where(o => o.Date >= start && o.Date <= end);
            }

            if (!string.IsNullOrEmpty(excludedCategories))
            {
                var excludedList = excludedCategories.Split(',').ToList();
                query = query.Where(op => !excludedList.Contains(op.Categorie));
            }

            var expenses = await query
                .Where(o => o.Montant < 0)
                .GroupBy(o => o.Categorie ?? "Sans catégorie")
                .Select(g => new
                {
                    Category = g.Key,
                    Total = Math.Abs(g.Sum(o => o.Montant))
                })
                .OrderByDescending(x => x.Total)
                .ToListAsync();

            return Ok(expenses);
        }
    }
}