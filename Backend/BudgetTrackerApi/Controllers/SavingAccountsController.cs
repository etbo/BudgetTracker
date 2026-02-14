using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BudgetTrackerApi.Data;
using BudgetTrackerApi.Models; // Pour accéder à Account et AccountType
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
    // On ne récupère que les comptes dont le type est "Savings"
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
    {
        return await _db.Accounts
            .Where(a => a.Type == AccountType.Savings) // Filtrage par type
            .Include(a => a.SavingStatements)
            .ToListAsync();
    }

    // POST: api/SavingAccounts
    [HttpPost]
    public async Task<ActionResult<Account>> CreateAccount(Account account)
    {
        // On force le type au cas où le front ne l'envoie pas
        account.Type = AccountType.Savings;

        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAccounts), new { id = account.Id }, account);
    }

    // POST: api/SavingAccounts/{id}/statements
    [HttpPost("{id}/statements")]
    public async Task<IActionResult> AddStatement(int id, SavingStatement statement)
    {
        // On force l'ID de l'URL dans l'objet pour éviter les erreurs de mapping
        statement.AccountId = id;

        // On peut retirer la vérification stricte 'if (id != statement.AccountId)' 
        // car on vient de forcer la valeur juste au-dessus.

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _db.SavingStatements.Add(statement);
        await _db.SaveChangesAsync();
        return Ok(statement);
    }

    // PUT: api/SavingAccounts/accounts/{id}
    [HttpPut("accounts/{id}")]
    public async Task<IActionResult> UpdateAccount(int id, Account account)
    {
        if (id != account.Id) return BadRequest();

        _db.Entry(account).State = EntityState.Modified;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_db.Accounts.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    // PUT: api/SavingAccounts/{accountId}/statements/{id}
    [HttpPut("{accountId}/statements/{id}")]
    public async Task<IActionResult> UpdateStatement(int accountId, int id, SavingStatement statement)
    {
        if (id != statement.Id) return BadRequest();

        _db.Entry(statement).State = EntityState.Modified;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_db.SavingStatements.Any(e => e.Id == id)) return NotFound();
            else throw;
        }

        return NoContent();
    }
}