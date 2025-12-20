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
        if (id != op.Id) return BadRequest();
        _db.Entry(op).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}