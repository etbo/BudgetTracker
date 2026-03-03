// Services/DatabaseSelectorService.cs

namespace BudgetTrackerApi.Services
{
    public class DatabaseSelectorService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DatabaseSelectorService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string CurrentDatabase =>
            _httpContextAccessor.HttpContext?.Request.Headers["X-Database-Selection"].ToString() ?? "Prod";
    }

}