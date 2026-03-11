using BudgetTrackerApi.DTOs;
using BudgetTrackerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTrackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseHealthController : ControllerBase
    {
        private readonly DatabaseHealthService _healthService;

        public DatabaseHealthController(DatabaseHealthService healthService)
        {
            _healthService = healthService;
        }

        [HttpGet]
        public async Task<ActionResult<DatabaseHealthReportDto>> GetHealthReport(CancellationToken cancellationToken)
        {
            var report = await _healthService.GetDatabaseHealthAsync(cancellationToken);
            return Ok(report);
        }
    }
}
