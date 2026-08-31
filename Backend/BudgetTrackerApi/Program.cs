using BudgetTrackerApi.Data;
using BudgetTrackerApi.Services;
using BudgetTrackerApi.Services.Export;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // <-- Ajouté pour .MigrateAsync()

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURATION DE BASE ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// --- INFRASTRUCTURE ---
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<DatabaseSelectorService>();

// --- BASE DE DONNÉES ---
builder.Services.AddDbContext<AppDbContext>();

// --- SERVICES APPLICATIFS ---
builder.Services.AddScoped<CcOperationService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<IPeaService, PeaService>();
builder.Services.AddScoped<BalanceReportService>();
builder.Services.AddScoped<ImportService>();
builder.Services.AddScoped<DatabaseExportService>();
builder.Services.AddScoped<IRuleService, RuleService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<PatrimonyService>();
builder.Services.AddScoped<CcAdjustmentService>();
builder.Services.AddScoped<DatabaseHealthService>();

// FinanceService via HttpClient (géré en Scoped par défaut)
builder.Services.AddHttpClient<FinanceService>();

// État global (Filtres)
builder.Services.AddSingleton<FiltersState>();

var app = builder.Build();

// --- EXÉCUTION DES MIGRATIONS AU DÉMARRAGE ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
        Console.WriteLine("---> Migrations SQLite appliquées avec succès !");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"---> Erreur lors de l'application des migrations : {ex.Message}");
    }
}

// --- PIPELINE HTTP ---
app.UseCors("AllowAngular");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/reports/evolution", async ([FromServices] BalanceReportService service) =>
{
    try 
    {
        var data = await service.GetCumulatedBalanceAsync();
        return Results.Ok(data);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapControllers();

app.Run();