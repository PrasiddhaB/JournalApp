namespace JournalApp.Data;

/// <summary>
/// Helper class for managing the SQLite database file path.
/// Stores the database in the user's local application data folder for Windows.
/// </summary>
public static class DbPathHelper
{
    private const string AppFolderName = "JournalApp";
    private const string DatabaseFileName = "journal.db";
    
    /// <summary>
    /// Gets the full path to the SQLite database file.
    /// Creates the directory if it doesn't exist.
    /// </summary>
    /// <returns>Full path to the database file.</returns>
    public static string GetDatabasePath()
    {
        // Use LocalApplicationData for Windows desktop apps
        // This results in: C:\Users\{username}\AppData\Local\JournalApp\journal.db
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(localAppData, AppFolderName);
        
        // Ensure the directory exists
        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }
        
        return Path.Combine(appFolder, DatabaseFileName);
    }
    
    /// <summary>
    /// Gets the folder path where the database and exports are stored.
    /// </summary>
    /// <returns>Full path to the application data folder.</returns>
    public static string GetAppDataFolder()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(localAppData, AppFolderName);
        
        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }
        
        return appFolder;
    }
    
    /// <summary>
    /// Gets the exports folder path for PDF files.
    /// </summary>
    /// <returns>Full path to the exports folder.</returns>
    public static string GetExportsFolder()
    {
        var appFolder = GetAppDataFolder();
        var exportsFolder = Path.Combine(appFolder, "Exports");
        
        if (!Directory.Exists(exportsFolder))
        {
            Directory.CreateDirectory(exportsFolder);
        }
        
        return exportsFolder;
    }
}
