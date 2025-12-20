using Microsoft.AspNetCore.Mvc;
using BudgetTrackerApp.Services; // Adapte selon tes namespaces

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
        Console.WriteLine($"Fichier reçu : ile == null");

        // On appelle la logique que tu avais dans ton @code Blazor
        // mais désormais encapsulée dans le service
        var result = await _importService.ProcessImportAsync(file);
        Console.WriteLine($"Fichier reçu : var result");

        if (result.IsSuccessful)
        {
            Console.WriteLine($"Fichier reçu : Success");
            return Ok(result); // Envoie le JSON que l'interface FichierTraite d'Angular attend
        }

        return UnprocessableEntity(result); // Envoie le rapport même en cas d'erreur de parsing
    }
}