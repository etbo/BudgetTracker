using System;
using System.Collections.Generic;

namespace BudgetTrackerApi.DTOs
{
    public class DatabaseHealthReportDto
    {
        public List<AccountMissingMonthsDto> MissingMonths { get; set; } = new List<AccountMissingMonthsDto>();
        public List<UnknownCategoryDto> UnknownCategories { get; set; } = new List<UnknownCategoryDto>();
    }

    public class AccountMissingMonthsDto
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public List<string> MissingMonths { get; set; } = new List<string>(); // Format "YYYY-MM"
    }

    public class UnknownCategoryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public int OperationCount { get; set; }
    }
}
