using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTrackerApi.Models.LifeInsurance
{
    public class LifeInsuranceLine
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; } = null!;

        public string Label { get; set; } = string.Empty;
        public bool IsScpi { get; set; }
        public virtual ICollection<LifeInsuranceStatement> Statements { get; set; } = new List<LifeInsuranceStatement>();
    }

    public class LifeInsuranceStatement
    {
        public int Id { get; set; }
        public int LifeInsuranceLineId { get; set; }
        public virtual LifeInsuranceLine Line { get; set; } = default!;
        public DateTime Date { get; set; }

        // Pour le fonds Euro, UnitCount sera 1 et UnitValue sera le montant
        // Pour les SCPI, on stocke les deux
        public decimal UnitCount { get; set; }
        public decimal UnitValue { get; set; }

        // Valeur totale calculée (Optionnel en base, peut être calculé au runtime)
        public decimal TotalValue => UnitCount * UnitValue;
    }
}