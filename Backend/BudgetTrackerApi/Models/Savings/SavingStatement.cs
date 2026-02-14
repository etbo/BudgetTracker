using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BudgetTrackerApi.Models.Savings
{
    public class SavingStatement
    {
        public int Id { get; set; }

        public int AccountId { get; set; }
        [ForeignKey("AccountId")]
        [JsonIgnore]
        public virtual Account? Account { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        // La note peut être vide, donc on utilise 'string?'
        public string? Note { get; set; }

    }
}