using BudgetTrackerApi.Data;
using BudgetTrackerApi.Models;
using BudgetTrackerApi.Services;
using BudgetTrackerApi.DTOs; // Assure-toi d'importer tes DTOs
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
    public async Task<ActionResult<IEnumerable<CcOperationDto>>> Get(
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
            var lastImportDate = await _db.CcImportLogs
                .MaxAsync(log => (DateTime?)log.ImportDate);

            if (lastImportDate.HasValue)
            {
                query = query.Where(op => op.ImportLog!.ImportDate == lastImportDate.Value);
            }
        }
        else if (startDate.HasValue && endDate.HasValue)
        {
            var start = startDate.Value.Date;
            var end = endDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(op => op.Date >= start && op.Date <= end);
        }

        // --- 2. Filtres cumulables ---
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

        // --- 3. Exécution de la requête SQL (On récupère les Models) ---
        var operations = await query.ToListAsync();

        // --- 4. Préparation des données complémentaires ---
        var rules = await _ruleService.GetActiveRulesAsync();
        var categoryMapping = await _db.CcCategories.ToDictionaryAsync(c => c.Name, c => c.Type);

        // --- 5. Mapping Manuel Model -> DTO avec Logique Métier ---
        var results = operations.Select(op =>
        {
            // Calcul de la suggestion auto
            string? autoCat = null;
            if (string.IsNullOrEmpty(op.Categorie))
            {
                autoCat = _ruleService.GetAutoCategory(op, rules);
            }

            // Détermination de la MacroCategory
            string currentCat = autoCat ?? op.Categorie ?? "";
            categoryMapping.TryGetValue(currentCat, out var macro);

            return new CcOperationDto
            {
                Id = op.Id,
                Date = op.Date,
                Amount = (decimal)op.Montant,
                Label = op.Description ?? "",
                Categorie = currentCat,
                IsSuggested = !string.IsNullOrEmpty(autoCat),
                MacroCategory = macro ?? "Inconnu",
                Comment = op.Comment,
                Banque = op.Banque
            };
        }).ToList();

        // --- 6. Filtre final "Suggestions" sur les DTOs ---
        if (suggestedCat)
        {
            results = results.Where(dto => dto.IsSuggested).ToList();
        }

        return Ok(results);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CcOperationDto dto)
    {
        if (id != dto.Id) return BadRequest();

        // On récupère le MODEL en base pour le mettre à jour
        var op = await _db.CcOperations.FindAsync(id);
        if (op == null) return NotFound();

        // Logique des règles (si la catégorie passe de vide à remplie)
        if (string.IsNullOrEmpty(op.Categorie) && !string.IsNullOrEmpty(dto.Categorie))
        {
            var rule = await _db.CcCategoryRules
                .FirstOrDefaultAsync(r => r.Category == dto.Categorie &&
                                         (op.Description ?? "").Contains(r.Pattern ?? ""));

            if (rule != null)
            {
                rule.UsageCount++;
                if (!rule.LastAppliedAt.HasValue || op.Date > rule.LastAppliedAt)
                    rule.LastAppliedAt = op.Date;
            }
        }

        // Mise à jour des champs du Model depuis le DTO
        op.Categorie = dto.Categorie;
        // op.Comment = dto.Comment; // Si tu l'ajoutes au DTO

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("suggest")]
    public async Task<IActionResult> SuggestCategory([FromBody] CcOperationDto opDto)
    {
        if (opDto == null) return BadRequest();

        // On crée un model temporaire pour le service de règles
        var tempOp = new CcOperation { Description = opDto.Label };
        
        var rules = await _db.CcCategoryRules.Where(r => r.IsUsed).ToListAsync();
        var matchingRule = rules.FirstOrDefault(r =>
            !string.IsNullOrEmpty(r.Pattern) &&
            (tempOp.Description ?? "").Contains(r.Pattern, StringComparison.OrdinalIgnoreCase));

        if (matchingRule != null)
        {
            return Ok(new { categorie = matchingRule.Category, isSuggested = true });
        }

        return Ok(new { categorie = "", isSuggested = false });
    }
}