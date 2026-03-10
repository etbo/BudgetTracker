using BudgetTrackerApi.Models;
using BudgetTrackerApi.Services.Import;
using Xunit;

namespace BudgetTracker.Tests
{
    public class BankParserTests
    {
        [Fact]
        public void FortuneoCsvParser_ShouldParseValidCsv()
        {
            // Arrange
            var parser = new FortuneoCsvParser();
            var csvContent = "Date;Libellé;Description;Débit;Crédit;Commentaire\n" +
                             "30/10/2023;ACHAT CB;Courses Carrefour;-55,50;;";
            
            var ctx = new ParserInputContext
            {
                TextContent = csvContent,
                FileName = "fortuneo.csv"
            };

            // Act
            var result = parser.Parse(ctx);

            // Assert
            Assert.Single(result);
            var op = result[0];
            Assert.Equal(new DateTime(2023, 10, 30), op.Date);
            Assert.Equal("Courses Carrefour", op.Description);
            Assert.Equal(-55.50, op.Amount);
            Assert.Equal("Fortuneo", op.Bank);
        }

        [Fact]
        public void RevolutParser_ShouldParseValidCsv()
        {
            // Arrange
            var parser = new RevolutParser();
            
            // Format Revolut: Type(0), Product(1), Started Date(2), Completed Date(3), Description(4), Amount(5), Fee(6), Currency(7), State(8), Balance(9)
            // Note: RevolutParser uses ';' or ','
            var csvContent = "Type,Product,Started Date,Completed Date,Description,Amount,Fee,Currency,State,Balance\n" +
                             "CARD_PAYMENT,Current,2023-10-31 14:30:00,2023-10-31 15:00:00,Starbucks,-6.50,0.00,EUR,COMPLETED,1000.00";
            
            var ctx = new ParserInputContext
            {
                TextContent = csvContent,
                FileName = "revolut.csv"
            };

            // Act
            var result = parser.Parse(ctx);

            // Assert
            Assert.Single(result);
            var op = result[0];
            Assert.Equal(new DateTime(2023, 10, 31, 14, 30, 0), op.Date);
            Assert.Equal("Starbucks", op.Description);
            Assert.Equal(-6.50, op.Amount);
            Assert.Equal("Revolut", op.Bank);
        }

        [Fact]
        public void RevolutParser_ShouldThrowOnWrongCurrency()
        {
            // Arrange
            var parser = new RevolutParser();
            var csvContent = "Type,Product,Started Date,Completed Date,Description,Amount,Fee,Currency,State,Balance\n" +
                             "CARD_PAYMENT,Current,2023-10-31 14:30:00,2023-10-31 15:00:00,USD Purchase,-10.00,0.00,USD,COMPLETED,990.00";
            
            var ctx = new ParserInputContext
            {
                TextContent = csvContent,
                FileName = "revolut.csv"
            };

            // Act & Assert
            var ex = Assert.Throws<FormatException>(() => parser.Parse(ctx));
            Assert.Contains("Devise inconnue : 'USD'", ex.Message);
        }
    }
}
