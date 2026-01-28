using System.ComponentModel.DataAnnotations;

namespace JournalApp.Models;

/// <summary>
/// Stores application settings including security and theme preferences.
/// Only one record exists in this table (singleton pattern).
/// </summary>
public class AppSettings
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Whether PIN/password protection is enabled.
    /// </summary>
    public bool IsPasswordEnabled { get; set; }
    
    /// <summary>
    /// BCrypt hashed PIN/password. Null if protection is disabled.
    /// </summary>
    [MaxLength(100)]
    public string? PasswordHash { get; set; }
    
    /// <summary>
    /// Current theme preference: "light" or "dark".
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Theme { get; set; } = "light";
    
    /// <summary>
    /// Last time the user successfully authenticated.
    /// Used for session management.
    /// </summary>
    public DateTime? LastAuthenticatedAt { get; set; }
    
    /// <summary>
    /// How long the app stays unlocked (in minutes) before requiring re-authentication.
    /// Default is 30 minutes.
    /// </summary>
    public int SessionTimeoutMinutes { get; set; } = 30;
    
    /// <summary>
    /// Timestamp when settings were last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
