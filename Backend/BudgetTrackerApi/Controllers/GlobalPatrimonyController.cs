using BudgetTrackerApi.Data;
using BudgetTrackerApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class GlobalPatrimonyController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly BudgetTrackerApi.Services.PatrimonyService _patrimonyService;

    public GlobalPatrimonyController(AppDbContext context, BudgetTrackerApi.Services.PatrimonyService patrimonyService)
    {
        _db = context;
        _patrimonyService = patrimonyService;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<PatrimonySummaryDto>> GetCurrentSummary()
    {
        var summary = await _patrimonyService.GetCurrentSummaryAsync();
        return Ok(summary);
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<GlobalHistoryDto>>> GetGlobalHistory()
    {
        var history = await _patrimonyService.GetGlobalHistoryAsync();
        return Ok(history);
    }
}