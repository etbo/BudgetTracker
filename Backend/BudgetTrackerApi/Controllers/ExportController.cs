using BudgetTrackerApi.Services.Export;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly DatabaseExportService _exportService;

    public ExportController(DatabaseExportService exportService)
    {
        _exportService = exportService;
    }

    [HttpGet("database")]
    public IActionResult DownloadDatabase()
    {
        try
        {
            byte[] fileBytes = _exportService.ExportDatabaseAsZip();
            string fileName = $"BudgetTrackerBackup_{DateTime.Now:yyyyMMdd_HHmm}.zip";
            
            return File(fileBytes, "application/zip", fileName);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound($"Erreur : {ex.Message}");
        }
    }
}