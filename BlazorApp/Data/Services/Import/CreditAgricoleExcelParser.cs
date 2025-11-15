using System.Text;
using BlazorApp.Data;

public class CreditAgricoleExcelParser : IBanqueParser
{
    public string BankName => "Crédit agricole";

    public List<OperationCC> Parse(ParserInputContext ctx)
    {
        var ListOperations = new List<OperationCC>();

        Console.WriteLine("Parser CA");


        return ListOperations;
    }
}
