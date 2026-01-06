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
    public async Task<IActionResult> GetEvolution()
    {
        // 1. Récupérer les opérations triées
        var operations = await _db.CcOperations
            .OrderBy(o => o.Date)
            .Select(o => new { o.Date, o.Montant })
            .ToListAsync();

        if (!operations.Any()) return Ok(new List<object>());

        // 2. Calculer le solde cumulé pour chaque opération
        double runningBalance = 0;
        var allPoints = operations.Select(o =>
        {
            runningBalance += o.Montant;
            return new
            {
                // On garde seulement la partie Date (sans l'heure) pour le regroupement
                Day = o.Date.Date,
                Balance = runningBalance
            };
        }).ToList();

        // 3. Ne garder que le DERNIER point de chaque journée
        var dailyEvolution = allPoints
            .GroupBy(p => p.Day)
            .Select(g => new
            {
                Date = g.Key,
                // On prend le dernier élément du groupe (le solde final du jour)
                CumulatedBalance = g.Last().Balance
            })
            .OrderBy(p => p.Date)
            .ToList();

        return Ok(dailyEvolution);
    }

    [HttpGet("expenses-by-category")]
    public async Task<IActionResult> GetExpensesByCategory([FromQuery] DateTime? start, [FromQuery] DateTime? end)
    {
        var query = _db.CcOperations.AsQueryable();

        if (start.HasValue && end.HasValue)
        {
            query = query.Where(o => o.Date >= start && o.Date <= end);
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