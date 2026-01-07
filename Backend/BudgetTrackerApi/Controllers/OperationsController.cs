using BudgetTrackerApi.Data;
using BudgetTrackerApi.Models;
using BudgetTrackerApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class OperationsController : ControllerBase
{
    private readonly AppDbContext _db;

    private readonly IRuleService _ruleService; // Injection du service

    public OperationsController(AppDbContext db, IRuleService ruleService)
    {
        _db = db;
        _ruleService = ruleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CcOperation>>> Get(
    [FromQuery] string mode = "last",
    [FromQuery] bool missingCat = false,
    [FromQuery] bool onlyCheques = false,
    [FromQuery] bool suggestedCat = false,
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null,
    [FromQuery] string? excludedCategories = null)
    {

        var query = _db.CcOperations.AsQueryable();

        // --- 1. Filtre de base (Période / Mode) ---
        if (mode == "last")
        {
            // On récupère la date du dernier import
            if (await _db.CcOperations.AnyAsync())
            {
                var lastDate = await _db.CcOperations.MaxAsync(op => op.DateImport);
                query = query.Where(op => op.DateImport == lastDate);
            }
        }
        else if (startDate.HasValue && endDate.HasValue)
        {
            query = query.Where(op => op.Date >= startDate.Value && op.Date <= endDate.Value);
        }
        // Si mode == "AllOps", on ne filtre pas par date

        // --- 2. Filtres cumulables (Chips) ---
        if (missingCat)
        {
            query = query.Where(op => string.IsNullOrEmpty(op.Categorie));
        }

        if (onlyCheques)
        {
            query = query.Where(op => op.Description != null &&
                (op.Description.ToUpper().Contains("CHEQUE") || op.Description.ToUpper().Contains("CHQ")));
        }

        if (!string.IsNullOrEmpty(excludedCategories))
        {
            var excludedList = excludedCategories.Split(',').ToList();
            query = query.Where(op => op.Categorie == null || !excludedList.Contains(op.Categorie));
        }

        // --- 3. Exécution de la requête ---
        var results = await query.ToListAsync();

        // --- 4. Logique Post-Requête (Catégorisation auto) ---
        var rules = await _ruleService.GetActiveRulesAsync();

        foreach (var op in results.Where(o => string.IsNullOrEmpty(o.Categorie)))
        {
            var autoCat = _ruleService.GetAutoCategory(op, rules);
            if (!string.IsNullOrEmpty(autoCat))
            {
                op.Categorie = autoCat;
                op.IsModified = true;
            }
        }

        // --- 5. Filtre "Suggestions" (uniquement celles qui ont été modifiées par les règles) ---
        if (suggestedCat)
        {
            results = results.Where(op => op.IsModified).ToList();
        }

        return Ok(results);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CcOperation op)
    {
        var entity = await _db.CcOperations.FindAsync(id);
        if (entity == null) return NotFound();

        entity.Categorie = op.Categorie;
        entity.Comment = op.Comment;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("suggest")]
    public async Task<IActionResult> SuggestCategory([FromBody] CcOperation op)
    {
        if (op == null) return BadRequest();
        
        // 1. Récupérer toutes les règles en base
        var rules = await _db.CcCategoryRules.ToListAsync();

        // 2. Utiliser ton service existant
        string suggestedCat = _ruleService.GetAutoCategory(op, rules);

        // 3. Répondre au frontend
        return Ok(new
        {
            categorie = suggestedCat,
            isSuggested = !string.IsNullOrEmpty(suggestedCat)
        });
    }
}