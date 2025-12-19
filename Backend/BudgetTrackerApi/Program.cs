using BudgetTrackerApp.Data;
using BudgetTrackerApp.Services;
using BudgetTrackerApp.Services.Export;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContextFactory<AppDbContext>();

// Add of custom CompteCourant service, defined in Data/Services
builder.Services.AddScoped<OperationCCService>();

// Service pour les filtres lors des requêtes :
builder.Services.AddSingleton<FiltersState>();

// Service pour gérer le choix de la Database entre production et test
builder.Services.AddSingleton<DatabaseSelectorService>();

builder.Services.AddSingleton<MyDataService>();

builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<PeaService>();
builder.Services.AddHttpClient<FinanceService>();
builder.Services.AddScoped<BalanceReportService>();

builder.Services.AddScoped<DatabaseExportService>();

var app = builder.Build();

app.UseCors("AllowAngular");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

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

app.MapPost("/api/imports/upload", async (IFormFile file) =>
{
    if (file == null || file.Length == 0)
        return Results.BadRequest("Fichier vide.");

    Console.WriteLine($"Upload fichier");
    // Ici, vous réutiliserez votre logique CsvHelper que vous aviez dans Blazor
    // 1. Lire le stream du fichier
    // 2. Parser avec CsvHelper
    // 3. Enregistrer en base
    
    return Results.Ok(new { message = "Fichier reçu !" });
})
.DisableAntiforgery(); // Important pour les uploads en Minimal API si vous n'avez pas configuré le CSRF

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
