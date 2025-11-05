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
            return await _context.ComptesCourants
                .OrderByDescending(c => c.Date)
                .ToListAsync();
        }

        public async Task<OperationCC?> GetByIdAsync(int id)
        {
            return await _context.ComptesCourants.FindAsync(id);
        }

        public async Task AddAsync(OperationCC item)
        {
            _context.ComptesCourants.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OperationCC item)
        {
            _context.ComptesCourants.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.ComptesCourants.FindAsync(id);

            if (item != null)
            {
                _context.ComptesCourants.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
