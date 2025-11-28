using BudgetTrackerApp.Models;
using BudgetTrackerApp.Services;
using Microsoft.EntityFrameworkCore;

namespace BudgetTrackerApp.Data
{
      public class AppDbContext : DbContext
      {
            private readonly string _connectionString;

            public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration config, DatabaseSelectorService dbSelector)
        : base(options)
            {
                  var dataDir = Path.GetFullPath(Path.Combine("..", "Database"));

                  _connectionString = dbSelector.CurrentDatabase switch
                  {
                        "Test" => $"Data Source={Path.Combine(dataDir, "BudgetTrackerTest.db")}",
                        _ => $"Data Source={Path.Combine(dataDir, "BudgetTracker.db")}"
                  };
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                  if (!optionsBuilder.IsConfigured)
                        optionsBuilder.UseSqlite(_connectionString);
            }

            public DbSet<OperationCC> OperationsCC => Set<OperationCC>();

            public DbSet<CategoryRule> CategoryRules { get; set; }

            public DbSet<Category> Categories { get; set; } = null!;

            public DbSet<OperationPea> OperationsPea { get; set; } = null!;
            public DbSet<CachedStockPrice> CachedStockPrices { get; set; } = null!;

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                  base.OnModelCreating(modelBuilder);

                  modelBuilder.Entity<OperationCC>(entity =>
                  {
                        entity.ToTable("CompteCourant");

                        // Clé primaire
                        entity.HasKey(e => e.Id);

                        entity.Property(e => e.Id)
                        .HasColumnName("Id");

                        entity.Property(e => e.Date)
                        .HasColumnName("Date")
                        .IsRequired();

                        entity.Property(e => e.Description)
                        .HasColumnName("Description");

                        entity.Property(e => e.Montant)
                        .HasColumnName("Montant")
                        .IsRequired();

                        entity.Property(e => e.Categorie)
                        .HasColumnName("Categorie");

                        entity.Property(e => e.Commentaire)
                        .HasColumnName("Commentaire");

                        entity.Property(e => e.Banque)
                        .HasColumnName("Banque");

                        entity.Property(e => e.DateImport)
                        .HasColumnName("DateImport");

                        entity.Property(e => e.Hash)
                        .HasColumnName("Hash");

                        entity.Ignore(e => e.IsModified);
                  });
            }
      }
}
