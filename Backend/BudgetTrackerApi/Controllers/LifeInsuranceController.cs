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
                AccountInfo = $"{s.Line.Account.Name} - {s.Line.Account.Owner}"
            })
            .ToListAsync();

        return Ok(history);
    }

    [HttpGet("accounts")]
    public async Task<ActionResult> GetAccounts()
    {
        // Récupère tous les contrats avec leur Owner
        return Ok(await _db.LifeInsuranceAccounts
            .Where(a => a.IsActive)
            .Select(a => new { a.Id, a.Name, a.Owner })
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
}