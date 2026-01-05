using BudgetTrackerApp.Data;
using BudgetTrackerApp.DTOs;

namespace BudgetTrackerApp.Services
{
    public class ImportService
    {
        private readonly AppDbContext _db;
        public ImportService(AppDbContext db) => _db = db;

        public async Task<ImportResultDto> ProcessImportAsync(IFormFile file)
        {
            var startTime = DateTime.Now;
            try
            {
                Console.WriteLine($"memoryStream");
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                string? textContent = null;

                // On lit le texte uniquement si c'est un fichier texte/csv
                if (file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    using var reader = new StreamReader(memoryStream, System.Text.Encoding.UTF8, leaveOpen: true);
                    textContent = await reader.ReadToEndAsync();

                    // IMPORTANT : Remettre la position à 0 après la lecture du texte
                    // pour que le Parser puisse relire le Stream s'il en a besoin.
                    memoryStream.Position = 0;
                }
                
                var ctx = new ParserInputContext
                {
                    FileStream = memoryStream,
                    TextContent = textContent,
                    FileName = file.FileName,
                    ContentType = file.ContentType
                };
                var parser = ParserFactory.GetParser(ctx);

                
                if (parser == null)
                    throw new Exception("Parser non trouvé");

                Console.WriteLine($"parser = {parser.BankName}");

                var operations = parser.Parse(ctx);

                Console.WriteLine($"Parsing terminé");

                // Filtrage par Hash
                var existingHashes = _db.CcOperations.Select(o => o.Hash).ToHashSet();
                var filteredOps = operations.Where(op => !existingHashes.Contains(op.Hash)).ToList();

                _db.CcOperations.AddRange(filteredOps);
                await _db.SaveChangesAsync();

                return new ImportResultDto(file.FileName, true, "", operations.Count, filteredOps.Count,
                    parser.BankName, filteredOps.Min(o => o.Date), filteredOps.Max(o => o.Date),
                    (DateTime.Now - startTime).TotalMilliseconds);
            }
            catch (Exception e)
            {
                Console.WriteLine($"erreor : {e.Message}");
                return new ImportResultDto(file.FileName, false, e.Message, 0, 0, null, DateTime.Now, DateTime.Now, 0);
            }
        }
    }
}