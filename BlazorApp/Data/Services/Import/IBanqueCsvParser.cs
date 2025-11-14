using BlazorApp.Data;

public interface IBanqueCsvParser
{
    string BankName { get; }
    List<OperationCC> ParseCsv(TextReader reader);
}
