using System.ComponentModel.DataAnnotations;
using BudgetTrackerApi.Models;

namespace BudgetTrackerApi.Models.Savings
{
    public class SavingAccount
    {
        public int Id { get; set; }

        // Le mot-clé 'required' force la présence de la valeur et rassure le compilateur
        public required string Name { get; set; }

        public required string Owner { get; set; }

        // Si la banque n'est pas obligatoire, utilise 'string?'
        public string? BankName { get; set; }

        public bool IsActive { get; set; } = true;

        // Pour une collection, on l'initialise toujours pour éviter les NullReferenceException
        public virtual ICollection<SavingStatement> Statements { get; set; } = new List<SavingStatement>();
    }
}