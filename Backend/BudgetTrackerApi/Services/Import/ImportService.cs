using BudgetTrackerApi.Data;
using BudgetTrackerApi.DTOs;
using BudgetTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTrackerApi.Services
{
    public class ImportService
    {
        private readonly AppDbContext _db;
        public ImportService(AppDbContext db) => _db = db;

        public async Task<ImportResultDto> ProcessImportAsync(IFormFile file)
        {
            var startTime = DateTime.Now;
            // On prépare le log immédiatement pour pouvoir le sauver même en cas de crash
            var importLog = new CcImportLog
            {
                FileName = file.FileName,
                ImportDate = DateTime.Now,
                IsSuccessful = false // Par défaut, on changera en true à la fin
            };

            try
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                string? textContent = null;
                if (file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    using var reader = new StreamReader(memoryStream, System.Text.Encoding.UTF8, leaveOpen: true);
                    textContent = await reader.ReadToEndAsync();
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
                if (parser == null) throw new Exception("Parser non trouvé pour ce type de fichier");

                var operations = parser.Parse(ctx);
                importLog.BankName = parser.BankName;

                // 1. Filtrage par Hash pour éviter les doublons
                var existingHashes = _db.CcOperations.Select(o => o.Hash).ToHashSet();
                var filteredOps = operations.Where(op => !existingHashes.Contains(op.Hash)).ToList();

                bool hasOps = filteredOps.Any();
                
                // Mise à jour des stats du log
                importLog.TotalRows = operations.Count;
                importLog.InsertedRows = filteredOps.Count;
                importLog.DateMin = hasOps ? filteredOps.Min(o => o.Date) : null;
                importLog.DateMax = hasOps ? filteredOps.Max(o => o.Date) : null;

                // 2. Transaction pour les données
                using (var transaction = await _db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        importLog.IsSuccessful = true;
                        importLog.TempsDeTraitementMs = (DateTime.Now - startTime).TotalMilliseconds;

                        _db.CcImportLogs.Add(importLog);
                        await _db.SaveChangesAsync(); // Génère l'Id

                        if (hasOps)
                        {
                            foreach (var op in filteredOps)
                            {
                                op.ImportLogId = importLog.Id;
                            }
                            _db.CcOperations.AddRange(filteredOps);
                            await _db.SaveChangesAsync();
                        }

                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw; // Relance pour le catch global
                    }
                }

                return MapToDto(importLog);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erreur : {e.Message}");
                
                // Finalisation du log en mode échec
                importLog.IsSuccessful = false;
                importLog.MsgErreur = e.Message;
                importLog.TempsDeTraitementMs = (DateTime.Now - startTime).TotalMilliseconds;

                // Si l'objet n'a pas été ajouté à la DB avant le crash, on l'ajoute
                if (_db.Entry(importLog).State == EntityState.Detached)
                {
                    _db.CcImportLogs.Add(importLog);
                }
                
                await _db.SaveChangesAsync();

                return new ImportResultDto(
                    file.FileName, 
                    false, 
                    e.Message, 
                    0, 0, 
                    importLog.BankName, 
                    null, null, 
                    importLog.TempsDeTraitementMs, 
                    importLog.ImportDate
                );
            }
        }

        public async Task<List<ImportResultDto>> GetAllImportsAsync()
        {
            return await _db.CcImportLogs
                .OrderByDescending(i => i.ImportDate)
                .Select(i => new ImportResultDto(
                    i.FileName,
                    i.IsSuccessful, // Utilise la vraie valeur de succès
                    i.MsgErreur ?? "",
                    i.TotalRows,
                    i.InsertedRows,
                    i.BankName,
                    i.DateMin,
                    i.DateMax,
                    i.TempsDeTraitementMs,
                    i.ImportDate))
                .ToListAsync();
        }

        private ImportResultDto MapToDto(CcImportLog log)
        {
            return new ImportResultDto(
                log.FileName,
                log.IsSuccessful,
                log.MsgErreur ?? "",
                log.TotalRows,
                log.InsertedRows,
                log.BankName,
                log.DateMin,
                log.DateMax,
                log.TempsDeTraitementMs,
                log.ImportDate
            );
        }
    }
}