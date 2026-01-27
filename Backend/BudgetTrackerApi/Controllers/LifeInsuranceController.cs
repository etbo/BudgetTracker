using BudgetTrackerApi.Data;
using BudgetTrackerApi.DTOs;
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
        // 1. On cherche la date du relevé le plus récent pour ce contrat
        var lastDate = await _db.LifeInsuranceStatements
            .Where(s => s.Line.LifeInsuranceAccountId == accountId)
            .OrderByDescending(s => s.Date)
            .Select(s => (DateTime?)s.Date) // Cast en nullable pour éviter les erreurs si vide
            .FirstOrDefaultAsync();

        // 2. On projette les lignes avec cette date
        var lines = await _db.LifeInsuranceLines
            .Where(l => l.LifeInsuranceAccountId == accountId)
            .Select(l => new LifeInsuranceSaisieDto
            {
                LineId = l.Id,
                Label = l.Label,
                IsScpi = l.IsScpi,
                LastStatementDate = lastDate, // On injecte la date trouvée plus haut
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
    public async Task<IActionResult> SaveStatement(List<SaveStatementDto> dtos)
    {
        if (dtos == null || !dtos.Any()) return BadRequest("Aucune donnée à enregistrer.");

        var statements = dtos.Select(d => new LifeInsuranceStatement
        {
            LifeInsuranceLineId = d.LifeInsuranceLineId,
            Date = d.Date,
            UnitCount = d.UnitCount,
            UnitValue = d.UnitValue
        }).ToList();

        _db.LifeInsuranceStatements.AddRange(statements);
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("history/{accountId}")]
    public async Task<ActionResult> GetHistory(int accountId)
    {
        var query = _db.LifeInsuranceStatements
            .Include(s => s.Line)
            .ThenInclude(l => l.Account)
            .AsQueryable();

        // Si accountId > 0, on filtre. Si c'est 0, on prend tout.
        if (accountId > 0)
        {
            query = query.Where(s => s.Line.LifeInsuranceAccountId == accountId);
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
            // Clé de groupage : AccountId et Date complète
            GroupKey = $"{s.Line.LifeInsuranceAccountId}_{s.Date}"
        })
        .ToListAsync();

        return Ok(history);
    }

    [HttpGet("accounts")]
    public async Task<ActionResult> GetAccounts()
    {
        // Récupère tous les contrats avec leur Owner
        return Ok(await _db.LifeInsuranceAccounts
            .Select(a => new { a.Id, a.Name, a.Owner, a.IsActive, a.UpdateFrequencyInMonths })
            .ToListAsync());
    }

    [HttpPut("update-statement/{id}")]
    public async Task<IActionResult> UpdateStatement(int id, [FromBody] SaveStatementDto dto)
    {
        var statement = await _db.LifeInsuranceStatements.FindAsync(id);
        if (statement == null) return NotFound();

        // On met à jour uniquement les valeurs numériques
        statement.UnitCount = dto.UnitCount;
        statement.UnitValue = dto.UnitValue;
        // On peut aussi permettre la date si besoin : statement.Date = dto.Date;

        await _db.SaveChangesAsync();
        return Ok();
    }

    // POST: api/LifeInsurance/accounts
    [HttpPost("accounts")]
    public async Task<ActionResult> CreateAccount([FromBody] LifeInsuranceAccount account)
    {
        if (account == null) return BadRequest();

        _db.LifeInsuranceAccounts.Add(account);
        await _db.SaveChangesAsync();

        return Ok(account);
    }

    // PUT: api/LifeInsurance/accounts/{id}
    [HttpPut("accounts/{id}")]
    public async Task<IActionResult> UpdateAccount(int id, [FromBody] LifeInsuranceAccount updatedAccount)
    {
        var account = await _db.LifeInsuranceAccounts.FindAsync(id);
        if (account == null) return NotFound();

        // Mise à jour des champs éditables dans ta grille AG-Grid
        account.Name = updatedAccount.Name;
        account.Owner = updatedAccount.Owner;
        account.IsActive = updatedAccount.IsActive;
        account.UpdateFrequencyInMonths = updatedAccount.UpdateFrequencyInMonths;
        // Ajoute ici bankName ou Provider si tu as ajouté ces colonnes en BDD

        await _db.SaveChangesAsync();
        return Ok();
    }

    // DELETE: api/LifeInsurance/accounts/{id}
    [HttpDelete("accounts/{id}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        var account = await _db.LifeInsuranceAccounts.FindAsync(id);
        if (account == null) return NotFound();

        // Attention : la suppression peut échouer si des 'Lines' ou 'Statements' y sont liés.
        // Optionnel : Passer IsActive à false au lieu de supprimer (Soft Delete)
        _db.LifeInsuranceAccounts.Remove(account);
        await _db.SaveChangesAsync();
        return Ok();
    }
}