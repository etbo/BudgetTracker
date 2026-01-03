using BudgetTrackerApp.Data;
using BudgetTrackerApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTrackerApp.Services
{
    // L'interface définit ce que le service sait faire
    public interface IRuleService
    {
        string GetAutoCategory(OperationCC op, List<CategoryRule> rules);
        Task<List<CategoryRule>> GetActiveRulesAsync();
    }

    // L'implémentation contient la logique
    public class RuleService : IRuleService
    {
        private readonly AppDbContext _db;

        public RuleService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CategoryRule>> GetActiveRulesAsync()
        {
            return await _db.CategoryRules
                .Where(r => r.IsUsed && !string.IsNullOrEmpty(r.Category))
                .ToListAsync();
        }

        public string GetAutoCategory(OperationCC op, List<CategoryRule> rules)
        {
            foreach (var r in rules)
            {
                if (string.IsNullOrEmpty(r.Category) || string.IsNullOrEmpty(r.Pattern))
                    continue;

                // 1. Vérification du Pattern (Description)
                if (string.IsNullOrEmpty(op.Description) ||
                    !op.Description.Contains(r.Pattern, StringComparison.OrdinalIgnoreCase))
                    continue;

                // 2. Vérification des Montants (Min / Max)
                if (r.MinAmount.HasValue && op.Montant < (double)r.MinAmount.Value)
                    continue;

                if (r.MaxAmount.HasValue && op.Montant > (double)r.MaxAmount.Value)
                    continue;

                // 3. Vérification des Dates (Min / Max)
                if (!string.IsNullOrEmpty(op.Date) && DateTime.TryParse(op.Date, out DateTime opDate))
                {
                    if (r.MinDate.HasValue && opDate < r.MinDate.Value)
                        continue;

                    if (r.MaxDate.HasValue && opDate > r.MaxDate.Value)
                        continue;
                }

                // Si on arrive ici, c'est que la règle matche !
                return r.Category;
            }

            return "";
        }
    }
}