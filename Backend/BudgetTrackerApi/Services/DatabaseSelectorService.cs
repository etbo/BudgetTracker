// Services/DatabaseSelectorService.cs

namespace BudgetTrackerApi.Services
{
    public class DatabaseSelectorService
    {
        public string CurrentDatabase { get; private set; } = "Test";

        public event Func<Task>? OnChange;

        public async Task SetDatabase(string db)
        {
            CurrentDatabase = db;
            if (OnChange != null)
            {
                await OnChange.Invoke();
            }
        }
    }

}