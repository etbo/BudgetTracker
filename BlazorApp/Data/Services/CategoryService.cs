using BlazorApp.Data;
using Microsoft.EntityFrameworkCore;

public class CategoryService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public CategoryService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        using var db = _dbFactory.CreateDbContext();
        return await db.Categories
                       .OrderBy(c => c.Name)
                       .ToListAsync();
    }
}