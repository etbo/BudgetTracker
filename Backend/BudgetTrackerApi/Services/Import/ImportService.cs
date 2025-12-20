using BudgetTrackerApp.Data;

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
                using var memoryStream = new MemoryStream();
                await file.OpenReadStream().CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // ... Votre logique de Factory et de Parser (identique à Blazor) ...
                var ctx = new ParserInputContext
                {
                    FileStream = memoryStream,
                    FileName = file.FileName
                };
                var parser = ParserFactory.GetParser(ctx);

                if (parser == null)
                    throw new Exception("Parser non trouvé");
                    
                var operations = parser.Parse(ctx);

                // Filtrage par Hash
                var existingHashes = _db.OperationsCC.Select(o => o.Hash).ToHashSet();
                var filteredOps = operations.Where(op => !existingHashes.Contains(op.Hash)).ToList();

                _db.OperationsCC.AddRange(filteredOps);
                await _db.SaveChangesAsync();

                return new ImportResultDto(file.FileName, true, "", operations.Count, filteredOps.Count,
                    parser.BankName, filteredOps.Min(o => o.Date), filteredOps.Max(o => o.Date),
                    (DateTime.Now - startTime).TotalMilliseconds);
            }
            catch (Exception e)
            {
                Console.WriteLine($"erreor : {e.Message}");
                return new ImportResultDto(file.FileName, false, e.Message, 0, 0, null, null, null, 0);
            }
        }
    }
}