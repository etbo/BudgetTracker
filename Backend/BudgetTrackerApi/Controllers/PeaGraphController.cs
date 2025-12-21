using Microsoft.AspNetCore.Mvc;
using BudgetTrackerApp.Services;
using BudgetTrackerApp.Models;

namespace BudgetTrackerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Ceci devient /api/finance
    public class PeaGraphController : ControllerBase
    {
        private readonly IPeaService _peaService;
        private readonly FinanceService _financeService;

        // Tu injectes les DEUX ici
        public PeaGraphController(IPeaService peaService, FinanceService financeService)
        {
            _peaService = peaService;
            _financeService = financeService;
        }

        [HttpGet("cumul")]
        public async Task<IActionResult> GetCumul()
        {
            try
            {
                if (_peaService == null)
                    return BadRequest("Le service _peaService est NULL dans le controller.");

                var data = await _peaService.CalculerCumul();
                return Ok(data);
            }
            catch (Exception ex)
            {
                // On renvoie TOUT : le message, la source et la pile d'exécution
                var fullError = $"Message: {ex.Message} | Source: {ex.Source} | Stack: {ex.StackTrace}";
                Console.WriteLine(fullError); // Force l'écriture en console
                return StatusCode(500, fullError);
            }
        }

        [HttpGet("update-prices")] // Ceci complète l'URL : /api/finance/update-prices
        public async Task<ActionResult<List<UpdateResult>>> UpdatePrices()
        {
            var tickers = await _financeService.GetTickerList();
            var results = new List<UpdateResult>();

            if (tickers == null) return Ok(results);

            foreach (var ticker in tickers)
            {
                if (string.IsNullOrEmpty(ticker.Ticker)) continue;

                // On appelle ta logique existante du FinanceService
                var res = await _financeService.UpdateCachedStockPrice(ticker.Ticker, ticker.OldestDate, false);

                results.Add(new UpdateResult
                {
                    Ticker = ticker.Ticker,
                    Status = res.Status.ToString(),
                    Message = res.Message
                });
            }

            return Ok(results);
        }
    }

    // Petite classe pour structurer la réponse JSON
    public class UpdateResult
    {
        public string Ticker { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}