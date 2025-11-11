using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace BlazorApp.Data.Services.Import
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

    public static class DateTimeHelper
    {
        public static string? ToIsoString(string rawDateTime)
        {
            if (string.IsNullOrWhiteSpace(rawDateTime))
                return null;
            
            // Essaie de parser la date
            if (DateTime.TryParse(rawDateTime, out DateTime dt))
            {
                return dt.ToString("yyyy-MM-dd");
            }

            // Retourne null si le parsing échoue
            return null;
        }

        public static string ToCustomFormat(this DateTime dt, string format)
        {
            return dt.ToString(format);
        }
    }
}
