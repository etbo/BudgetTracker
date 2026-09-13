using BudgetTrackerApi.Models;
using BudgetTrackerApi.Models.LifeInsurance;
using BudgetTrackerApi.Models.Savings;
using Microsoft.EntityFrameworkCore;

namespace BudgetTrackerApi.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // S'assurer que le compte admin existe toujours
            var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            
            if (adminUser == null)
            {
                context.Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("ChangeMe123!"),
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                // Réinitialise le mot de passe au cas où le hash stocké soit corrompu
                adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword("ChangeMe123!");
            }

            await context.SaveChangesAsync();

            Console.WriteLine("---> Début de l'injection des données de test (Seed)...");

            // 1. Création des comptes
            var checkingAccount = new Account
            {
                Name = "Compte Courant BoursoBank",
                Owner = "Alex",
                BankName = "BoursoBank",
                Type = AccountType.Checking,
                IsActive = true,
                UpdateFrequencyInMonths = 1
            };

            var savingsAccount = new Account
            {
                Name = "Livret A",
                Owner = "Alex",
                BankName = "BoursoBank",
                Type = AccountType.Savings,
                IsActive = true,
                UpdateFrequencyInMonths = 1
            };

            var lifeInsuranceAccount = new Account
            {
                Name = "Assurance Vie Linxea",
                Owner = "Alex",
                BankName = "Linxea",
                Type = AccountType.LifeInsurance,
                IsActive = true,
                UpdateFrequencyInMonths = 1
            };

            await context.Accounts.AddRangeAsync(checkingAccount, savingsAccount, lifeInsuranceAccount);
            await context.SaveChangesAsync();

            // 2. Journal d'import pour les opérations de compte courant
            var today = DateTime.Today;
            var importLog = new CcImportLog
            {
                FileName = "dummy_seed_data.csv",
                ImportDate = today,
                IsSuccessful = true,
                MsgErreur = string.Empty,
                BankName = checkingAccount.BankName,
                ProcessingTimeMs = 25.0
            };
            context.CcImportLogs.Add(importLog);
            await context.SaveChangesAsync();

            // 3. Opérations sur les 3 derniers mois pour le compte courant
            var operations = new List<CcOperation>();

            for (int monthOffset = -2; monthOffset <= 0; monthOffset++)
            {
                var targetDate = today.AddMonths(monthOffset);
                var year = targetDate.Year;
                var month = targetDate.Month;

                // Salaire
                operations.Add(new CcOperation
                {
                    Date = new DateTime(year, month, 1),
                    Description = "Virement Salaire Entreprise ACME",
                    Amount = 3100.00,
                    Category = "Revenu",
                    Comment = "Salaire mensuel net",
                    Bank = checkingAccount.BankName,
                    ImportLogId = importLog.Id,
                    Hash = Guid.NewGuid().ToString("N")
                });

                // Prêt immo
                operations.Add(new CcOperation
                {
                    Date = new DateTime(year, month, 3),
                    Description = "Prélèvement Prêt Immobilier",
                    Amount = -920.00,
                    Category = "Prêt",
                    Comment = "Échéance prêt",
                    Bank = checkingAccount.BankName,
                    ImportLogId = importLog.Id,
                    Hash = Guid.NewGuid().ToString("N")
                });

                // Factures
                operations.Add(new CcOperation
                {
                    Date = new DateTime(year, month, 5),
                    Description = "Prélèvement EDF Electricité",
                    Amount = -78.40,
                    Category = "Factures",
                    Comment = "Facture d'énergie",
                    Bank = checkingAccount.BankName,
                    ImportLogId = importLog.Id,
                    Hash = Guid.NewGuid().ToString("N")
                });

                operations.Add(new CcOperation
                {
                    Date = new DateTime(year, month, 7),
                    Description = "Abonnement Freebox Fibre",
                    Amount = -39.99,
                    Category = "Factures",
                    Comment = "Internet",
                    Bank = checkingAccount.BankName,
                    ImportLogId = importLog.Id,
                    Hash = Guid.NewGuid().ToString("N")
                });

                // Transport
                operations.Add(new CcOperation
                {
                    Date = new DateTime(year, month, 10),
                    Description = "Forfait Navigo Île-de-France Mobilités",
                    Amount = -86.40,
                    Category = "Transport",
                    Comment = "Pass mensuel",
                    Bank = checkingAccount.BankName,
                    ImportLogId = importLog.Id,
                    Hash = Guid.NewGuid().ToString("N")
                });

                // Courses
                operations.Add(new CcOperation
                {
                    Date = new DateTime(year, month, 12),
                    Description = "Courses Carrefour Market",
                    Amount = -115.30,
                    Category = "Courses",
                    Comment = "Ravitaillement semaine",
                    Bank = checkingAccount.BankName,
                    ImportLogId = importLog.Id,
                    Hash = Guid.NewGuid().ToString("N")
                });

                operations.Add(new CcOperation
                {
                    Date = new DateTime(year, month, 19),
                    Description = "Courses Monoprix Bio",
                    Amount = -84.60,
                    Category = "Courses",
                    Comment = string.Empty,
                    Bank = checkingAccount.BankName,
                    ImportLogId = importLog.Id,
                    Hash = Guid.NewGuid().ToString("N")
                });

                operations.Add(new CcOperation
                {
                    Date = new DateTime(year, month, 25),
                    Description = "Courses Lidl",
                    Amount = -68.90,
                    Category = "Courses",
                    Comment = string.Empty,
                    Bank = checkingAccount.BankName,
                    ImportLogId = importLog.Id,
                    Hash = Guid.NewGuid().ToString("N")
                });

                // Loisir & Sorties
                operations.Add(new CcOperation
                {
                    Date = new DateTime(year, month, 14),
                    Description = "Restaurant Le Bistrot Gourmand",
                    Amount = -54.00,
                    Category = "Loisir",
                    Comment = "Sortie weekend",
                    Bank = checkingAccount.BankName,
                    ImportLogId = importLog.Id,
                    Hash = Guid.NewGuid().ToString("N")
                });

                operations.Add(new CcOperation
                {
                    Date = new DateTime(year, month, 21),
                    Description = "Cinéma UGC & Popcorn",
                    Amount = -26.50,
                    Category = "Loisir",
                    Comment = string.Empty,
                    Bank = checkingAccount.BankName,
                    ImportLogId = importLog.Id,
                    Hash = Guid.NewGuid().ToString("N")
                });

                // Santé
                operations.Add(new CcOperation
                {
                    Date = new DateTime(year, month, 16),
                    Description = "Pharmacie Centrale",
                    Amount = -21.40,
                    Category = "Santé",
                    Comment = string.Empty,
                    Bank = checkingAccount.BankName,
                    ImportLogId = importLog.Id,
                    Hash = Guid.NewGuid().ToString("N")
                });

                // Maison / Équipement ou Vêtements
                if (monthOffset % 2 == 0)
                {
                    operations.Add(new CcOperation
                    {
                        Date = new DateTime(year, month, 23),
                        Description = "IKEA Décoration & Rangement",
                        Amount = -135.00,
                        Category = "Maison/Equip.",
                        Comment = "Ameublement",
                        Bank = checkingAccount.BankName,
                        ImportLogId = importLog.Id,
                        Hash = Guid.NewGuid().ToString("N")
                    });
                }
                else
                {
                    operations.Add(new CcOperation
                    {
                        Date = new DateTime(year, month, 23),
                        Description = "Boutique Uniqlo Vêtements",
                        Amount = -79.90,
                        Category = "Vêtements",
                        Comment = string.Empty,
                        Bank = checkingAccount.BankName,
                        ImportLogId = importLog.Id,
                        Hash = Guid.NewGuid().ToString("N")
                    });
                }
            }

            // Mettre à jour les métadonnées de l'import log
            importLog.TotalRows = operations.Count;
            importLog.InsertedRows = operations.Count;
            importLog.DateMin = operations.Min(o => o.Date);
            importLog.DateMax = operations.Max(o => o.Date);

            await context.CcOperations.AddRangeAsync(operations);

            // 4. Relevés d'épargne (Livret A) sur les 6 derniers mois
            var savingStatements = new List<SavingStatement>
            {
                new() { AccountId = savingsAccount.Id, Date = today.AddMonths(-5), Amount = 7500m, Note = "Solde mensuel" },
                new() { AccountId = savingsAccount.Id, Date = today.AddMonths(-4), Amount = 7800m, Note = "Solde mensuel" },
                new() { AccountId = savingsAccount.Id, Date = today.AddMonths(-3), Amount = 8100m, Note = "Solde mensuel" },
                new() { AccountId = savingsAccount.Id, Date = today.AddMonths(-2), Amount = 8450m, Note = "Solde mensuel" },
                new() { AccountId = savingsAccount.Id, Date = today.AddMonths(-1), Amount = 8800m, Note = "Solde mensuel" },
                new() { AccountId = savingsAccount.Id, Date = today, Amount = 9200m, Note = "Solde mensuel" }
            };
            await context.SavingStatements.AddRangeAsync(savingStatements);

            // 5. Assurance Vie (Lignes + Relevés)
            var euroFundLine = new LifeInsuranceLine
            {
                AccountId = lifeInsuranceAccount.Id,
                Label = "Fonds Euro Suravenir",
                IsScpi = false
            };

            var scpiLine = new LifeInsuranceLine
            {
                AccountId = lifeInsuranceAccount.Id,
                Label = "SCPI Primovie",
                IsScpi = true
            };

            await context.LifeInsuranceLines.AddRangeAsync(euroFundLine, scpiLine);
            await context.SaveChangesAsync();

            var lifeInsuranceStatements = new List<LifeInsuranceStatement>
            {
                // Fonds Euro
                new() { LifeInsuranceLineId = euroFundLine.Id, Date = today.AddMonths(-2), UnitCount = 1, UnitValue = 12000m },
                new() { LifeInsuranceLineId = euroFundLine.Id, Date = today.AddMonths(-1), UnitCount = 1, UnitValue = 12050m },
                new() { LifeInsuranceLineId = euroFundLine.Id, Date = today, UnitCount = 1, UnitValue = 12100m },

                // SCPI
                new() { LifeInsuranceLineId = scpiLine.Id, Date = today.AddMonths(-2), UnitCount = 40, UnitValue = 203m },
                new() { LifeInsuranceLineId = scpiLine.Id, Date = today.AddMonths(-1), UnitCount = 40, UnitValue = 203m },
                new() { LifeInsuranceLineId = scpiLine.Id, Date = today, UnitCount = 40, UnitValue = 205m }
            };
            await context.LifeInsuranceStatements.AddRangeAsync(lifeInsuranceStatements);

            // 6. PEA (Opérations et cours en cache)
            var peaOperations = new List<PeaOperation>
            {
                new()
                {
                    Owner = "Alex",
                    Date = today.AddMonths(-4),
                    Code = "CW8",
                    Quantity = 10,
                    GrossUnitAmount = 440.0,
                    NetAmount = 4400.0
                },
                new()
                {
                    Owner = "Alex",
                    Date = today.AddMonths(-2),
                    Code = "CW8",
                    Quantity = 5,
                    GrossUnitAmount = 460.0,
                    NetAmount = 2300.0
                },
                new()
                {
                    Owner = "Alex",
                    Date = today.AddMonths(-1),
                    Code = "Appro",
                    Quantity = 600,
                    GrossUnitAmount = 1.0,
                    NetAmount = 600.0
                }
            };
            await context.PeaOperations.AddRangeAsync(peaOperations);

            var cachedStockPrice = new PeaCachedStockPrice
            {
                Ticker = "CW8",
                Date = today,
                Price = 482.50m,
                CacheTimestamp = DateTime.UtcNow
            };
            await context.PeaCachedStockPrices.AddAsync(cachedStockPrice);

            // Sauvegarde finale
            await context.SaveChangesAsync();
            Console.WriteLine("---> Données de test (Seed) insérées avec succès !");
        }
    }
}

