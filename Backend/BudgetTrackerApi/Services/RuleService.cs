using BudgetTrackerApi.Data;
using BudgetTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTrackerApi.Services
{
    // L'interface définit ce que le service sait faire
    public interface IRuleService
    {
        string GetAutoCategory(CcOperation op, List<CcCategoryRule> rules);
        Task<List<CcCategoryRule>> GetActiveRulesAsync();
    }

    // L'implémentation contient la logique
    public class RuleService : IRuleService
    {
        private readonly AppDbContext _db;

        public RuleService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CcCategoryRule>> GetActiveRulesAsync()
        {
            return await _db.CcCategoryRules
                .Where(r => r.IsUsed && !string.IsNullOrEmpty(r.Category))
                .ToListAsync();
        }

        public string GetAutoCategory(CcOperation op, List<CcCategoryRule> rules)
        {
            foreach (var r in rules)
            {
                if (string.IsNullOrEmpty(r.Category) || string.IsNullOrEmpty(r.Pattern))
                    continue;

                // 1. Vérification du Pattern (Description)
                if (string.IsNullOrEmpty(op.Description) ||
                    !op.Description.Contains(r.Pattern, StringComparison.OrdinalIgnoreCase))
                    continue;

                // 2. Vérification des Amounts (Min / Max)
                if (r.MinAmount.HasValue && op.Amount < (double)r.MinAmount.Value)
                    continue;

                if (r.MaxAmount.HasValue && op.Amount > (double)r.MaxAmount.Value)
                    continue;

                // 3. Vérification des Dates (Min / Max)

                if (r.MinDate.HasValue && op.Date < r.MinDate.Value)
                    continue;

                if (r.MaxDate.HasValue && op.Date > r.MaxDate.Value)
                    continue;

                // Si on arrive ici, c'est que la règle matche !
                return r.Category;
            }

            return "";
        }
    }
}