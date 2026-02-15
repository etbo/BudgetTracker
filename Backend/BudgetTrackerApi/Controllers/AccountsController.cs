using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BudgetTrackerApi.Data;
using BudgetTrackerApi.Models;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AccountsController(AppDbContext context)
    {
        _db = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetAccounts()
    {
        var now = DateTime.Now;

        // 1. On charge les données de base
        var accounts = await _db.Accounts.ToListAsync();
        
        // Dates max pour les Comptes Courants (via le nom de la banque)
        var ccDates = await _db.CcOperations
            .Where(o => o.Bank != null)
            .GroupBy(o => o.Bank)
            .Select(g => new { BankName = g.Key, LastDate = g.Max(o => o.Date) })
            .ToListAsync();

        // 2. Projection finale
        var result = accounts.Select(a =>
        {
            DateTime? lastDate = null;

            if (a.Type == AccountType.Checking)
            {
                lastDate = ccDates.FirstOrDefault(d => d.BankName == a.BankName)?.LastDate;
            }
            else if (a.Type == AccountType.Savings)
            {
                lastDate = _db.SavingStatements
                    .Where(s => s.AccountId == a.Id)
                    .Max(s => (DateTime?)s.Date);
            }
            else if (a.Type == AccountType.LifeInsurance)
            {
                // Pour l'AV, on cherche la date max parmi tous les relevés de tous les fonds liés à ce compte
                lastDate = _db.LifeInsuranceStatements
                    .Where(s => s.Line.AccountId == a.Id)
                    .Max(s => (DateTime?)s.Date);
            }

            int months = a.UpdateFrequencyInMonths > 0 ? a.UpdateFrequencyInMonths : 1;
            bool isLate = a.IsActive && (lastDate == null || lastDate < now.AddMonths(-months));

            return new
            {
                a.Id,
                a.Name,
                a.Owner,
                a.BankName,
                a.Type,
                a.IsActive,
                a.UpdateFrequencyInMonths,
                LastEntryDate = lastDate,
                IsLate = isLate,
                StatusMessage = !a.IsActive ? "Désactivé" : 
                                (lastDate == null ? "Aucune donnée" : 
                                (isLate ? "MàJ requise" : "À jour"))
            };
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Account>> CreateAccount(Account account)
    {
        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAccounts), new { id = account.Id }, account);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAccount(int id, Account account)
    {
        if (id != account.Id) return BadRequest();

        _db.Entry(account).State = EntityState.Modified;

        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!_db.Accounts.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        var account = await _db.Accounts.FindAsync(id);
        if (account == null) return NotFound();

        _db.Accounts.Remove(account);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}