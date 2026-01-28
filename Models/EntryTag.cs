using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JournalApp.Models;

/// <summary>
/// Join table for many-to-many relationship between JournalEntry and Tag.
/// Allows entries to have multiple tags and tags to be applied to multiple entries.
/// </summary>
public class EntryTag
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Foreign key to the journal entry.
    /// </summary>
    [Required]
    public int JournalEntryId { get; set; }
    
    [ForeignKey(nameof(JournalEntryId))]
    public JournalEntry? JournalEntry { get; set; }
    
    /// <summary>
    /// Foreign key to the tag.
    /// </summary>
    [Required]
    public int TagId { get; set; }
    
    [ForeignKey(nameof(TagId))]
    public Tag? Tag { get; set; }
}
