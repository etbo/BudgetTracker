using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace BudgetTrackerApp.Data.Helpers
{
    public static class OperationHashHelper
    {
        public static string ComputeHash(string texte)
        {
            using var sha = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(texte);
            byte[] hashBytes = sha.ComputeHash(bytes);
            return Convert.ToHexString(hashBytes);
        }

    }

}
