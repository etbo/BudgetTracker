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
                  // Le service donne la BDD choisie
                  _connectionString = dbSelector.CurrentDatabase switch
                  {
                        "Test" => "Data Source=Database/BudgetTrackerTest.db",
                        _ => "Data Source=Database/BudgetTracker.db"
                  };
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                  if (!optionsBuilder.IsConfigured)
                        optionsBuilder.UseSqlite(_connectionString);
            }

            public DbSet<OperationCC> Operations => Set<OperationCC>();

            public DbSet<AutoCategoryRule> AutoCategoryRules { get; set; }

            public DbSet<Category> Categories { get; set; } = null!;

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

      public class OperationCC
      {
            public int Id { get; set; }
            public string Date { get; set; } = string.Empty;
            public string? Description { get; set; }
            public double Montant { get; set; }
            public string? Categorie { get; set; }
            public string? Commentaire { get; set; }
            public string? Banque { get; set; }
            public string? DateImport { get; set; }
            public string? Hash { get; set; }
            public bool IsModified { get; set; } = false;
      }

      public class AutoCategoryRule
      {
            public int Id { get; set; }
            public string? Pattern { get; set; }
            public double? MinAmount { get; set; }
            public double? MaxAmount { get; set; }
            public DateTime? MinDate { get; set; }
            public DateTime? MaxDate { get; set; }
            public string? Category { get; set; } = "";
            public string? Commentaire { get; set; } = "";
      }
}
