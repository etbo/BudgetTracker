using BudgetTrackerApi.Data;
using BudgetTrackerApi.Services;
using BudgetTrackerApi.Services.Export;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using BudgetTrackerApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICES ---
builder.Services.AddControllers();

// N'utilise QUE Swashbuckle (supprime AddOpenApi() qui entre en conflit)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Optionnel : Permet de renseigner le Cookie de session directement dans SwaggerUI
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "BudgetTracker API", Version = "v1" });
});

// --- 2. CORS ---
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

// --- 3. AUTHENTIFICATION PAR COOKIE ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "BudgetTracker_Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

// --- 4. BASE DE DONNÉES & SERVICES ---
builder.Services.AddDbContext<AppDbContext>();

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

builder.Services.AddHttpClient<FinanceService>();
builder.Services.AddSingleton<FiltersState>();

var app = builder.Build();

// --- 5. SEEDING ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();

        if (!await context.Users.AnyAsync())
        {
            var defaultAdmin = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("ChangeMe123!")
            };
            context.Users.Add(defaultAdmin);
            await context.SaveChangesAsync();
        }

        if (app.Environment.IsDevelopment())
        {
            await DbInitializer.SeedAsync(context);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"---> Erreur : {ex.Message}");
    }
}

// --- 6. PIPELINE PIPELINE (Swagger configuré explicitement) ---

app.UseCors("AllowAngular");

if (app.Environment.IsDevelopment())
{
    // Configure Swagger Middleware
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BudgetTracker API v1");
        c.RoutePrefix = "swagger"; // L'UI sera accessible sur /swagger
    });

    app.MapPost("/api/dev/reset-and-seed", async (AppDbContext context) =>
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
        await DbInitializer.SeedAsync(context);
        return Results.Ok(new { message = "Base réinitialisée." });
    }).WithTags("Dev").AllowAnonymous();
}

app.UseAuthentication();
app.UseAuthorization();

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