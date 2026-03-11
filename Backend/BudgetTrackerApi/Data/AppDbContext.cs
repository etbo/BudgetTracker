using BudgetTrackerApi.Models;
using BudgetTrackerApi.Models.LifeInsurance;
using BudgetTrackerApi.Models.Savings;
using BudgetTrackerApi.Services;
using BudgetTrackerApi.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BudgetTrackerApi.Data
{
    public class AppDbContext : DbContext
    {
        private readonly DatabaseSelectorService _dbSelector;

        public AppDbContext(DbContextOptions<AppDbContext> options, DatabaseSelectorService dbSelector)
            : base(options)
        {
            _dbSelector = dbSelector;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // On cherche le dossier Database à partir de la racine de la solution
                var executionDir = AppDomain.CurrentDomain.BaseDirectory;
                // On remonte jusqu'à trouver le dossier "Database"
                var dataDir = Path.Combine(executionDir, "..", "..", "..", "Database");

                // Si ça ne marche pas en debug, on utilise le chemin relatif direct
                if (!Directory.Exists(dataDir))
                {
                    dataDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Database"));
                }

                string dbFileName = _dbSelector.CurrentDatabase == "Test"
                    ? "BudgetTrackerTest.db"
                    : "BudgetTracker.db";

                string fullPath = Path.Combine(dataDir, dbFileName);

                // LOG DE DEBUG : Très important pour voir où l'API cherche vraiment
                Console.WriteLine($"---> Connexion à la base : {fullPath}");

                optionsBuilder.UseSqlite($"Data Source={fullPath};Pooling=False");
            }
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
            modelBuilder.Entity<SavingStatement>(entity =>
            {
                entity.HasOne(s => s.Account)
                      .WithMany(a => a.SavingStatements)
                      .HasForeignKey(s => s.AccountId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

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

            // 3. Configurations séparées pour le Seed Data
            modelBuilder.ApplyConfiguration(new CcCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new CcCategoryRuleConfiguration());
        }

        private string ToSnakeCase(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return Regex.Replace(text, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }
    }
}