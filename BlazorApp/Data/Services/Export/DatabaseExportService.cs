using System.IO.Compression;

namespace BlazorApp.Data.Services.Export
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

            Console.WriteLine($"dbPath = {dbPath}");

            using var memoryStream = new MemoryStream();
            Console.WriteLine($"new ZipArchive");
            using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                Console.WriteLine($"CreateEntry");
                var entry = zip.CreateEntry("BudgetTracker.db", CompressionLevel.Fastest);

                Console.WriteLine($"Open");
                using var entryStream = entry.Open();
                using var fileStream = File.OpenRead(dbPath);

                Console.WriteLine($"Copy stream");
                fileStream.CopyTo(entryStream);
            }

            Console.WriteLine("Return");
            return memoryStream.ToArray();
        }
    }
}