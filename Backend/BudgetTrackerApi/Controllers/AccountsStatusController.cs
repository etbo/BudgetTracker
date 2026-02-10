using BudgetTrackerApi.Data;
using BudgetTrackerApi.DTOs;
using BudgetTrackerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetTrackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountStatusController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AccountStatusController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("status")]
        public async Task<ActionResult<IEnumerable<AccountStatusDto>>> GetStatus()
        {
            var now = DateTime.Now;

            // 1. On récupère TOUS les comptes actifs avec leurs dernières dates de relevés/opérations
            var accountsData = await _db.Accounts
                .Where(a => a.IsActive)
                .Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.Owner,
                    a.Type,
                    a.UpdateFrequencyInMonths,
                    // Récupération de la dernière date selon le type
                    LastDate = a.Type == AccountType.Checking 
                        ? _db.CcOperations.Where(o => o.Bank == a.BankName).Max(o => (DateTime?)o.Date)
                        : a.Type == AccountType.Savings
                            ? _db.SavingStatements.Where(s => s.AccountId == a.Id).Max(s => (DateTime?)s.Date)
                            : _db.LifeInsuranceStatements.Where(s => s.Line.AccountId == a.Id).Max(s => (DateTime?)s.Date)
                })
                .ToListAsync();

            // 2. Transformation en DTO avec logique métier
            var globalStatus = accountsData.Select(a =>
            {
                // Calcul de la limite personnalisée basée sur la fréquence du compte
                // Si fréquence = 0 (ex pour CC), on peut mettre 1 mois par défaut
                int months = a.UpdateFrequencyInMonths > 0 ? a.UpdateFrequencyInMonths : 1;
                var limit = now.AddMonths(-months);

                bool isLate = a.LastDate == null || a.LastDate < limit;
                
                string typeLabel = a.Type switch {
                    AccountType.Checking => "Compte Courant",
                    AccountType.Savings => "Épargne",
                    AccountType.LifeInsurance => "Assurance Vie",
                    _ => "Autre"
                };

                string msg = "À jour";
                if (a.LastDate == null) msg = "Aucune donnée";
                else if (isLate) msg = $"Mise à jour requise (Fréq: {months} mois)";

                return new AccountStatusDto
                {
                    Owner = a.Owner,
                    AccountName = a.Name,
                    Type = typeLabel,
                    LastEntryDate = a.LastDate,
                    ActionRequired = isLate,
                    Message = msg
                };
            })
            .OrderByDescending(x => x.ActionRequired)
            .ThenBy(x => x.LastEntryDate)
            .ToList();

            return Ok(globalStatus);
        }
    }
}