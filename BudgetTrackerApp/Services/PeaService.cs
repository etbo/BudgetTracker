using BudgetTrackerApp.Data;
using BudgetTrackerApp.Models;
using Microsoft.EntityFrameworkCore;

public class PeaService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public PeaService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<OperationPea>> GetAllOperationsAsync()
    {
        using var db = _dbFactory.CreateDbContext();
        return await db.PEA
                       .OrderBy(c => c.Date)
                       .ToListAsync();
    }
}