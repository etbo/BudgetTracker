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
    public async Task<ActionResult<IEnumerable<CategoryRule>>> GetRules() => await _db.CategoryRules.ToListAsync();

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories() => await _db.Categories.ToListAsync();

    [HttpPost]
    public async Task<ActionResult<CategoryRule>> Create(CategoryRule rule)
    {
        _db.CategoryRules.Add(rule);
        await _db.SaveChangesAsync();
        return Ok(rule);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CategoryRule rule)
    {
        if (id != rule.Id) return BadRequest();
        _db.Entry(rule).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}