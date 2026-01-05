using BudgetTrackerApi.Data;
using BudgetTrackerApi.Models;

public interface IBanqueParser
{
    string BankName { get; }
    List<CcOperation> Parse(ParserInputContext ctx);
}
