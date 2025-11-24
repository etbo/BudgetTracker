using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace BudgetTrackerApp.Data.Helpers
{
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
