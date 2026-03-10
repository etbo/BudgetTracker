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
            // Arrange - Parents
            var accMain = new Account { Id = 1, Name = "Main", Owner = "Me", BankName = "B1", Type = AccountType.Checking };
            var savingAcc = new Account { Id = 2, Name = "Livret A", Owner = "Me", Type = AccountType.Savings };
            var liLine1 = new LifeInsuranceLine { Id = 1, AccountId = 1, Label = "Euro" };
            var liLine2 = new LifeInsuranceLine { Id = 2, AccountId = 1, Label = "SCPI" };
            
            _context.Accounts.AddRange(accMain, savingAcc);
            _context.LifeInsuranceLines.AddRange(liLine1, liLine2);

            // Arrange - Cash (Total = 150)
            _context.CcOperations.AddRange(
                new CcOperation { Id = 1, Date = DateTime.Now, Amount = 200, Description = "Salary" },
                new CcOperation { Id = 2, Date = DateTime.Now, Amount = -50, Description = "Groceries" }
            );

            // Arrange - Savings (Latest known = 500)
            _context.SavingStatements.AddRange(
                new SavingStatement { Id = 1, AccountId = 2, Date = DateTime.Now.AddMonths(-2), Amount = 400 },
                new SavingStatement { Id = 2, AccountId = 2, Date = DateTime.Now.AddMonths(-1), Amount = 500 }
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
            Assert.Equal(2050, summary.TotalGlobal);
        }

        [Fact]
        public async Task GetGlobalHistoryAsync_ShouldCalculateCorrectHistoricalPoints()
        {
            // Arrange - Parents
            _context.Accounts.Add(new Account { Id = 3, Name = "S1", Owner = "Me", Type = AccountType.Savings });

            var date1 = new DateTime(2024, 01, 15);
            var date2 = new DateTime(2024, 02, 15);

            // Cash: Month 1 (+100), Month 2 (+50) -> Totals: 100, 150
            _context.CcOperations.AddRange(
                new CcOperation { Id = 3, Date = date1, Amount = 100, Description = "S1" },
                new CcOperation { Id = 4, Date = date2, Amount = 50, Description = "S2" }
            );

            // Savings: Month 1 (200), Month 2 (350)
            _context.SavingStatements.AddRange(
                new SavingStatement { Id = 3, AccountId = 3, Date = date1, Amount = 200 },
                new SavingStatement { Id = 4, AccountId = 3, Date = date2, Amount = 350 }
            );

            // PEA: Month 1 (1 unit @ 10), Month 2 (1 unit @ 12) -> Totals: 10, 12
            _context.PeaOperations.Add(new PeaOperation { Id = 3, Date = date1, Code = "T1", Quantity = 1 });
            _context.PeaCachedStockPrices.AddRange(
                new PeaCachedStockPrice { Id = 3, Ticker = "T1", Date = date1, Price = 10 },
                new PeaCachedStockPrice { Id = 4, Ticker = "T1", Date = date2, Price = 12 }
            );

            await _context.SaveChangesAsync();

            var service = new PatrimonyService(_context);

            // Act
            var history = (await service.GetGlobalHistoryAsync()).ToList();

            // Assert
            Assert.True(history.Count >= 2);

            // Month 1 (Jan 2024)
            var p1 = history.First(h => h.Label == "01/2024");
            Assert.Equal(100, p1.Cash);
            Assert.Equal(200, p1.Savings);
            Assert.Equal(10, p1.Pea);

            // Month 2 (Feb 2024)
            var p2 = history.First(h => h.Label == "02/2024");
            Assert.Equal(150, p2.Cash);
            Assert.Equal(350, p2.Savings);
            Assert.Equal(12, p2.Pea);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Close();
        }
    }
}
