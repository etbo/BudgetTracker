using BudgetTrackerApi.Data;
using Microsoft.EntityFrameworkCore;

public class CategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CcCategory>> GetAllCcCategoriesAsync()
    {
        return await _context.CcCategories
                       .OrderBy(c => c.Name)
                       .ToListAsync();
    }
}