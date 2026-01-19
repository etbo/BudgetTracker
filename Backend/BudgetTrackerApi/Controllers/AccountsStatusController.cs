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
                .GroupBy(o => o.Banque)
                .Select(g => new AccountStatusDto
                {
                    Owner = "Joint",
                    AccountName = g.Key ?? "Inconnu",
                    Type = "Compte Courant",
                    LastEntryDate = g.Max(o => o.Date),
                    ActionRequired = g.Max(o => o.Date) < defaultLimit,
                    Message = g.Max(o => o.Date) < defaultLimit ? "Import mensuel requis" : "À jour"
                }).ToListAsync();

            // 2. Statut des Livrets avec Fréquence Personnalisée
            // On récupère d'abord les infos des comptes et la date max des statements
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

            // 3. Calcul de l'alerte en fonction de la fréquence de chaque livret
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

            // 4. Fusion et tri final
            // On trie d'abord par ActionRequired (les alertes en haut)
            // Puis par Date (les plus vieux en premier pour les alertes)
            var globalStatus = ccStatus.Concat(savingsStatus)
                .OrderByDescending(x => x.ActionRequired)
                .ThenBy(x => x.LastEntryDate) 
                .ThenBy(x => x.AccountName);

            return Ok(globalStatus);
        }
    }
}