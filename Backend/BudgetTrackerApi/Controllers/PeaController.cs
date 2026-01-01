using Microsoft.AspNetCore.Mvc;
using BudgetTrackerApp.Services;
using BudgetTrackerApp.Data;
using BudgetTrackerApp.Models;
using Microsoft.EntityFrameworkCore; // Adapte selon tes namespaces

[ApiController]
[Route("api/[controller]")]
public class PeaController : ControllerBase
{
    private readonly AppDbContext _db;

    public PeaController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OperationPea>>> Get()
        => await _db.OperationsPea.ToListAsync();

    [HttpPost]
    public async Task<ActionResult<OperationPea>> Create(OperationPea op)
    {
        _db.OperationsPea.Add(op);
        await _db.SaveChangesAsync();
        return Ok(op);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, OperationPea op)
    {
        Console.WriteLine($"OperationPea : {op.Id}, {op.Date}, {op.Titulaire}");
        if (id != op.Id) return BadRequest();
        _db.Entry(op).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var operation = await _db.OperationsPea.FindAsync(id);

        if (operation == null)
        {
            return NotFound(); // Retourne 404 si l'ID n'existe pas en base
        }

        _db.OperationsPea.Remove(operation);
        await _db.SaveChangesAsync();

        return NoContent(); // Retourne 204 (Succès sans contenu)
    }
}