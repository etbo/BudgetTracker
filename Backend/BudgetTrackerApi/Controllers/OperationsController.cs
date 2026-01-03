using BudgetTrackerApp.Data;
using BudgetTrackerApp.Models;
using BudgetTrackerApp.Services;
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
    public async Task<ActionResult<IEnumerable<CcOperation>>> Get([FromQuery] string filterType)
    {
        IQueryable<CcOperation> query = _db.CcOperations;

        switch (filterType)
        {
            case "A": query = query.Where(op => string.IsNullOrEmpty(op.Categorie)); break;
            case "B": query = query.Where(op => op.Description != null && (op.Description.ToUpper().Contains("CHEQUE") || op.Description.ToUpper().Contains("CHQ "))); break;
            case "C": 
                var lastDate = await _db.CcOperations.MaxAsync(op => op.DateImport);
                query = query.Where(op => op.DateImport == lastDate);
                break;
            case "D":
                var validCats = _db.CcCategories.Select(c => c.Name).ToHashSet();
                var all = await query.ToListAsync();
                return Ok(all.Where(o => !string.IsNullOrEmpty(o.Categorie) && !validCats.Contains(o.Categorie)));
        }

        var results = await query.ToListAsync();

        var rules = await _ruleService.GetActiveRulesAsync();
        
        // Appliquer la catégorisation automatique sur les vides (TraiterOperation en Blazor)
        foreach (var op in results.Where(o => string.IsNullOrEmpty(o.Categorie)))
        {
            var autoCat = _ruleService.GetAutoCategory(op, rules);
            if (!string.IsNullOrEmpty(autoCat))
            {
                op.Categorie = autoCat;
                op.IsModified = true; 
            }
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
}