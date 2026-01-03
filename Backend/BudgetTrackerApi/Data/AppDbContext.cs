using BudgetTrackerApp.Models;
using BudgetTrackerApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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

        // --- Tables ---
        public DbSet<OperationCC> OperationsCC => Set<OperationCC>();
        public DbSet<CategoryRule> CategoryRules { get; set; }
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<OperationPea> OperationsPea { get; set; } = null!;
        public DbSet<CachedStockPrice> CachedStockPrices { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Appliquer le snake_case AUTOMATIQUEMENT à toutes les tables et colonnes
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Nom de la table : OperationPea -> operation_pea
                var tableName = entity.GetTableName();
                if (tableName != null)
                {
                    entity.SetTableName(ToSnakeCase(tableName));
                }

                // Nom des colonnes : MontantNet -> montant_net
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToSnakeCase(property.Name));
                }
            }

            // 2. Ajustements manuels spécifiques (Exceptions aux règles automatiques)
            
            modelBuilder.Entity<OperationCC>(entity =>
            {
                // On s'assure que le nom de table reste cohérent avec ton choix
                entity.ToTable("operations_cc");
                entity.Ignore(e => e.IsModified);
            });

            modelBuilder.Entity<OperationPea>(entity =>
            {
                entity.ToTable("operations_pea");
                // Exception manuelle pour l'accent : sinon le snake_case ferait "quantit_é" ou "quantit"
                entity.Property(e => e.Quantité).HasColumnName("quantite");
            });

            modelBuilder.Entity<CategoryRule>(entity =>
            {
                entity.ToTable("category_rules");
            });
        }

        /// <summary>
        /// Transforme le PascalCase en snake_case (ex: MontantNet -> montant_net)
        /// </summary>
        private string ToSnakeCase(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            // Ajoute un underscore devant chaque majuscule et passe en minuscule
            return Regex.Replace(text, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }
    }
}