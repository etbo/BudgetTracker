// Fichier : MonProjetBlazor/Models/PeaCachedStockPrice.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTrackerApi.Models
{
    // Cette table stockera un prix unitaire pour une date donnée
    public class PeaCachedStockPrice
    {
        public int Id { get; set; }

        public string Ticker { get; set; } = "";

        // Utiliser la date du prix de clôture
        public DateTime Date { get; set; }

        public decimal Price { get; set; }

        // Horodatage pour vérifier l'expiration du cache
        public DateTime CacheTimestamp { get; set; }
    }
}