using BlazorApp.Data;

public interface IBanqueCsvParser
{
    List<OperationCC> ParseCsv(Stream csvStream);
}
