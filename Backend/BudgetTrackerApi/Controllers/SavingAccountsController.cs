using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BudgetTrackerApi.Data; // À ajuster selon ton namespace
using BudgetTrackerApi.Models.Savings;

[ApiController]
[Route("api/[controller]")]
public class SavingAccountsController : ControllerBase
{
    private readonly AppDbContext _db;

    public SavingAccountsController(AppDbContext context)
    {
        _db = context;
    }

    // GET: api/SavingAccounts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SavingAccount>>> GetAccounts()
    {
        return await _db.SavingAccounts
            .Include(a => a.Statements) // On inclut l'historique
            .ToListAsync();
    }

    // POST: api/SavingAccounts
    [HttpPost]
    public async Task<ActionResult<SavingAccount>> CreateAccount(SavingAccount account)
    {
        _db.SavingAccounts.Add(account);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAccounts), new { id = account.Id }, account);
    }

    // POST: api/SavingAccounts/{id}/statements
    [HttpPost("{id}/statements")]
    public async Task<IActionResult> AddStatement(int id, SavingStatement statement)
    {
        if (id != statement.SavingAccountId) return BadRequest();

        _db.SavingStatements.Add(statement);
        await _db.SaveChangesAsync();
        return Ok(statement);
    }
}