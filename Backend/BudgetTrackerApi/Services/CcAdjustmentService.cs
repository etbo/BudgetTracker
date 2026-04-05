using System.Text.RegularExpressions;
using BudgetTrackerApi.Data;
using BudgetTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTrackerApi.Services
{
    public class CcAdjustmentService
    {
        private readonly AppDbContext _db;
        private readonly Regex _targetRegex = new Regex(@"Ajustement Solde \(([\d,.\s+-]+)€\)", RegexOptions.Compiled);

        public CcAdjustmentService(AppDbContext db)
        {
            _db = db;
        }

        public async Task RecalculateAsync(string bank, DateTime fromDate)
        {
            var start = fromDate.Date;

            // 1. Récupérer toutes les opérations à partir de la date (pour cette banque)
            // On inclut les opérations AVANT fromDate pour avoir le solde correct au point de départ ?
            // Non, on calcule le solde théorique initial AVANT start.
            
            var initialBalance = await _db.CcOperations
                .Where(o => o.Bank == bank && o.Date.Date < start)
                .SumAsync(o => o.Amount);

            // 2. Récupérer les opérations à partir de la date de départ, triées par :
            //    - Date (chronologique)
            //    - Type (opérations normales d'abord, ajustements à la fin de la journée)
            //    - Id (pour l'ordre d'insertion)
            var operations = await _db.CcOperations
                .Where(o => o.Bank == bank && o.Date.Date >= start)
                .OrderBy(o => o.Date)
                .ThenBy(o => (o.Description != null && o.Description.StartsWith("Ajustement Solde")) ? 1 : 0)
                .ThenBy(o => o.Id) 
                .ToListAsync();

            double currentTheoreticalBalance = initialBalance;

            bool hasChanges = false;

            foreach (var op in operations)
            {
                if (op.Description != null && op.Description.StartsWith("Ajustement Solde"))
                {
                    var match = _targetRegex.Match(op.Description);
                    if (match.Success)
                    {
                        var targetStr = match.Groups[1].Value.Replace(" ", "").Replace(",", ".");
                        if (double.TryParse(targetStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double targetBalance))
                        {
                            // Calcul de l'ajustement nécessaire pour arriver à targetBalance
                            double newAmount = targetBalance - currentTheoreticalBalance;
                            
                            if (Math.Abs(op.Amount - newAmount) > 0.001)
                            {
                                op.Amount = newAmount;
                                op.Comment = $"Saisi: {targetBalance:F2} | Recalculé le {DateTime.Now:dd/MM/yyyy HH:mm}";
                                hasChanges = true;
                            }
                            
                            // On met à jour le solde courant par la cible qu'on vient d'atteindre
                            currentTheoreticalBalance = targetBalance;
                            continue;
                        }
                    }
                }

                // Pour une opération normale, on ajoute simplement son montant au solde théorique
                currentTheoreticalBalance += op.Amount;
            }

            if (hasChanges)
            {
                await _db.SaveChangesAsync();
            }
        }
    }
}
