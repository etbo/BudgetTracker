using System.IO.Compression;

namespace BudgetTrackerApp.Services.Export
{
    public class DatabaseExportService
    {
        private readonly IWebHostEnvironment _env;

        public DatabaseExportService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public byte[] ExportDatabaseAsZip()
        {
            // Localisation du fichier SQLite
            string dbPath = Path.Combine(_env.ContentRootPath, "Database", "BudgetTracker.db");

            using var memoryStream = new MemoryStream();
            using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var entry = zip.CreateEntry("BudgetTracker.db", CompressionLevel.Fastest);

                using var entryStream = entry.Open();
                using var fileStream = File.OpenRead(dbPath);

                fileStream.CopyTo(entryStream);
            }

            return memoryStream.ToArray();
        }
    }
}