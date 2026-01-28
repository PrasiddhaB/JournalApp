using Microsoft.EntityFrameworkCore;
using JournalApp.Data;
using JournalApp.Models;

namespace JournalApp.Repositories;

/// <summary>
/// SQLite implementation for application settings operations.
/// </summary>
public class SettingsRepository : ISettingsRepository
{
    private readonly JournalDbContext _context;
    
    public SettingsRepository(JournalDbContext context)
    {
        _context = context;
    }
    
    public async Task<AppSettings> GetSettingsAsync()
    {
        // Settings table has only one record with Id = 1
        var settings = await _context.AppSettings.FirstOrDefaultAsync();
        
        if (settings == null)
        {
            // Create default settings if not found
            settings = new AppSettings
            {
                Id = 1,
                IsPasswordEnabled = false,
                Theme = "light",
                SessionTimeoutMinutes = 30,
                UpdatedAt = DateTime.UtcNow
            };
            _context.AppSettings.Add(settings);
            await _context.SaveChangesAsync();
        }
        
        return settings;
    }
    
    public async Task<AppSettings> UpdateSettingsAsync(AppSettings settings)
    {
        settings.UpdatedAt = DateTime.UtcNow;
        _context.AppSettings.Update(settings);
        await _context.SaveChangesAsync();
        return settings;
    }
    
    public async Task SetPasswordAsync(string passwordHash)
    {
        var settings = await GetSettingsAsync();
        settings.PasswordHash = passwordHash;
        settings.IsPasswordEnabled = true;
        await UpdateSettingsAsync(settings);
    }
    
    public async Task RemovePasswordAsync()
    {
        var settings = await GetSettingsAsync();
        settings.PasswordHash = null;
        settings.IsPasswordEnabled = false;
        await UpdateSettingsAsync(settings);
    }
    
    public async Task SetThemeAsync(string theme)
    {
        var settings = await GetSettingsAsync();
        settings.Theme = theme;
        await UpdateSettingsAsync(settings);
    }
    
    public async Task UpdateLastAuthenticatedAsync()
    {
        var settings = await GetSettingsAsync();
        settings.LastAuthenticatedAt = DateTime.UtcNow;
        await UpdateSettingsAsync(settings);
    }
}
