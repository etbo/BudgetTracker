using BudgetTrackerApi.Data;
using BudgetTrackerApi.Models;

public interface IBankParser
{
    string BankName { get; }
    List<CcOperation> Parse(ParserInputContext ctx);
}
