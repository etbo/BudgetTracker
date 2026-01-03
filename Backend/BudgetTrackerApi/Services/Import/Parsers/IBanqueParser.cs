using BudgetTrackerApp.Data;
using BudgetTrackerApp.Models;

public interface IBanqueParser
{
    string BankName { get; }
    List<CcOperation> Parse(ParserInputContext ctx);
}
