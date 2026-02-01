using System;
using BudgetTrackerApi.Models;

namespace BudgetTrackerApi.Data.Helpers
{
    public static class OperationExtensions
    {
        public static string GenerateBaseHash(this CcOperation op)
        {
            string raw = $"{op.Date}|{op.Description}|{op.Amount}|{op.Bank}";

            if (string.IsNullOrWhiteSpace(raw))
                throw new InvalidOperationException("Impossible de générer un hash à partir de valeurs null ou vides.");

            return OperationHashHelper.ComputeHash(raw);
        }
    }
}

