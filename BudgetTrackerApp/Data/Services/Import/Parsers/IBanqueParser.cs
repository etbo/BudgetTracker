using BudgetTrackerApp.Data;

public interface IBanqueParser
{
    string BankName { get; }
    List<OperationCC> Parse(ParserInputContext ctx);
}
