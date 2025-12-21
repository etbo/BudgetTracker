using BudgetTrackerApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;
    public CategoriesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> Get() 
        => await _db.Categories.ToListAsync();

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Category updatedCat)
    {
        var existingCat = await _db.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (existingCat == null) return NotFound();

        // Logique de renommage global
        if (existingCat.Name != updatedCat.Name)
        {
            // 1. Mise à jour des opérations CC
            var ops = await _db.OperationsCC.Where(o => o.Categorie == existingCat.Name).ToListAsync();
            ops.ForEach(o => o.Categorie = updatedCat.Name);

            // 2. Mise à jour des règles
            var rules = await _db.CategoryRules.Where(r => r.Category == existingCat.Name).ToListAsync();
            rules.ForEach(r => r.Category = updatedCat.Name);
        }

        _db.Entry(updatedCat).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Category>> Create(Category cat)
    {
        _db.Categories.Add(cat);
        await _db.SaveChangesAsync();
        return Ok(cat);
    }
}