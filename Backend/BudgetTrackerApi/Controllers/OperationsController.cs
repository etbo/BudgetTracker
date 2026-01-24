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
    private readonly IRuleService _ruleService;

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

        // --- 3. Exécution de la requête SQL ---
        // Grâce au [NotMapped] dans CcOperation.cs, EF Core ignore MacroCategory ici
        var results = await query.ToListAsync();

        // --- 4. Logique Post-Requête A : Catégorisation automatique par règles ---
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

        // --- 5. Logique Post-Requête B : Affectation de la MacroCategory ---
        // On récupère le dictionnaire des types (ex: "Courses" -> "Variable")
        var categoryMapping = await _db.CcCategories
            .ToDictionaryAsync(c => c.Name, c => c.Type);

        foreach (var op in results)
        {
            if (!string.IsNullOrEmpty(op.Categorie) && categoryMapping.TryGetValue(op.Categorie, out var type))
            {
                op.MacroCategory = type;
            }
            else
            {
                op.MacroCategory = "Inconnu";
            }
        }

        // --- 6. Filtre final "Suggestions" ---
        // On le fait après la catégorisation auto pour que IsModified soit à jour
        if (suggestedCat)
        {
            results = results.Where(op => op.IsModified).ToList();
        }

        return Ok(results);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CcOperation op)
    {
        if (id != op.Id) return BadRequest();

        // 1. On récupère la version actuelle en base pour comparer
        var existingOp = await _db.CcOperations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        // 2. Si la catégorie était vide et qu'elle vient d'être remplie
        if (string.IsNullOrEmpty(existingOp?.Categorie) && !string.IsNullOrEmpty(op.Categorie))
        {
            // On cherche la règle qui correspond à ce qui vient d'être validé
            var rule = await _db.CcCategoryRules
                .FirstOrDefaultAsync(r => r.Category == op.Categorie &&
                                         (op.Description ?? "").Contains(r.Pattern ?? ""));

            if (rule != null)
            {
                rule.UsageCount++;

                if (!rule.LastAppliedAt.HasValue || op.Date > rule.LastAppliedAt)
                {
                    rule.LastAppliedAt = op.Date;
                }
            }
        }

        _db.Entry(op).State = EntityState.Modified;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("suggest")]
    public async Task<IActionResult> SuggestCategory([FromBody] CcOperation op)
    {
        if (op == null) return BadRequest();

        // 1. On récupère les règles actives
        var rules = await _db.CcCategoryRules.Where(r => r.IsUsed).ToListAsync();

        // 2. On cherche la règle qui match (logique identique à ton RuleService)
        var matchingRule = rules.FirstOrDefault(r =>
            !string.IsNullOrEmpty(r.Pattern) &&
            (op.Description ?? "").Contains(r.Pattern, StringComparison.OrdinalIgnoreCase));

        if (matchingRule != null)
        {
            return Ok(new
            {
                categorie = matchingRule.Category,
                isSuggested = true
            });
        }

        return Ok(new { categorie = "", isSuggested = false });
    }
}