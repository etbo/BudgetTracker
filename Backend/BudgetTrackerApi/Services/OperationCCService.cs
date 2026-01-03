using BudgetTrackerApp.Data;
using BudgetTrackerApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTrackerApp.Services
{
    public class CcOperationService
    {
        private readonly AppDbContext _context;

        public CcOperationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CcOperation>> GetAllAsync()
        {
            return await _context.CcOperations
                .OrderByDescending(c => c.Date)
                .ToListAsync();
        }

        public async Task<CcOperation?> GetByIdAsync(int id)
        {
            return await _context.CcOperations.FindAsync(id);
        }

        public async Task AddAsync(CcOperation item)
        {
            _context.CcOperations.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CcOperation item)
        {
            _context.CcOperations.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.CcOperations.FindAsync(id);

            if (item != null)
            {
                _context.CcOperations.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
