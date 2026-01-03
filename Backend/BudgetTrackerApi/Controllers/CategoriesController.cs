using BudgetTrackerApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CcCategoriesController : ControllerBase
{
    private readonly AppDbContext _db;
    public CcCategoriesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CcCategory>>> Get() 
        => await _db.CcCategories.ToListAsync();

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CcCategory updatedCat)
    {
        var existingCat = await _db.CcCategories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (existingCat == null) return NotFound();

        // Logique de renommage global
        if (existingCat.Name != updatedCat.Name)
        {
            // 1. Mise à jour des opérations CC
            var ops = await _db.CcOperations.Where(o => o.Categorie == existingCat.Name).ToListAsync();
            ops.ForEach(o => o.Categorie = updatedCat.Name);

            // 2. Mise à jour des règles
            var rules = await _db.CcCategoryRules.Where(r => r.Category == existingCat.Name).ToListAsync();
            rules.ForEach(r => r.Category = updatedCat.Name);
        }

        _db.Entry(updatedCat).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<CcCategory>> Create(CcCategory cat)
    {
        _db.CcCategories.Add(cat);
        await _db.SaveChangesAsync();
        return Ok(cat);
    }

    [HttpDelete("{id}")] // <--- Très important : définit la méthode et le paramètre
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _db.CcCategories.FindAsync(id);
        if (category == null) return NotFound();

        _db.CcCategories.Remove(category);
        await _db.SaveChangesAsync();

        return NoContent(); // Retourne 204
    }
}