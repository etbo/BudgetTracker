using BudgetTrackerApi.Data;
using BudgetTrackerApi.Models;
using Microsoft.EntityFrameworkCore;
using BudgetTrackerApi.DTOs;

namespace BudgetTrackerApi.Services
{
    public class DatabaseHealthService
    {
        private readonly AppDbContext _context;

        public DatabaseHealthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DatabaseHealthReportDto> GetDatabaseHealthAsync(CancellationToken cancellationToken = default)
        {
            var report = new DatabaseHealthReportDto();

            // 1. Check for Missing Months (Checking accounts)
            var checkingAccounts = await _context.Accounts
                .Where(a => a.Type == AccountType.Checking && a.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var account in checkingAccounts)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var operations = await _context.CcOperations
                    .Where(o => o.Bank == account.BankName)
                    .Select(o => o.Date)
                    .ToListAsync(cancellationToken);

                if (operations.Any())
                {
                    var minDate = operations.Min();
                    var maxDate = operations.Max();

                    var missing = new List<string>();
                    var current = new DateTime(minDate.Year, minDate.Month, 1);
                    var end = new DateTime(maxDate.Year, maxDate.Month, 1);

                    while (current <= end)
                    {
                        var hasData = operations.Any(d => d.Year == current.Year && d.Month == current.Month);
                        if (!hasData)
                        {
                            missing.Add(current.ToString("yyyy-MM"));
                        }
                        current = current.AddMonths(1);
                    }

                    if (missing.Any())
                    {
                        report.MissingMonths.Add(new AccountMissingMonthsDto
                        {
                            AccountId = account.Id,
                            AccountName = account.Name,
                            MissingMonths = missing
                        });
                    }
                }
            }

            // 2. Check for Unknown Categories
            var existingCategories = await _context.CcCategories
                .Select(c => c.Name)
                .ToListAsync(cancellationToken);

            // On récupère d'abord toutes les catégories utilisées depuis la BDD
            var usedCategories = await _context.CcOperations
                .Where(o => !string.IsNullOrEmpty(o.Category))
                .GroupBy(o => o.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            // On filtre ensuite côté C# pour éviter les problèmes de traduction EF Core/SQLite
            var unknownCats = usedCategories
                .Where(g => !existingCategories.Contains(g.Category))
                .Select(g => new UnknownCategoryDto
                {
                    CategoryName = g.Category ?? "Inconnue",
                    OperationCount = g.Count
                })
                .ToList();

            report.UnknownCategories = unknownCats;

            return report;
        }
    }
}
