using System;
using BudgetTrackerApp.Models;

namespace BudgetTrackerApp.Data.Helpers
{
    public static class OperationExtensions
    {
        public static string GenerateBaseHash(this CcOperation op)
        {
            string raw = $"{op.Date}|{op.Description}|{op.Montant}|{op.Categorie}|{op.Banque}";

            if (string.IsNullOrWhiteSpace(raw))
                throw new InvalidOperationException("Impossible de générer un hash à partir de valeurs null ou vides.");

            return OperationHashHelper.ComputeHash(raw);
        }
    }
}

