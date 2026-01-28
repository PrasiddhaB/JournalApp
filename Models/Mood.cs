using System.ComponentModel.DataAnnotations;

namespace JournalApp.Models;

/// <summary>
/// Categories for moods - used for analytics grouping.
/// </summary>
public enum MoodCategory
{
    Positive = 1,
    Neutral = 2,
    Negative = 3
}

/// <summary>
/// Represents a mood option that can be selected for journal entries.
/// Moods are seeded with predefined values and categorized for analytics.
/// </summary>
public class Mood
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the mood (e.g., "Happy", "Sad", "Calm").
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Category of the mood for grouping in analytics.
    /// </summary>
    [Required]
    public MoodCategory Category { get; set; }
    
    /// <summary>
    /// Optional emoji representation of the mood.
    /// </summary>
    [MaxLength(10)]
    public string? Emoji { get; set; }
    
    /// <summary>
    /// Display order for UI presentation.
    /// </summary>
    public int DisplayOrder { get; set; }
}
