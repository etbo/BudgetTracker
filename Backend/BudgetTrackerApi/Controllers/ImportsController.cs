using Microsoft.AspNetCore.Mvc;
using BudgetTrackerApi.Services;
using BudgetTrackerApi.DTOs; // Adapte selon tes namespaces

[ApiController]
[Route("api/[controller]")]
public class ImportsController : ControllerBase
{
    private readonly ImportService _importService;

    public ImportsController(ImportService importService)
    {
        _importService = importService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        Console.WriteLine($"Fichier reçu :: {file.FileName}, Taille : {file.Length} octets");

        if (file == null || file.Length == 0)
            return BadRequest("Aucun fichier reçu.");

        // On appelle la logique que tu avais dans ton @code Blazor
        // mais désormais encapsulée dans le service
        var result = await _importService.ProcessImportAsync(file);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<ImportResultDto>>> GetHistory()
    {
        var history = await _importService.GetAllImportsAsync();
        return Ok(history);
    }
}