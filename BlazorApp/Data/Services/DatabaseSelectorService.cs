// Services/DatabaseSelectorService.cs

namespace BlazorApp.Services
{
    public class DatabaseSelectorService
    {
        public string CurrentDatabase { get; private set; } = "Production";

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