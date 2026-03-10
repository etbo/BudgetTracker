using BudgetTrackerApi.Models;
using BudgetTrackerApi.Models.Savings;
using BudgetTrackerApi.Models.LifeInsurance;
using BudgetTrackerApi.Data;
using BudgetTrackerApi.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BudgetTracker.Tests
{
    public class AccountServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly SqliteConnection _connection;

        public AccountServiceTests()
        {
            // On utilise une base de données SQLite en mémoire pour les tests
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            // Mock du sélecteur de base de données
            var mockHttp = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var mockSelector = new Mock<DatabaseSelectorService>(mockHttp.Object);

            _context = new AppDbContext(options, mockSelector.Object);
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetAllAccountSummariesAsync_ShouldReturnAccounts()
        {
            // Arrange
            _context.Accounts.Add(new Account { 
                Id = 1, 
                Name = "Test Account", 
                Owner = "Test Owner",
                Type = AccountType.Checking, 
                IsActive = true,
                BankName = "Test Bank"
            });
            
            // On ajoute une opération récente pour que le compte soit "À jour"
            _context.CcOperations.Add(new CcOperation {
                Id = 1,
                Bank = "Test Bank",
                Date = DateTime.Now,
                Description = "Initial balance",
                Amount = 0
            });

            await _context.SaveChangesAsync();

            var service = new AccountService(_context);

            // Act
            var result = await service.GetAllAccountSummariesAsync();

            // Assert
            var list = result.ToList();
            Assert.Single(list);
            Assert.Equal("Test Account", list[0].Name);
            Assert.False(list[0].IsLate);
        }

        [Fact]
        public async Task GetAllAccountSummariesAsync_ShouldMarkAccountAsLate()
        {
            // Arrange
            var now = DateTime.Now;
            var accountId = 2;
            
            _context.Accounts.Add(new Account { 
                Id = accountId, 
                Name = "Late Savings", 
                Owner = "User",
                Type = AccountType.Savings, 
                IsActive = true,
                UpdateFrequencyInMonths = 1
            });

            // Ajout d'un relevé datant de 3 mois (pour une fréquence de 1 mois)
            _context.SavingStatements.Add(new SavingStatement { 
                Id = 1, 
                AccountId = accountId, 
                Date = now.AddMonths(-3), 
                Amount = 1000 
            });
            
            await _context.SaveChangesAsync();

            var service = new AccountService(_context);

            // Act
            var result = await service.GetAllAccountSummariesAsync();

            // Assert
            var summary = result.First(a => a.Id == accountId);
            Assert.True(summary.IsLate);
            Assert.Equal("MàJ requise", summary.StatusMessage);
        }

        [Fact]
        public async Task GetAllAccountSummariesAsync_ShouldPickMostRecentDate()
        {
            // Arrange
            var now = DateTime.Now;
            var accountId = 3;
            
            _context.Accounts.Add(new Account { 
                Id = accountId, 
                Name = "Multi Entry Savings", 
                Owner = "User",
                Type = AccountType.Savings, 
                IsActive = true,
                UpdateFrequencyInMonths = 1
            });

            // On ajoute deux relevés : un vieux et un récent
            _context.SavingStatements.AddRange(
                new SavingStatement { Id = 10, AccountId = accountId, Date = now.AddMonths(-5), Amount = 500 },
                new SavingStatement { Id = 11, AccountId = accountId, Date = now.AddDays(-2), Amount = 600 }
            );
            
            await _context.SaveChangesAsync();

            var service = new AccountService(_context);

            // Act
            var result = await service.GetAllAccountSummariesAsync();

            // Assert
            var summary = result.First(a => a.Id == accountId);
            Assert.False(summary.IsLate);
            Assert.Equal("À jour", summary.StatusMessage);
            Assert.Equal(now.AddDays(-2).Date, summary.LastEntryDate?.Date);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Close();
        }
    }
}
