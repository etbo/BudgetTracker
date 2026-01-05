using Microsoft.AspNetCore.Mvc;
using BudgetTrackerApi.Services;
using BudgetTrackerApi.Data;
using BudgetTrackerApi.Models;
using Microsoft.EntityFrameworkCore; // Adapte selon tes namespaces

[ApiController]
[Route("api/[controller]")]
public class PeaController : ControllerBase
{
    private readonly AppDbContext _db;

    public PeaController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PeaOperation>>> Get()
        => await _db.PeaOperations.ToListAsync();

    [HttpPost]
    public async Task<ActionResult<PeaOperation>> Create(PeaOperation op)
    {
        _db.PeaOperations.Add(op);
        await _db.SaveChangesAsync();
        return Ok(op);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, PeaOperation op)
    {
        Console.WriteLine($"PeaOperation : {op.Id}, {op.Date}, {op.Titulaire}");
        if (id != op.Id) return BadRequest();
        _db.Entry(op).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var operation = await _db.PeaOperations.FindAsync(id);

        if (operation == null)
        {
            return NotFound(); // Retourne 404 si l'ID n'existe pas en base
        }

        _db.PeaOperations.Remove(operation);
        await _db.SaveChangesAsync();

        return NoContent(); // Retourne 204 (Succès sans contenu)
    }
}