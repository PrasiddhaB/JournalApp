using System.ComponentModel.DataAnnotations;

namespace JournalApp.Models;

/// <summary>
/// Represents a tag that can be applied to journal entries.
/// Tags can be pre-built (system-defined) or custom (user-defined).
/// </summary>
public class Tag
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the tag (e.g., "Work", "Health", "Travel").
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether this tag is a system pre-built tag or user-created.
    /// Pre-built tags cannot be deleted by users.
    /// </summary>
    public bool IsPrebuilt { get; set; }
    
    /// <summary>
    /// Optional color code for UI display (hex format, e.g., "#FF5733").
    /// </summary>
    [MaxLength(20)]
    public string? Color { get; set; }
    
    /// <summary>
    /// Timestamp when the tag was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Entries that have this tag (many-to-many relationship).
    /// </summary>
    public ICollection<EntryTag> EntryTags { get; set; } = new List<EntryTag>();
}
