using BudgetTrackerApi.Models.LifeInsurance;
using BudgetTrackerApi.Models.Savings;

namespace BudgetTrackerApi.Models
{
    public enum AccountType
    {
        Checking,      // Compte Courant
        Savings,       // Livret d'épargne
        LifeInsurance  // Assurance Vie
    }

    public class Account
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Owner { get; set; }
        public string? BankName { get; set; } // Utile pour le parsing CC et Livrets
        public bool IsActive { get; set; } = true;
        public int UpdateFrequencyInMonths { get; set; } = 1;
        public AccountType Type { get; set; }

        // Relations (optionnelles selon le type)
        
        // Pour les Assurances Vie
        public virtual ICollection<LifeInsuranceLine> LifeInsuranceLines { get; set; } = new List<LifeInsuranceLine>();
        
        // Pour les Livrets d'épargne
        public virtual ICollection<SavingStatement> SavingStatements { get; set; } = new List<SavingStatement>();
        
        // Pour les Comptes Courants (à venir)
        // public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}