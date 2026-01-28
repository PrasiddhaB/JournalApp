using JournalApp.Data;

namespace JournalApp;

/// <summary>
/// Main application class.
/// Initializes the database on first run and sets up the main window.
/// </summary>
public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Initialize database on app startup
        InitializeDatabaseAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Initializes the SQLite database and seeds initial data.
    /// Called once at application startup.
    /// </summary>
    private async Task InitializeDatabaseAsync()
    {
        try
        {
            using var context = new JournalDbContext();
            await AppInitializer.InitializeAsync(context);
        }
        catch (Exception ex)
        {
            // Log the error - in production, use proper logging
            System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
        }
    }

    /// <summary>
    /// Configure window properties for Windows desktop.
    /// </summary>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new MainPage())
        {
            Title = "Personal Journal",
            MinimumWidth = 800,
            MinimumHeight = 600,
            Width = 1200,
            Height = 800
        };

        return window;
    }
}