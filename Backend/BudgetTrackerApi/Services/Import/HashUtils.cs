// Fichier : Data/Services/Import/HashUtils.cs
using System.Security.Cryptography;
using System.Text;

namespace BudgetTrackerApp.Services.Import
{
    public static class HashUtils
    {
        public static string ComputeHash(string raw)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(raw);
            var hashBytes = sha.ComputeHash(bytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}
