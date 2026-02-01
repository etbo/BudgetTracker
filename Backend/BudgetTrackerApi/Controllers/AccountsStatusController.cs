using BudgetTrackerApi.Data;
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
            // Limite standard pour les Comptes Courants (mois dernier)
            var defaultLimit = new DateTime(now.Year, now.Month, 1).AddDays(-1);

            // 1. Statut des Comptes Courants (Fréquence fixe : 1 mois)
            var ccStatus = await _db.CcOperations
                .GroupBy(o => o.Bank)
                .Select(g => new AccountStatusDto
                {
                    Owner = "Joint",
                    AccountName = g.Key ?? "Inconnu",
                    Type = "Compte Courant",
                    LastEntryDate = g.Max(o => o.Date),
                    ActionRequired = g.Max(o => o.Date) < defaultLimit,
                    Message = g.Max(o => o.Date) < defaultLimit ? "Import mensuel requis" : "À jour"
                }).ToListAsync();

            // 2. Statut des Livrets (Fréquence variable)
            var savingsData = await _db.SavingAccounts
                .Where(s => s.IsActive)
                .Select(s => new
                {
                    s.Name,
                    s.UpdateFrequencyInMonths,
                    s.Id,
                    s.Owner,
                    LastDate = _db.SavingStatements
                        .Where(st => st.SavingAccountId == s.Id)
                        .Max(st => (DateTime?)st.Date)
                }).ToListAsync();

            var savingsStatus = savingsData.Select(s =>
            {
                // Calcul de la limite : Date du jour MOINS X mois de fréquence
                // Si fréquence = 12 (annuel), la limite est il y a un an
                var customLimit = now.AddMonths(-s.UpdateFrequencyInMonths);

                bool isLate = s.LastDate == null || s.LastDate < customLimit;
                string msg = "À jour";

                if (s.LastDate == null) msg = "Aucune donnée";
                else if (isLate) msg = $"Saisie requise (tous les {s.UpdateFrequencyInMonths} mois)";

                return new AccountStatusDto
                {
                    Owner = s.Owner,
                    AccountName = s.Name,
                    Type = "Épargne",
                    LastEntryDate = s.LastDate,
                    ActionRequired = isLate,
                    Message = msg
                };
            }).ToList();

            // 3. Statut des AV (Fréquence fixe : 3 mois)
            var liFrequency = 3;
            var liLimit = now.AddMonths(-liFrequency);

            var liData = await _db.LifeInsuranceAccounts
                .Where(a => a.IsActive)
                .Select(a => new
                {
                    a.Name,
                    a.Owner,
                    a.Id,
                    LastDate = _db.LifeInsuranceStatements
                        .Where(s => s.Line.LifeInsuranceAccountId == a.Id)
                        .Max(s => (DateTime?)s.Date)
                }).ToListAsync();

            var liStatus = liData.Select(a =>
            {
                bool isLate = a.LastDate == null || a.LastDate < liLimit;
                return new AccountStatusDto
                {
                    Owner = a.Owner,
                    AccountName = a.Name,
                    Type = "Assurance Vie",
                    LastEntryDate = a.LastDate,
                    ActionRequired = isLate,
                    Message = a.LastDate == null ? "Aucune donnée" : (isLate ? "Mise à jour trimestrielle requise" : "À jour")
                };
            }).ToList();

            // 4. Fusion et tri final
            // On trie d'abord par ActionRequired (les alertes en haut)
            // Puis par Date (les plus vieux en premier pour les alertes)
            var globalStatus = ccStatus.Concat(savingsStatus).Concat(liStatus)
                .OrderByDescending(x => x.ActionRequired)
                .ThenBy(x => x.LastEntryDate)
                .ThenBy(x => x.AccountName);

            return Ok(globalStatus);
        }
    }
}