namespace BlazorApp.CsvMappings
{
    using CsvHelper.Configuration;

    public sealed class TransactionMaybankCsvMap : ClassMap<TransactionMaybankCsv>
    {
        public TransactionMaybankCsvMap()
        {
            Map(m => m.Date).Name("Entry Date");
            Map(m => m.TransactionType).Name("Transaction Type");
            Map(m => m.Description).Name("Transaction Description");
            Map(m => m.Montant).Name("Transaction Amount");
            Map(m => m.StatementBalance).Name("Statement_Balance");
            Map(m => m.Flow).Name("flow");
        }
    }
}
