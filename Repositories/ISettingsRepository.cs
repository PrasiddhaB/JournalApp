using JournalApp.Models;

namespace JournalApp.Repositories;

/// <summary>
/// Repository interface for application settings operations.
/// </summary>
public interface ISettingsRepository
{
    /// <summary>
    /// Gets the current application settings.
    /// </summary>
    Task<AppSettings> GetSettingsAsync();
    
    /// <summary>
    /// Updates the application settings.
    /// </summary>
    Task<AppSettings> UpdateSettingsAsync(AppSettings settings);
    
    /// <summary>
    /// Sets or updates the password/PIN.
    /// </summary>
    Task SetPasswordAsync(string passwordHash);
    
    /// <summary>
    /// Removes the password/PIN protection.
    /// </summary>
    Task RemovePasswordAsync();
    
    /// <summary>
    /// Updates the theme preference.
    /// </summary>
    Task SetThemeAsync(string theme);
    
    /// <summary>
    /// Updates the last authenticated timestamp.
    /// </summary>
    Task UpdateLastAuthenticatedAsync();
}
