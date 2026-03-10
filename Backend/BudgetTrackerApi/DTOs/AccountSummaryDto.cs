using BudgetTrackerApi.Models;

namespace BudgetTrackerApi.DTOs
{
    public record AccountSummaryDto(
        int Id,
        string Name,
        string? Owner,
        string? BankName,
        AccountType Type,
        bool IsActive,
        int UpdateFrequencyInMonths,
        DateTime? LastEntryDate,
        bool IsLate,
        string StatusMessage
    );
}
