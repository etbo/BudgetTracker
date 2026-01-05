using System.IO.Compression;

namespace BudgetTrackerApi.Services.Export
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
            string rawPath = Path.Combine(_env.ContentRootPath, "..", "Database", "BudgetTracker.db");
            string dbPath = Path.GetFullPath(rawPath);

            if (!File.Exists(dbPath))
                throw new FileNotFoundException($"Fichier introuvable {dbPath}");

            using var memoryStream = new MemoryStream();
            using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var entry = zip.CreateEntry("BudgetTracker.db", CompressionLevel.Optimal);

                using var entryStream = entry.Open();
                // OUVERTURE SÉCURISÉE : FileShare.ReadWrite est crucial ici
                using var fileStream = new FileStream(dbPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                fileStream.CopyTo(entryStream);
            }

            return memoryStream.ToArray();
        }
    }
}