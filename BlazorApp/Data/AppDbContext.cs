using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Data
{
      public class AppDbContext : DbContext
      {
            public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options)
            {
            }

            public DbSet<OperationCC> Operations => Set<OperationCC>();

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

                        entity.Property(e => e.Type)
                        .HasColumnName("Type");

                        entity.Property(e => e.Commentaire)
                        .HasColumnName("Commentaire");

                        entity.Property(e => e.Banque)
                        .HasColumnName("Banque");

                        entity.Property(e => e.DateImport)
                        .HasColumnName("DateImport");
                        
                        entity.Property(e => e.Hash)
                        .HasColumnName("Hash");
                  });
            }
      }

      public class OperationCC
      {
            public int Id { get; set; }
            public string Date { get; set; } = string.Empty;
            public string? Description { get; set; }
            public double Montant { get; set; }
            public string? Type { get; set; }
            public string? Commentaire { get; set; }
            public string? Banque { get; set; }
            public string? DateImport { get; set; }
            public string? Hash { get; set; }
      }
}
