using BudgetTrackerApi.Data;
using BudgetTrackerApi.Models;
using BudgetTrackerApi.Models.LifeInsurance;
using BudgetTrackerApi.Models.Savings;
using BudgetTrackerApi.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BudgetTracker.Tests
{
    public class PatrimonyServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly SqliteConnection _connection;

        public PatrimonyServiceTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            var mockHttp = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var mockSelector = new Mock<DatabaseSelectorService>(mockHttp.Object);

            _context = new AppDbContext(options, mockSelector.Object);
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetCurrentSummaryAsync_ShouldCalculateCorrectTotals()
        {
            // Arrange - Cash (Total = 150)
            _context.CcOperations.AddRange(
                new CcOperation { Id = 1, Date = DateTime.Now, Amount = 200, Description = "Salary" },
                new CcOperation { Id = 2, Date = DateTime.Now, Amount = -50, Description = "Groceries" }
            );

            // Arrange - Savings (Latest known = 500)
            _context.SavingStatements.AddRange(
                new SavingStatement { Id = 1, AccountId = 1, Date = DateTime.Now.AddMonths(-2), Amount = 400 },
                new SavingStatement { Id = 2, AccountId = 1, Date = DateTime.Now.AddMonths(-1), Amount = 500 } // This one should be picked
            );

            // Arrange - Life Insurance (Latest known line 1 = 1000, line 2 = 300 -> Total 1300)
            _context.LifeInsuranceStatements.AddRange(
                new LifeInsuranceStatement { Id = 1, LifeInsuranceLineId = 1, Date = DateTime.Now.AddMonths(-2), UnitCount = 10, UnitValue = 90 },
                new LifeInsuranceStatement { Id = 2, LifeInsuranceLineId = 1, Date = DateTime.Now.AddMonths(-1), UnitCount = 10, UnitValue = 100 }, // 1000
                new LifeInsuranceStatement { Id = 3, LifeInsuranceLineId = 2, Date = DateTime.Now.AddMonths(-1), UnitCount = 5, UnitValue = 60 } // 300
            );

            // Arrange - PEA (CW8: 2 units. Latest price: 50 -> Total 100)
            _context.PeaOperations.AddRange(
                new PeaOperation { Id = 1, Date = DateTime.Now.AddMonths(-2), Code = "CW8", Quantity = 1 },
                new PeaOperation { Id = 2, Date = DateTime.Now.AddMonths(-1), Code = "CW8", Quantity = 1 }
            );
            _context.PeaCachedStockPrices.AddRange(
                new PeaCachedStockPrice { Id = 1, Ticker = "CW8", Date = DateTime.Now.AddMonths(-2), Price = 45 },
                new PeaCachedStockPrice { Id = 2, Ticker = "CW8", Date = DateTime.Now.AddMonths(-1), Price = 50 } // Latest price
            );

            await _context.SaveChangesAsync();

            var service = new PatrimonyService(_context);

            // Act
            var summary = await service.GetCurrentSummaryAsync();

            // Assert
            Assert.Equal(150, summary.Cash);
            Assert.Equal(500, summary.Savings);
            Assert.Equal(1300, summary.LifeInsurance);
            Assert.Equal(100, summary.Pea);
            
            // Total should be 150 + 500 + 1300 + 100 = 2050
            Assert.Equal(2050, summary.TotalGlobal);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Close();
        }
    }
}
