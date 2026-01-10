using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BudgetTrackerApi.Models.Savings
{
    public class SavingStatement
    {
        public int Id { get; set; }

        public int SavingAccountId { get; set; }

        public DateTime Date { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        // La note peut être vide, donc on utilise 'string?'
        public string? Note { get; set; }

        // Pour EF Core, on marque la navigation property comme 'null!' 
        // car elle sera remplie par EF lors des jointures (Lazy/Eager loading)
        [JsonIgnore]
        public virtual SavingAccount Account { get; set; } = null!;
    }
}