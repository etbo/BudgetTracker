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

            // --- Tables ---
            public DbSet<CcOperation> CcOperations => Set<CcOperation>();
            public DbSet<CcCategoryRule> CcCategoryRules { get; set; }
            public DbSet<CcCategory> CcCategories { get; set; } = null!;
            public DbSet<PeaOperation> PeaOperations { get; set; } = null!;
            public DbSet<PeaCachedStockPrice> PeaCachedStockPrices { get; set; } = null!;
            public DbSet<SavingAccount> SavingAccounts { get; set; } = null!;
            public DbSet<SavingStatement> SavingStatements { get; set; } = null!;

            public DbSet<LifeInsuranceAccount> LifeInsuranceAccounts { get; set; }
            public DbSet<LifeInsuranceLine> LifeInsuranceLines { get; set; }
            public DbSet<LifeInsuranceStatement> LifeInsuranceStatements { get; set; }
            public DbSet<CcImportLog> CcImportLogs { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                  base.OnModelCreating(modelBuilder);

                  // 1. Appliquer le snake_case AUTOMATIQUEMENT à toutes les tables et colonnes
                  foreach (var entity in modelBuilder.Model.GetEntityTypes())
                  {
                        // Nom de la table : PeaOperation -> operation_pea
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

                  modelBuilder.Entity<CcOperation>(entity =>
                  {
                        // On s'assure que le nom de table reste cohérent avec ton choix
                        entity.ToTable("cc_operations");
                  });

                  modelBuilder.Entity<PeaOperation>(entity =>
                  {
                        entity.ToTable("pea_operations");
                        // Exception manuelle pour l'accent : sinon le snake_case ferait "quantit_é" ou "quantit"
                        entity.Property(e => e.Quantité).HasColumnName("quantite");
                  });

                  modelBuilder.Entity<CcCategoryRule>(entity =>
                  {
                        entity.ToTable("cc_category_rules");
                  });

                  modelBuilder.Entity<CcCategory>().HasData(
                        new CcCategory { Id = 1, Name = "Prêt", Type = "Obligatoire" },
                        new CcCategory { Id = 2, Name = "Courses", Type = "Obligatoire" },
                        new CcCategory { Id = 3, Name = "Travaux", Type = "Obligatoire" },
                        new CcCategory { Id = 4, Name = "Loisir", Type = "Loisir" },
                        new CcCategory { Id = 5, Name = "Vacances", Type = "Loisir" },
                        new CcCategory { Id = 6, Name = "Transport", Type = "Obligatoire" },
                        new CcCategory { Id = 7, Name = "Factures", Type = "Obligatoire" },
                        new CcCategory { Id = 8, Name = "Vêtements", Type = "Obligatoire" },
                        new CcCategory { Id = 9, Name = "Cadeaux", Type = "Loisir" },
                        new CcCategory { Id = 10, Name = "Santé", Type = "Obligatoire" },
                        new CcCategory { Id = 11, Name = "Autres", Type = "Obligatoire" },
                        new CcCategory { Id = 12, Name = "Maison/Equip.", Type = "Obligatoire" },
                        new CcCategory { Id = 13, Name = "Neutre", Type = "Neutre" },
                        new CcCategory { Id = 14, Name = "Revenu", Type = "Revenu" }
                  );

                  modelBuilder.Entity<CcCategoryRule>().HasData(
                        new CcCategoryRule { Id = 1, IsUsed = true, Pattern = "VIR GENERATION", Category = "Santé", Comment = "" },
                        new CcCategoryRule { Id = 2, IsUsed = true, Pattern = "MAISON LEGUI", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 3, IsUsed = true, Pattern = "Castorama", Category = "Travaux", Comment = "" },
                        new CcCategoryRule { Id = 4, IsUsed = true, Pattern = "Sfr", Category = "Factures", Comment = "" },
                        new CcCategoryRule { Id = 5, IsUsed = true, Pattern = "edm", Category = "Ammeublement", Comment = "" },
                        new CcCategoryRule { Id = 6, IsUsed = true, Pattern = "Assu. Cnp Pret Habitat", Category = "Prêt", Comment = "" },
                        new CcCategoryRule { Id = 7, IsUsed = true, Pattern = "Bio's Hair", Category = "Santé", Comment = "" },
                        new CcCategoryRule { Id = 8, IsUsed = true, Pattern = "Auchan Le Mans", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 9, IsUsed = true, Pattern = "Boulangerie", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 10, IsUsed = true, Pattern = "Carrefour City Le Mans", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 11, IsUsed = true, Pattern = "PRLV DIRECTION GENERALE DES FINA", Category = "Factures", Comment = "" },
                        new CcCategoryRule { Id = 12, IsUsed = true, Pattern = "Carrefour Lemans", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 13, IsUsed = true, Pattern = "Casino Shop", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 14, IsUsed = true, Pattern = "Eleveurs Regionaux", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 15, IsUsed = true, Pattern = "Fournil D'edison", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 16, IsUsed = true, Pattern = "Grand*Frais", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 17, IsUsed = true, Pattern = "Leclerc*Web", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 18, IsUsed = true, Pattern = "Lidl", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 19, IsUsed = true, Pattern = "Mgp*la Ruche", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 20, IsUsed = true, Pattern = "Mie*Caline", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 21, IsUsed = true, Pattern = "U Express", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 22, IsUsed = true, Pattern = "Direct*Energie", Category = "Factures", Comment = "" },
                        new CcCategoryRule { Id = 23, IsUsed = true, Pattern = "ENI Gas", Category = "Factures", Comment = "" },
                        new CcCategoryRule { Id = 24, IsUsed = true, Pattern = "Tres.  Le Mans Ville", Category = "Factures", Comment = "" },
                        new CcCategoryRule { Id = 25, IsUsed = true, Pattern = "Auchan Carburant", Category = "Transport", Comment = "" },
                        new CcCategoryRule { Id = 26, IsUsed = true, Pattern = "Dac Intermarche", Category = "Transport", Comment = "" },
                        new CcCategoryRule { Id = 27, IsUsed = true, Pattern = "E.Leclerc Laval Cedex", Category = "Transport", Comment = "" },
                        new CcCategoryRule { Id = 28, IsUsed = true, Pattern = "Relais*Le*Mans*R", Category = "Transport", Comment = "" },
                        new CcCategoryRule { Id = 29, IsUsed = true, Pattern = "Stat Leclerc 24/24", Category = "Transport", Comment = "" },
                        new CcCategoryRule { Id = 30, IsUsed = true, Pattern = "Station U 72", Category = "Transport", Comment = "" },
                        new CcCategoryRule { Id = 31, IsUsed = true, Pattern = "Cabriole", Category = "Factures", Comment = "" },
                        new CcCategoryRule { Id = 32, IsUsed = true, Pattern = "Esprit Ruaudin", Category = "Vêtements", Comment = "" },
                        new CcCategoryRule { Id = 33, IsUsed = true, Pattern = "Impot", Category = "Factures", Comment = "" },
                        new CcCategoryRule { Id = 34, IsUsed = true, Pattern = "Cultura", Category = "Loisir", Comment = "" },
                        new CcCategoryRule { Id = 35, IsUsed = true, Pattern = "Harmonie", Category = "Santé", Comment = "" },
                        new CcCategoryRule { Id = 36, IsUsed = true, Pattern = "Siaci Saint Honore", Category = "Santé", Comment = "" },
                        new CcCategoryRule { Id = 37, IsUsed = true, Pattern = "Bsa Finances*Virement*Faveur*Malinowski", Category = "Neutre", Comment = "" },
                        new CcCategoryRule { Id = 38, IsUsed = true, Pattern = "Cofir*Rueil", Category = "Transport", Comment = "" },
                        new CcCategoryRule { Id = 39, IsUsed = true, Pattern = "Realisation De Pret", Category = "Prêt", Comment = "" },
                        new CcCategoryRule { Id = 40, IsUsed = true, Pattern = "Remboursement De Pr*t", Category = "Prêt", Comment = "" },
                        new CcCategoryRule { Id = 41, IsUsed = true, Pattern = "Bk Rest", Category = "Loisir", Comment = "" },
                        new CcCategoryRule { Id = 42, IsUsed = true, Pattern = "Burger King", Category = "Loisir", Comment = "" },
                        new CcCategoryRule { Id = 43, IsUsed = true, Pattern = "Crousty", Category = "Loisir", Comment = "" },
                        new CcCategoryRule { Id = 44, IsUsed = true, Pattern = "Haochi", Category = "Loisir", Comment = "" },
                        new CcCategoryRule { Id = 45, IsUsed = true, Pattern = "Kebab", Category = "Loisir", Comment = "" },
                        new CcCategoryRule { Id = 46, IsUsed = true, Pattern = "Lgpo", Category = "Loisir", Comment = "" },
                        new CcCategoryRule { Id = 47, IsUsed = true, Pattern = "Mac Donald", Category = "Loisir", Comment = "" },
                        new CcCategoryRule { Id = 48, IsUsed = true, Pattern = "Mc Donald", Category = "Loisir", Comment = "" },
                        new CcCategoryRule { Id = 49, IsUsed = true, Pattern = "P'tits Ponts", Category = "Loisir", Comment = "" },
                        new CcCategoryRule { Id = 50, IsUsed = true, Pattern = "Retrait Au Distributeur", Category = "Autre", Comment = "" },
                        new CcCategoryRule { Id = 51, IsUsed = true, Pattern = "Serac", Category = "Revenu", Comment = "" },
                        new CcCategoryRule { Id = 52, IsUsed = true, Pattern = "Virement Salaire", Category = "Revenu", Comment = "" },
                        new CcCategoryRule { Id = 53, IsUsed = true, Pattern = "C.P.A.M.", Category = "Santé", Comment = "" },
                        new CcCategoryRule { Id = 54, IsUsed = true, Pattern = "Phie", Category = "Santé", Comment = "" },
                        new CcCategoryRule { Id = 55, IsUsed = true, Pattern = "Setram", Category = "Transport", Comment = "" },
                        new CcCategoryRule { Id = 56, IsUsed = true, Pattern = "Sncf", Category = "Transport", Comment = "" },
                        new CcCategoryRule { Id = 57, IsUsed = true, Pattern = "Brico Depot", Category = "Travaux", Comment = "" },
                        new CcCategoryRule { Id = 58, IsUsed = true, Pattern = "Lapeyre", Category = "Travaux", Comment = "" },
                        new CcCategoryRule { Id = 59, IsUsed = true, Pattern = "Leroy", Category = "Travaux", Comment = "" },
                        new CcCategoryRule { Id = 60, IsUsed = true, Pattern = "Ppg 72", Category = "Travaux", Comment = "" },
                        new CcCategoryRule { Id = 61, IsUsed = true, Pattern = "point p", Category = "Travaux", Comment = "" },
                        new CcCategoryRule { Id = 62, IsUsed = true, Pattern = "LES-BOUCHERS-REGIONAU", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 63, IsUsed = true, Pattern = "LES JARDINS VOYAGEURS SAIN", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 64, IsUsed = true, Pattern = "DR VIANNAY", Category = "Santé", Comment = "" },
                        new CcCategoryRule { Id = 65, IsUsed = true, Pattern = "Biocoop", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 66, IsUsed = true, Pattern = "Latrouite", Category = "Santé", Comment = "" },
                        new CcCategoryRule { Id = 67, IsUsed = true, Pattern = "LA CUISINE DU SO LA FERTE", Category = "Loisir", Comment = "" },
                        new CcCategoryRule { Id = 68, IsUsed = true, Pattern = "LE FENOUIL LE MANS", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 69, IsUsed = true, Pattern = "PHARMACIE", Category = "Santé", Comment = "" },
                        new CcCategoryRule { Id = 70, IsUsed = true, Pattern = "CLAP BOULANGERI", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 71, IsUsed = true, Pattern = "HENRIOT THOMAS", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 72, IsUsed = true, Pattern = "MAISON LEGUI LE MANS", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 73, IsUsed = true, Pattern = "E.LECLERC LE MANS CEDEX", Category = "Transport", Comment = "Essence" },
                        new CcCategoryRule { Id = 74, IsUsed = true, Pattern = "GAN ASSURANCES PLV CLT", Category = "Assurance", Comment = "" },
                        new CcCategoryRule { Id = 75, IsUsed = true, Pattern = "Deezer", Category = "Loisir", Comment = "" },
                        new CcCategoryRule { Id = 76, IsUsed = true, Pattern = "INGUERE ALICE LE MANS", Category = "Santé", Comment = "" },
                        new CcCategoryRule { Id = 77, IsUsed = true, Pattern = "FREE MOBILE", Category = "Factures", Comment = "" },
                        new CcCategoryRule { Id = 78, IsUsed = true, Pattern = "UDEMY", Category = "Loisir", Comment = "" },
                        new CcCategoryRule { Id = 79, IsUsed = true, Pattern = "PRLV CEDETEL", Category = "Factures", Comment = "" },
                        new CcCategoryRule { Id = 80, IsUsed = true, Pattern = "PALAIS DES THES", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 81, IsUsed = true, Pattern = "TotalEnergies", Category = "Factures", Comment = "" },
                        new CcCategoryRule { Id = 82, IsUsed = true, Pattern = "Psychomot", Category = "Santé", Comment = "" },
                        new CcCategoryRule { Id = 83, IsUsed = true, Pattern = "BELLE GARDE SAVIGNE", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 84, IsUsed = true, Pattern = "kdopapymamie", Category = "Revenu", Comment = "" },
                        new CcCategoryRule { Id = 85, IsUsed = true, Pattern = "PN LOISIRS PARIS", Category = "Loisir", Comment = "" },
                        new CcCategoryRule { Id = 86, IsUsed = true, Pattern = "CREATTITUDE", Category = "Loisir", Comment = "Manège" },
                        new CcCategoryRule { Id = 87, IsUsed = true, Pattern = "RELAIS MANS RHIN LE MANS", Category = "Transport", Comment = "Essence" },
                        new CcCategoryRule { Id = 88, IsUsed = true, Pattern = "GMT M374", Category = "Loisir", Comment = "Mondial Tissus" },
                        new CcCategoryRule { Id = 89, IsUsed = true, Pattern = "MAMIE MESURE LE MANS", Category = "Santé", Comment = "" },
                        new CcCategoryRule { Id = 90, IsUsed = true, Pattern = "Google Payment I Dublin", Category = "Loisir", Comment = "" },
                        new CcCategoryRule { Id = 91, IsUsed = true, Pattern = "VIR CAF DE LA SARTHE", Category = "Revenu", Comment = "" },
                        new CcCategoryRule { Id = 92, IsUsed = true, Pattern = "Appro remboursement Pret", Category = "Prêt", Comment = "" },
                        new CcCategoryRule { Id = 93, IsUsed = true, Pattern = "PRLV DIRECTION GENERALE DES FINA 2", Category = "Factures", Comment = "Impôts" },
                        new CcCategoryRule { Id = 94, IsUsed = true, Pattern = "DECA DEVELOPPEME ST MAIXENT", Category = "Courses", Comment = "Café BOC" },
                        new CcCategoryRule { Id = 95, IsUsed = true, Pattern = "PHARMA UNIVERSIT LE MANS", Category = "Santé", Comment = "" },
                        new CcCategoryRule { Id = 96, IsUsed = true, Pattern = "JARDINS DE BELLE 72460", Category = "Courses", Comment = "" },
                        new CcCategoryRule { Id = 97, IsUsed = true, Pattern = "PHARMA BEAUREGAR", Category = "Santé", Comment = "" },
                        new CcCategoryRule { Id = 98, IsUsed = true, Pattern = "ST0072 LE MANS", Category = "Transport", Comment = "" },
                        new CcCategoryRule { Id = 99, IsUsed = true, Pattern = "S EVEILLER SIMPL 69290 CRAPONNE", Category = "Loisir", Comment = "" },
                        new CcCategoryRule { Id = 100, IsUsed = true, Pattern = "PRLV ECOLE SAINT PAVIN", Category = "Factures", Comment = "" },
                        new CcCategoryRule { Id = 101, IsUsed = true, Pattern = "L'ARTISANE LE MANS", Category = "Santé", Comment = "" },
                        new CcCategoryRule { Id = 102, IsUsed = true, Pattern = "TIDAL Malmo", Category = "Loisir", Comment = "" }
                        );

                  modelBuilder.Entity<SavingStatement>()
                        .HasOne(s => s.Account)
                        .WithMany(a => a.Statements)
                        .HasForeignKey(s => s.SavingAccountId)
                        .OnDelete(DeleteBehavior.Cascade);
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