using BudgetTrackerApp.Data;
using BudgetTrackerApp.Services;
using BudgetTrackerApp.Services.Export;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

builder.Services.AddScoped<ImportService>();

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

// Active le mappage des routes des contrôleurs
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
