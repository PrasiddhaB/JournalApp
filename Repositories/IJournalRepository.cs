using JournalApp.Models;

namespace JournalApp.Repositories;

/// <summary>
/// Repository interface for journal entry CRUD operations and queries.
/// </summary>
public interface IJournalRepository
{
    // Entry CRUD Operations
    
    /// <summary>
    /// Gets an entry by its ID including all related data.
    /// </summary>
    Task<JournalEntry?> GetByIdAsync(int id);
    
    /// <summary>
    /// Gets an entry for a specific date (only one entry per day).
    /// </summary>
    Task<JournalEntry?> GetByDateAsync(DateOnly date);
    
    /// <summary>
    /// Gets today's entry if it exists.
    /// </summary>
    Task<JournalEntry?> GetTodayEntryAsync();
    
    /// <summary>
    /// Creates a new journal entry.
    /// </summary>
    Task<JournalEntry> CreateAsync(JournalEntry entry);
    
    /// <summary>
    /// Updates an existing journal entry.
    /// </summary>
    Task<JournalEntry> UpdateAsync(JournalEntry entry);
    
    /// <summary>
    /// Deletes a journal entry.
    /// </summary>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Checks if an entry exists for a specific date.
    /// </summary>
    Task<bool> EntryExistsAsync(DateOnly date);
    
    // Query Operations
    
    /// <summary>
    /// Gets entries with pagination and optional filters.
    /// </summary>
    Task<PagedResult<JournalEntry>> GetEntriesAsync(EntryFilter filter);
    
    /// <summary>
    /// Gets all entries within a date range.
    /// </summary>
    Task<List<JournalEntry>> GetEntriesInRangeAsync(DateOnly startDate, DateOnly endDate);
    
    /// <summary>
    /// Gets all dates that have entries (for calendar view).
    /// </summary>
    Task<List<DateOnly>> GetDatesWithEntriesAsync(int year, int month);
    
    /// <summary>
    /// Gets all dates with entries in a year (for analytics).
    /// </summary>
    Task<List<DateOnly>> GetAllEntryDatesAsync();
    
    // Tag Operations
    
    /// <summary>
    /// Gets all tags (pre-built and custom).
    /// </summary>
    Task<List<Tag>> GetAllTagsAsync();
    
    /// <summary>
    /// Creates a custom tag.
    /// </summary>
    Task<Tag> CreateTagAsync(Tag tag);
    
    /// <summary>
    /// Deletes a custom tag (pre-built tags cannot be deleted).
    /// </summary>
    Task<bool> DeleteTagAsync(int id);
    
    /// <summary>
    /// Updates tags for an entry (replaces all existing tags).
    /// </summary>
    Task UpdateEntryTagsAsync(int entryId, List<int> tagIds);
    
    // Mood Operations
    
    /// <summary>
    /// Gets all moods.
    /// </summary>
    Task<List<Mood>> GetAllMoodsAsync();
    
    /// <summary>
    /// Gets moods by category.
    /// </summary>
    Task<List<Mood>> GetMoodsByCategoryAsync(MoodCategory category);
    
    // Category Operations
    
    /// <summary>
    /// Gets all unique categories used in entries.
    /// </summary>
    Task<List<string>> GetAllCategoriesAsync();
}
