using JournalApp.Repositories;

namespace JournalApp.Services;

/// <summary>
/// Service for managing application theme (light/dark mode).
/// </summary>
public class ThemeService
{
    private readonly ISettingsRepository _settingsRepository;
    private string _currentTheme = "light";
    
    /// <summary>
    /// Event raised when the theme changes.
    /// </summary>
    public event Action<string>? OnThemeChanged;
    
    public ThemeService(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }
    
    /// <summary>
    /// Gets the current theme.
    /// </summary>
    public string CurrentTheme => _currentTheme;
    
    /// <summary>
    /// Gets whether dark mode is active.
    /// </summary>
    public bool IsDarkMode => _currentTheme == "dark";
    
    /// <summary>
    /// Initializes the theme from saved settings.
    /// Should be called at application startup.
    /// </summary>
    public async Task InitializeAsync()
    {
        var settings = await _settingsRepository.GetSettingsAsync();
        _currentTheme = settings.Theme;
        OnThemeChanged?.Invoke(_currentTheme);
    }
    
    /// <summary>
    /// Sets the theme and persists to database.
    /// </summary>
    /// <param name="theme">"light" or "dark"</param>
    public async Task SetThemeAsync(string theme)
    {
        if (theme != "light" && theme != "dark")
        {
            theme = "light";
        }
        
        _currentTheme = theme;
        await _settingsRepository.SetThemeAsync(theme);
        OnThemeChanged?.Invoke(_currentTheme);
    }
    
    /// <summary>
    /// Toggles between light and dark mode.
    /// </summary>
    public async Task ToggleThemeAsync()
    {
        var newTheme = _currentTheme == "light" ? "dark" : "light";
        await SetThemeAsync(newTheme);
    }
    
    /// <summary>
    /// Gets CSS class for the current theme.
    /// </summary>
    public string GetThemeClass()
    {
        return _currentTheme == "dark" ? "dark-theme" : "light-theme";
    }
}
