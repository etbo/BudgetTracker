using BudgetTrackerApp.Data;
using BudgetTrackerApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class RulesController : ControllerBase
{
    private readonly AppDbContext _db;
    public RulesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CcCategoryRule>>> GetRules() => await _db.CcCategoryRules.ToListAsync();

    [HttpGet("CcCategories")]
    public async Task<ActionResult<IEnumerable<CcCategory>>> GetCcCategories() => await _db.CcCategories.ToListAsync();

    [HttpPost]
    public async Task<ActionResult<CcCategoryRule>> Create(CcCategoryRule rule)
    {
        _db.CcCategoryRules.Add(rule);
        await _db.SaveChangesAsync();
        return Ok(rule);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRule(int id)
    {
        // On utilise _db (le nom que vous avez choisi) 
        // et CcCategoryRules (le nom de votre DbSet)
        var rule = await _db.CcCategoryRules.FindAsync(id);

        if (rule == null)
        {
            return NotFound();
        }

        _db.CcCategoryRules.Remove(rule);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CcCategoryRule rule)
    {
        if (id != rule.Id) return BadRequest();
        _db.Entry(rule).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}