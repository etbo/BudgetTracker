using BudgetTrackerApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTrackerApp.Data.Services
{
    public class OperationCCService
    {
        private readonly AppDbContext _context;

        public OperationCCService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OperationCC>> GetAllAsync()
        {
            return await _context.OperationsCC
                .OrderByDescending(c => c.Date)
                .ToListAsync();
        }

        public async Task<OperationCC?> GetByIdAsync(int id)
        {
            return await _context.OperationsCC.FindAsync(id);
        }

        public async Task AddAsync(OperationCC item)
        {
            _context.OperationsCC.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OperationCC item)
        {
            _context.OperationsCC.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.OperationsCC.FindAsync(id);

            if (item != null)
            {
                _context.OperationsCC.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
