namespace BudgetTrackerApi.Models.LifeInsurance
{
    public class LifeInsuranceAccount
    {
        public int Id { get; set; }
        public required string Name { get; set; }        // ex: "Linxea Spirit 2"
        public required string Owner { get; set; }       // Ton nouveau champ (ex: "Joint", "Moi")
        public bool IsActive { get; set; } = true;

        // Liste des lignes de ce contrat (Fonds Euro + les 3 SCPI)
        public List<LifeInsuranceLine> Lines { get; set; }
    }

    public class LifeInsuranceLine
    {
        public int Id { get; set; }
        public int LifeInsuranceAccountId { get; set; }

        public LifeInsuranceAccount Account { get; set; }
        
        public required string Label { get; set; }       // ex: "Fonds Euro", "Activimmo", etc.
        public bool IsScpi { get; set; }        // Pour différencier le fonds euro des SCPI
        public List<LifeInsuranceStatement> Statements { get; set; }
    }

    public class LifeInsuranceStatement
    {
        public int Id { get; set; }
        public int LifeInsuranceLineId { get; set; }
        public LifeInsuranceLine Line { get; set; }
        public DateTime Date { get; set; }

        // Pour le fonds Euro, UnitCount sera 1 et UnitValue sera le montant
        // Pour les SCPI, on stocke les deux
        public decimal UnitCount { get; set; }
        public decimal UnitValue { get; set; }

        // Valeur totale calculée (Optionnel en base, peut être calculé au runtime)
        public decimal TotalValue => UnitCount * UnitValue;
    }
}