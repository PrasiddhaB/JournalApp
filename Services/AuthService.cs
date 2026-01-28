using JournalApp.Repositories;

namespace JournalApp.Services;

/// <summary>
/// Service for handling application authentication (PIN/password protection).
/// Uses BCrypt for secure password hashing.
/// </summary>
public class AuthService
{
    private readonly ISettingsRepository _settingsRepository;
    private bool _isAuthenticated;
    private DateTime? _lastAuthTime;
    
    public AuthService(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }
    
    /// <summary>
    /// Gets whether the app is currently unlocked.
    /// </summary>
    public bool IsAuthenticated => _isAuthenticated;
    
    /// <summary>
    /// Checks if password protection is enabled.
    /// </summary>
    public async Task<bool> IsPasswordEnabledAsync()
    {
        var settings = await _settingsRepository.GetSettingsAsync();
        return settings.IsPasswordEnabled;
    }
    
    /// <summary>
    /// Checks if the current session is still valid.
    /// </summary>
    public async Task<bool> IsSessionValidAsync()
    {
        var settings = await _settingsRepository.GetSettingsAsync();
        
        // If no password, always valid
        if (!settings.IsPasswordEnabled)
        {
            _isAuthenticated = true;
            return true;
        }
        
        // Check if already authenticated in this session
        if (!_isAuthenticated || !_lastAuthTime.HasValue)
        {
            return false;
        }
        
        // Check session timeout
        var elapsed = DateTime.UtcNow - _lastAuthTime.Value;
        if (elapsed.TotalMinutes > settings.SessionTimeoutMinutes)
        {
            _isAuthenticated = false;
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Attempts to authenticate with the provided password/PIN.
    /// </summary>
    /// <param name="password">The password or PIN to verify.</param>
    /// <returns>True if authentication successful, false otherwise.</returns>
    public async Task<bool> AuthenticateAsync(string password)
    {
        var settings = await _settingsRepository.GetSettingsAsync();
        
        if (!settings.IsPasswordEnabled || string.IsNullOrEmpty(settings.PasswordHash))
        {
            _isAuthenticated = true;
            _lastAuthTime = DateTime.UtcNow;
            return true;
        }
        
        // Verify password using BCrypt
        var isValid = BCrypt.Net.BCrypt.Verify(password, settings.PasswordHash);
        
        if (isValid)
        {
            _isAuthenticated = true;
            _lastAuthTime = DateTime.UtcNow;
            await _settingsRepository.UpdateLastAuthenticatedAsync();
        }
        
        return isValid;
    }
    
    /// <summary>
    /// Sets a new password/PIN.
    /// </summary>
    /// <param name="password">The new password or PIN.</param>
    public async Task SetPasswordAsync(string password)
    {
        // Hash the password using BCrypt with work factor 12
        var hash = BCrypt.Net.BCrypt.HashPassword(password, 12);
        await _settingsRepository.SetPasswordAsync(hash);
        
        // Authenticate after setting password
        _isAuthenticated = true;
        _lastAuthTime = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Changes the existing password/PIN.
    /// </summary>
    /// <param name="currentPassword">The current password to verify.</param>
    /// <param name="newPassword">The new password to set.</param>
    /// <returns>True if password was changed, false if current password was wrong.</returns>
    public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        // Verify current password first
        if (!await AuthenticateAsync(currentPassword))
        {
            return false;
        }
        
        await SetPasswordAsync(newPassword);
        return true;
    }
    
    /// <summary>
    /// Removes password protection from the app.
    /// </summary>
    /// <param name="currentPassword">The current password to verify.</param>
    /// <returns>True if protection was removed, false if password was wrong.</returns>
    public async Task<bool> RemovePasswordAsync(string currentPassword)
    {
        // Verify current password first
        if (!await AuthenticateAsync(currentPassword))
        {
            return false;
        }
        
        await _settingsRepository.RemovePasswordAsync();
        _isAuthenticated = true;
        return true;
    }
    
    /// <summary>
    /// Locks the application (requires re-authentication).
    /// </summary>
    public void Lock()
    {
        _isAuthenticated = false;
        _lastAuthTime = null;
    }
    
    /// <summary>
    /// Refreshes the session timer (call on user activity).
    /// </summary>
    public void RefreshSession()
    {
        if (_isAuthenticated)
        {
            _lastAuthTime = DateTime.UtcNow;
        }
    }
}
