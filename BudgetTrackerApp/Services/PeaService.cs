using System.Globalization;
using BudgetTrackerApp.Data;
using BudgetTrackerApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTrackerApp.Data.Services
{
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
            return await db.OperationsPea
                           .OrderBy(c => c.Date)
                           .ToListAsync();
        }

        public async Task<List<CumulPea>> CalculerCumul()
        {
            var serieCumuls = new List<CumulPea>();

            using var db = _dbFactory.CreateDbContext();
            
            var operations = await db.OperationsPea
                .ToListAsync();

            if (!operations.Any())
            {
                Console.WriteLine($"New DailyBalance");
                return new List<CumulPea>();
            }
            else
            {
                Console.WriteLine($"DailyBalance existant ({operations.Count()})");
            }


            // var orderedOperations = operations
            //     .Where(o => DateTime.TryParseExact(
            //         o.Date, 
            //         DateFormat, 
            //         CultureInfo.InvariantCulture, 
            //         DateTimeStyles.None, 
            //         out _))
            //     .OrderBy(o => DateTime.ParseExact(
            //         o.Date, 
            //         DateFormat, 
            //         CultureInfo.InvariantCulture))
            //     .ToList();






            serieCumuls.Add(new CumulPea(
                new DateTime(2025, 1, 1),
                1500.0,
                1800.50
            ));

            serieCumuls.Add(new CumulPea(
                new DateTime(2024, 1, 1),
                150.0,
                180.50
            ));

            serieCumuls.Add(new CumulPea(
                new DateTime(2023, 1, 1),
                150.0,
                500.50
            ));

            serieCumuls.Add(new CumulPea(
                new DateTime(2012, 1, 1),
                10.0,
                0.50
            ));

            return serieCumuls;
        }
    }
}