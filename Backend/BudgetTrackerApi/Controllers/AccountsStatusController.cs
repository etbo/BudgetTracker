using BudgetTrackerApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetTrackerApi.Controllers // Assure-toi d'avoir un namespace
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountStatusController : ControllerBase
    {
        private readonly AppDbContext _db;

        // UN SEUL constructeur ici
        public AccountStatusController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("status")]
        public async Task<ActionResult<IEnumerable<AccountStatusDto>>> GetStatus()
        {
            var now = DateTime.Now;
            var lastMonthLimit = new DateTime(now.Year, now.Month, 1).AddDays(-1);

            // 1. Statut des Comptes Courants (inchangé)
            var ccStatus = await _db.CcOperations
                .GroupBy(o => o.Banque)
                .Select(g => new AccountStatusDto
                {
                    AccountName = g.Key ?? "Inconnu",
                    Type = "Compte Courant",
                    LastEntryDate = g.Max(o => o.Date),
                    ActionRequired = g.Max(o => o.Date) < lastMonthLimit,
                    Message = g.Max(o => o.Date) < lastMonthLimit ? "Import mensuel requis" : "À jour"
                }).ToListAsync();

            // 2. Statut des Livrets (SavingAccounts + SavingStatements)
            // On part des comptes actifs
            var savingsStatus = await _db.SavingAccounts
                .Where(s => s.IsActive)
                .Select(s => new AccountStatusDto
                {
                    AccountName = $"{s.Name} ({s.Owner})", // Adapte selon ton champ (ex: s.Label ou s.Name)
                    Type = "Épargne",
                    // On cherche la date max dans la table des relevés pour ce livret précis
                    LastEntryDate = _db.SavingStatements
                        .Where(st => st.SavingAccountId == s.Id) // Jointure sur l'ID du livret
                        .Max(st => (DateTime?)st.Date),

                    // La logique de calcul sera finalisée en mémoire juste après
                    ActionRequired = false,
                    Message = ""
                }).ToListAsync();

            // 3. Post-traitement pour les livrets (gestion des dates nulles et alertes)
            foreach (var s in savingsStatus)
            {
                if (s.LastEntryDate == null)
                {
                    s.ActionRequired = true;
                    s.Message = "Aucune donnée saisie";
                }
                else
                {
                    s.ActionRequired = s.LastEntryDate < lastMonthLimit;
                    s.Message = s.ActionRequired ? "Saisie solde requise" : "À jour";
                }
            }

            // 4. Fusion et tri
            var globalStatus = ccStatus.Concat(savingsStatus)
                .OrderByDescending(x => x.ActionRequired)
                .ThenBy(x => x.AccountName);

            return Ok(globalStatus);
        }
    }
}