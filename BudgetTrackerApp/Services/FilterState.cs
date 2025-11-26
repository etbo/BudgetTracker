namespace BudgetTrackerApp.Data.Services
{
    public class FiltersState
    {
        public DateTime StartDate { get; private set; } = DateTime.Today.AddMonths(-1);
        public DateTime EndDate { get; private set; } = DateTime.Today;

        public string? Account { get; private set; } = null; // null = tous
        public string? OperationType { get; private set; } = null;

        public event Action? OnChange;

        public void SetStartDate(DateTime value)
        {
            StartDate = value;
            Notify();
        }

        public void SetEndDate(DateTime value)
        {
            EndDate = value;
            Notify();
        }

        public void SetAccount(string? value)
        {
            Account = value;
            Notify();
        }

        public void SetOperationType(string? value)
        {
            OperationType = value;
            Notify();
        }

        private void Notify() => OnChange?.Invoke();
    }
}
