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

    // GET: api/Accounts
    // Récupère TOUS les comptes (CC, Savings, AV)
    [HttpGet]
    public async Task<ActionResult> GetAccounts()
    {
        var now = DateTime.Now;
        var accounts = await _db.Accounts.ToListAsync();

        // On projette les données pour inclure le statut calculé
        var result = accounts.Select(a =>
        {
            DateTime? lastDate = a.Type switch
            {
                AccountType.Checking => _db.CcOperations.Where(o => o.Bank == a.BankName).Max(o => (DateTime?)o.Date),
                AccountType.Savings => _db.SavingStatements.Where(s => s.AccountId == a.Id).Max(s => (DateTime?)s.Date),
                AccountType.LifeInsurance => _db.LifeInsuranceStatements.Where(s => s.Line.AccountId == a.Id).Max(s => (DateTime?)s.Date),
                _ => null
            };

            int months = a.UpdateFrequencyInMonths > 0 ? a.UpdateFrequencyInMonths : 1;

            // LOGIQUE CORRIGÉE : Si inactif, IsLate est toujours false
            bool isLate = a.IsActive && (lastDate == null || lastDate < now.AddMonths(-months));

            string statusMessage = "À jour";
            if (!a.IsActive) statusMessage = "Désactivé";
            else if (lastDate == null) statusMessage = "Aucune donnée";
            else if (isLate) statusMessage = "MàJ requise";

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
                StatusMessage = statusMessage
            };
        });

        return Ok(result);
    }

    // POST: api/Accounts
    [HttpPost]
    public async Task<ActionResult<Account>> CreateAccount(Account account)
    {
        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAccounts), new { id = account.Id }, account);
    }

    // PUT: api/Accounts/{id}
    [HttpPut("{id}")]
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

    // DELETE: api/Accounts/{id}
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