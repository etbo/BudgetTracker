using BudgetTrackerApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetTrackerApi.Data.Configurations
{
    public class CcCategoryConfiguration : IEntityTypeConfiguration<CcCategory>
    {
        public void Configure(EntityTypeBuilder<CcCategory> builder)
        {
            builder.HasData(
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
        }
    }
}
