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

// --- SERVICES ---
builder.Services.AddScoped<CcOperationService>();
builder.Services.AddSingleton<FiltersState>();
builder.Services.AddSingleton<DatabaseSelectorService>();
builder.Services.AddSingleton<MyDataService>();
builder.Services.AddScoped<CategoryService>();

// Utilise UNIQUEMENT l'interface pour PeaService
builder.Services.AddScoped<IPeaService, PeaService>(); 

// FinanceService doit être enregistré via AddHttpClient (ne pas rajouter AddScoped après)
builder.Services.AddHttpClient<FinanceService>();

builder.Services.AddScoped<BalanceReportService>();
builder.Services.AddScoped<ImportService>();
builder.Services.AddScoped<DatabaseExportService>();
builder.Services.AddScoped<IRuleService, RuleService>();

builder.Services.AddDbContextFactory<AppDbContext>();

var app = builder.Build();

app.UseCors("AllowAngular");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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

// Active le mappage des routes des contrôleurs
app.MapControllers();

app.Run();
