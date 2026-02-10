using BudgetTrackerApi.Models;
using BudgetTrackerApi.Models.LifeInsurance;
using BudgetTrackerApi.Models.Savings;
using BudgetTrackerApi.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BudgetTrackerApi.Data
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

        // --- Table Unifiée des Comptes ---
        public DbSet<Account> Accounts { get; set; } = null!;

        // --- Détails des Comptes ---
        public DbSet<CcOperation> CcOperations => Set<CcOperation>();
        public DbSet<CcCategoryRule> CcCategoryRules { get; set; } = null!;
        public DbSet<CcCategory> CcCategories { get; set; } = null!;
        public DbSet<CcImportLog> CcImportLogs { get; set; } = null!;
        
        public DbSet<SavingStatement> SavingStatements { get; set; } = null!;
        
        public DbSet<LifeInsuranceLine> LifeInsuranceLines { get; set; } = null!;
        public DbSet<LifeInsuranceStatement> LifeInsuranceStatements { get; set; } = null!;
        
        public DbSet<PeaOperation> PeaOperations { get; set; } = null!;
        public DbSet<PeaCachedStockPrice> PeaCachedStockPrices { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Application automatique du snake_case
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (tableName != null)
                {
                    entity.SetTableName(ToSnakeCase(tableName));
                }

                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToSnakeCase(property.Name));
                }
            }

            // 2. Ajustements manuels et Relations
            
            // Relation SavingStatements -> Account
            modelBuilder.Entity<SavingStatement>(entity =>
            {
                entity.HasOne(s => s.Account)
                      .WithMany(a => a.SavingStatements)
                      .HasForeignKey(s => s.AccountId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Relation LifeInsuranceLines -> Account
            modelBuilder.Entity<LifeInsuranceLine>(entity =>
            {
                entity.HasOne(l => l.Account)
                      .WithMany(a => a.LifeInsuranceLines)
                      .HasForeignKey(l => l.AccountId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CcOperation>().ToTable("cc_operations");

            modelBuilder.Entity<PeaOperation>(entity =>
            {
                entity.ToTable("pea_operations");
                entity.Property(e => e.Quantity).HasColumnName("quantite");
            });

            // 3. Seed Data des Catégories
            modelBuilder.Entity<CcCategory>().HasData(
                new CcCategory { Id = 1, Name = "Prêt", MacroCategory = "Obligatoire" },
                new CcCategory { Id = 2, Name = "Courses", MacroCategory = "Obligatoire" },
                new CcCategory { Id = 3, Name = "Travaux", MacroCategory = "Obligatoire" },
                new CcCategory { Id = 4, Name = "Loisir", MacroCategory = "Loisir" },
                new CcCategory { Id = 5, Name = "Vacances", MacroCategory = "Loisir" },
                new CcCategory { Id = 6, Name = "Transport", MacroCategory = "Obligatoire" },
                new CcCategory { Id = 7, Name = "Factures", MacroCategory = "Obligatoire" },
                new CcCategory { Id = 8, Name = "Vêtements", MacroCategory = "Obligatoire" },
                new CcCategory { Id = 9, Name = "Cadeaux", MacroCategory = "Loisir" },
                new CcCategory { Id = 10, Name = "Santé", MacroCategory = "Obligatoire" },
                new CcCategory { Id = 11, Name = "Autres", MacroCategory = "Obligatoire" },
                new CcCategory { Id = 12, Name = "Maison/Equip.", MacroCategory = "Obligatoire" },
                new CcCategory { Id = 13, Name = "Neutre", MacroCategory = "Neutre" },
                new CcCategory { Id = 14, Name = "Revenu", MacroCategory = "Revenu" }
            );

            // 4. Seed Data des Règles (Exemple réduit pour la clarté, conserve tes 100+ règles ici)
            modelBuilder.Entity<CcCategoryRule>().HasData(
                new CcCategoryRule { Id = 1, IsUsed = true, Pattern = "VIR GENERATION", Category = "Santé", Comment = "" },
                new CcCategoryRule { Id = 2, IsUsed = true, Pattern = "MAISON LEGUI", Category = "Courses", Comment = "" }
                // ... (Conserve tes autres règles ici)
            );
        }

        private string ToSnakeCase(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return Regex.Replace(text, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }
    }
}