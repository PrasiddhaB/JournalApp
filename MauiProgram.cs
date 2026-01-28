using Microsoft.Extensions.Logging;
using JournalApp.Data;
using JournalApp.Repositories;
using JournalApp.Services;

namespace JournalApp;

/// <summary>
/// MAUI application entry point and dependency injection configuration.
/// Configures all services needed for the journal application.
/// </summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        
        // Add Blazor WebView
        builder.Services.AddMauiBlazorWebView();
        
        #if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
        #endif
        
        // Register Database Context
        // Using Scoped lifetime for DbContext as per EF Core best practices
        builder.Services.AddDbContext<JournalDbContext>();
        
        // Register Repositories
        // Scoped: One instance per "scope" (each page/component request in Blazor)
        builder.Services.AddScoped<IJournalRepository, JournalRepository>();
        builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
        
        // Register Services
        // Singleton: Services that maintain state across the entire app lifetime
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<ThemeService>();
        
        // Scoped: Services that may need fresh data per request
        builder.Services.AddScoped<StreakService>();
        builder.Services.AddScoped<AnalyticsService>();
        builder.Services.AddScoped<ExportPdfService>();
        
        return builder.Build();
    }
}
