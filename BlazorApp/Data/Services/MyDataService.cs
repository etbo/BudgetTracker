namespace BlazorApp.Services
{
    public class MyDataService
    {
        private readonly DatabaseSelectorService _dbSelector;

        public MyDataService(DatabaseSelectorService dbSelector)
        {
            _dbSelector = dbSelector;
        }

        public string GetConnectionString()
        {
            return _dbSelector.CurrentDatabase == "Test"
                ? "Data Source=TestDB..."
                : "Data Source=ProdDB...";
        }
    }
}
