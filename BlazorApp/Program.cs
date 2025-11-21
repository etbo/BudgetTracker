global using System.Globalization;
using Microsoft.EntityFrameworkCore;
using BlazorApp.Components;
using BlazorApp.Data;
using BlazorApp.Data.Services;
using BlazorApp.Services;
using BlazorApp.Data.Services.Export;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<AppDbContext>();

// Add of custom CompteCourant service, defined in Data/Services
builder.Services.AddScoped<CompteCourantService>();

// Service pour les filtres lors des requêtes :
builder.Services.AddSingleton<FiltersState>();

// Service pour gérer le choix de la Database entre production et test
builder.Services.AddSingleton<DatabaseSelectorService>();

builder.Services.AddSingleton<MyDataService>();

builder.Services.AddScoped<CategoryService>();

builder.Services.AddScoped<DatabaseExportService>();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
