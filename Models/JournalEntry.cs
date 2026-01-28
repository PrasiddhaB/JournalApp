using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalApp.Models;

/// <summary>
/// Represents a single journal entry. Only one entry is allowed per day.
/// The date constraint is enforced at the database level via unique index.
/// </summary>
public class JournalEntry
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// The date of the entry (date only, no time component).
    /// Unique constraint ensures one entry per day.
    /// </summary>
    [Required]
    public DateOnly EntryDate { get; set; }
    
    /// <summary>
    /// Title of the journal entry.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// The main content of the entry. Supports Markdown formatting.
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Auto-calculated word count from content.
    /// </summary>
    public int WordCount { get; set; }
    
    /// <summary>
    /// Optional category for organizing entries (e.g., "Personal", "Work", "Travel").
    /// </summary>
    [MaxLength(100)]
    public string? Category { get; set; }
    
    /// <summary>
    /// System-generated timestamp when entry was first created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// System-generated timestamp when entry was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    // Navigation Properties
    
    /// <summary>
    /// Primary mood (required) - Foreign key to Mood table.
    /// </summary>
    [Required]
    public int PrimaryMoodId { get; set; }
    
    [ForeignKey(nameof(PrimaryMoodId))]
    public Mood? PrimaryMood { get; set; }
    
    /// <summary>
    /// First secondary mood (optional).
    /// </summary>
    public int? SecondaryMood1Id { get; set; }
    
    [ForeignKey(nameof(SecondaryMood1Id))]
    public Mood? SecondaryMood1 { get; set; }
    
    /// <summary>
    /// Second secondary mood (optional).
    /// </summary>
    public int? SecondaryMood2Id { get; set; }
    
    [ForeignKey(nameof(SecondaryMood2Id))]
    public Mood? SecondaryMood2 { get; set; }
    
    /// <summary>
    /// Tags associated with this entry (many-to-many relationship).
    /// </summary>
    public ICollection<EntryTag> EntryTags { get; set; } = new List<EntryTag>();
    
    /// <summary>
    /// Calculates word count from the content.
    /// </summary>
    public void CalculateWordCount()
    {
        if (string.IsNullOrWhiteSpace(Content))
        {
            WordCount = 0;
            return;
        }
        
        // Split on whitespace and count non-empty entries
        WordCount = Content
            .Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Length;
    }
}
