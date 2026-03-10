using BudgetTrackerApi.Data;
using BudgetTrackerApi.DTOs;
using BudgetTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTrackerApi.Services
{
    public class AccountService
    {
        private readonly AppDbContext _db;

        public AccountService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<AccountSummaryDto>> GetAllAccountSummariesAsync()
        {
            var now = DateTime.Now;

            // 1. On charge les données de base
            var accounts = await _db.Accounts.ToListAsync();
            
            // Dates max pour les Comptes Courants
            var ccDates = await _db.CcOperations
                .Where(o => o.Bank != null)
                .GroupBy(o => o.Bank)
                .Select(g => new { BankName = g.Key, LastDate = g.Max(o => o.Date) })
                .ToListAsync();

            // Dates max pour les comptes d'Épargne
            var savingDates = await _db.SavingStatements
                .GroupBy(s => s.AccountId)
                .Select(g => new { AccountId = g.Key, LastDate = g.Max(s => (DateTime?)s.Date) })
                .ToDictionaryAsync(x => x.AccountId, x => x.LastDate);

            // Dates max pour l'Assurance Vie
            var lifeInsuranceDates = await _db.LifeInsuranceStatements
                .GroupBy(s => s.Line.AccountId)
                .Select(g => new { AccountId = g.Key, LastDate = g.Max(s => (DateTime?)s.Date) })
                .ToDictionaryAsync(x => x.AccountId, x => x.LastDate);

            // 2. Projection finale
            return accounts.Select(a =>
            {
                DateTime? lastDate = null;

                if (a.Type == AccountType.Checking)
                {
                    lastDate = ccDates.FirstOrDefault(d => d.BankName == a.BankName)?.LastDate;
                }
                else if (a.Type == AccountType.Savings)
                {
                    savingDates.TryGetValue(a.Id, out lastDate);
                }
                else if (a.Type == AccountType.LifeInsurance)
                {
                    lifeInsuranceDates.TryGetValue(a.Id, out lastDate);
                }

                int months = a.UpdateFrequencyInMonths > 0 ? a.UpdateFrequencyInMonths : 1;
                bool isLate = a.IsActive && (lastDate == null || lastDate < now.AddMonths(-months));

                return new AccountSummaryDto(
                    a.Id,
                    a.Name,
                    a.Owner,
                    a.BankName,
                    a.Type,
                    a.IsActive,
                    a.UpdateFrequencyInMonths,
                    lastDate,
                    isLate,
                    !a.IsActive ? "Désactivé" : 
                    (lastDate == null ? "Aucune donnée" : 
                    (isLate ? "MàJ requise" : "À jour"))
                );
            });
        }
    }
}
