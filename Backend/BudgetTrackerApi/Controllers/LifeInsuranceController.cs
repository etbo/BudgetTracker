using BudgetTrackerApi.Data;
using BudgetTrackerApi.DTOs;
using BudgetTrackerApi.Models;
using BudgetTrackerApi.Models.LifeInsurance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class LifeInsuranceController : ControllerBase
{
    private readonly AppDbContext _db;

    public LifeInsuranceController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("last-values/{accountId}")]
    public async Task<ActionResult> GetLastValues(int accountId)
    {
        // 1. On cherche la date du relevé le plus récent (via AccountId)
        var lastDate = await _db.LifeInsuranceStatements
            .Where(s => s.Line.AccountId == accountId)
            .OrderByDescending(s => s.Date)
            .Select(s => (DateTime?)s.Date)
            .FirstOrDefaultAsync();

        // 2. On projette les lignes (via AccountId)
        var lines = await _db.LifeInsuranceLines
            .Where(l => l.AccountId == accountId)
            .Select(l => new LifeInsuranceSaisieDto
            {
                LineId = l.Id,
                Label = l.Label,
                IsScpi = l.IsScpi,
                LastStatementDate = lastDate,
                LastUnitCount = _db.LifeInsuranceStatements
                    .Where(s => s.LifeInsuranceLineId == l.Id)
                    .OrderByDescending(s => s.Date)
                    .Select(s => s.UnitCount)
                    .FirstOrDefault(),
                LastUnitValue = _db.LifeInsuranceStatements
                    .Where(s => s.LifeInsuranceLineId == l.Id)
                    .OrderByDescending(s => s.Date)
                    .Select(s => s.UnitValue)
                    .FirstOrDefault()
            }).ToListAsync();

        return Ok(lines);
    }

    [HttpPost("save-statement")]
    public async Task<IActionResult> SaveStatement([FromBody] GlobalSaveStatementDto dto)
    {
        if (dto == null || !dto.Items.Any()) return BadRequest("Aucune donnée à enregistrer.");

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            foreach (var item in dto.Items)
            {
                int lineId = item.LifeInsuranceLineId;

                if (lineId <= 0)
                {
                    var newLine = new LifeInsuranceLine
                    {
                        AccountId = dto.AccountId, // Renommé
                        Label = item.Label,
                        IsScpi = item.IsScpi
                    };
                    _db.LifeInsuranceLines.Add(newLine);
                    await _db.SaveChangesAsync();
                    lineId = newLine.Id;
                }

                var statement = new LifeInsuranceStatement
                {
                    LifeInsuranceLineId = lineId,
                    Date = dto.Date,
                    UnitCount = item.UnitCount,
                    UnitValue = item.UnitValue
                };
                _db.LifeInsuranceStatements.Add(statement);
            }

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Erreur : {ex.Message}");
        }
    }

    [HttpGet("history/{accountId}")]
    public async Task<ActionResult> GetHistory(int accountId)
    {
        var query = _db.LifeInsuranceStatements
            .Include(s => s.Line)
            .ThenInclude(l => l.Account)
            .AsQueryable();

        if (accountId > 0)
        {
            query = query.Where(s => s.Line.AccountId == accountId);
        }

        var history = await query
            .OrderByDescending(s => s.Date)
            .Select(s => new
            {
                Id = s.Id,
                Date = s.Date,
                UnitCount = s.UnitCount,
                UnitValue = s.UnitValue,
                LineLabel = s.Line.Label,
                IsScpi = s.Line.IsScpi,
                AccountName = s.Line.Account.Name,
                AccountOwner = s.Line.Account.Owner,
                GroupKey = $"{s.Line.AccountId}_{s.Date:yyyy-MM-dd}"
            })
            .ToListAsync();

        return Ok(history);
    }

    [HttpGet("accounts")]
    public async Task<ActionResult> GetAccounts()
    {
        // Filtre uniquement sur le type LifeInsurance
        return Ok(await _db.Accounts
            .Where(a => a.Type == AccountType.LifeInsurance)
            .Select(a => new { a.Id, a.Name, a.Owner, a.IsActive, a.UpdateFrequencyInMonths })
            .ToListAsync());
    }

    [HttpPost("accounts")]
    public async Task<ActionResult> CreateAccount([FromBody] Account account)
    {
        if (account == null) return BadRequest();

        account.Type = AccountType.LifeInsurance; // On force le type
        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();
        return Ok(account);
    }

    [HttpPut("accounts/{id}")]
    public async Task<IActionResult> UpdateAccount(int id, [FromBody] Account updatedAccount)
    {
        var account = await _db.Accounts.FindAsync(id);
        if (account == null) return NotFound();

        account.Name = updatedAccount.Name;
        account.Owner = updatedAccount.Owner;
        account.IsActive = updatedAccount.IsActive;
        account.UpdateFrequencyInMonths = updatedAccount.UpdateFrequencyInMonths;
        account.BankName = updatedAccount.BankName; // Ajouté pour la cohérence

        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("accounts/{id}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        var account = await _db.Accounts.FindAsync(id);
        if (account == null) return NotFound();

        _db.Accounts.Remove(account);
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("history/group/{groupKey}")]
    public async Task<IActionResult> DeleteHistoryGroup(string groupKey)
    {
        // On décode la clé (Ex: "4_20/02/2026 00:00:00")
        var parts = groupKey.Split('_');
        if (parts.Length < 2) return BadRequest("Format de clé invalide.");

        int accountId = int.Parse(parts[0]);
        DateTime date = DateTime.Parse(parts[1]);

        // On supprime tous les relevés du compte à cette date précise
        var statementsToDelete = await _db.LifeInsuranceStatements
            .Where(s => s.Line.AccountId == accountId && s.Date == date)
            .ToListAsync();

        if (!statementsToDelete.Any()) return NotFound("Aucun relevé trouvé pour ce groupe.");

        _db.LifeInsuranceStatements.RemoveRange(statementsToDelete);
        await _db.SaveChangesAsync();

        return Ok();
    }

    // PUT: api/LifeInsurance/history/group/date
    [HttpPut("history/group/date")]
    public async Task<IActionResult> UpdateGroupDate([FromBody] UpdateGroupDateDto dto)
    {
        if (dto == null || string.IsNullOrEmpty(dto.GroupKey)) return BadRequest();

        var parts = dto.GroupKey.Split('_');
        int accountId = int.Parse(parts[0]);

        // On parse la date provenant de la clé (format yyyy-MM-dd maintenant)
        if (!DateTime.TryParse(parts[1], out DateTime oldDate))
            return BadRequest("Date source invalide");

        // Important : .Date permet d'ignorer les heures/minutes/secondes si besoin
        var statements = await _db.LifeInsuranceStatements
            .Where(s => s.Line.AccountId == accountId && s.Date.Date == oldDate.Date)
            .ToListAsync();

        if (!statements.Any()) return NotFound("Aucun relevé trouvé.");

        foreach (var s in statements)
        {
            s.Date = dto.NewDate.Date; // On stocke la nouvelle date (sans l'heure UTC)
        }

        await _db.SaveChangesAsync();
        return Ok();
    }
}