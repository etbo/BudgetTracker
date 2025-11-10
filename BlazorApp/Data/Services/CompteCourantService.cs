using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Data.Services
{
    public class CompteCourantService
    {
        private readonly AppDbContext _context;

        public CompteCourantService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OperationCC>> GetAllAsync()
        {
            return await _context.Operations
                .OrderByDescending(c => c.Date)
                .ToListAsync();
        }

        public async Task<OperationCC?> GetByIdAsync(int id)
        {
            return await _context.Operations.FindAsync(id);
        }

        public async Task AddAsync(OperationCC item)
        {
            _context.Operations.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OperationCC item)
        {
            _context.Operations.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.Operations.FindAsync(id);

            if (item != null)
            {
                _context.Operations.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
