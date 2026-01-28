using Microsoft.EntityFrameworkCore;
using JournalApp.Data;
using JournalApp.Models;

namespace JournalApp.Repositories;

/// <summary>
/// SQLite implementation of the journal repository.
/// Handles all database operations for journal entries, tags, and moods.
/// </summary>
public class JournalRepository : IJournalRepository
{
    private readonly JournalDbContext _context;
    
    public JournalRepository(JournalDbContext context)
    {
        _context = context;
    }
    
    #region Entry CRUD Operations
    
    public async Task<JournalEntry?> GetByIdAsync(int id)
    {
        return await _context.JournalEntries
            .Include(e => e.PrimaryMood)
            .Include(e => e.SecondaryMood1)
            .Include(e => e.SecondaryMood2)
            .Include(e => e.EntryTags)
                .ThenInclude(et => et.Tag)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
    
    public async Task<JournalEntry?> GetByDateAsync(DateOnly date)
    {
        return await _context.JournalEntries
            .Include(e => e.PrimaryMood)
            .Include(e => e.SecondaryMood1)
            .Include(e => e.SecondaryMood2)
            .Include(e => e.EntryTags)
                .ThenInclude(et => et.Tag)
            .FirstOrDefaultAsync(e => e.EntryDate == date);
    }
    
    public async Task<JournalEntry?> GetTodayEntryAsync()
    {
        return await GetByDateAsync(DateOnly.FromDateTime(DateTime.Today));
    }
    
    public async Task<JournalEntry> CreateAsync(JournalEntry entry)
    {
        // Set timestamps
        entry.CreatedAt = DateTime.UtcNow;
        entry.UpdatedAt = DateTime.UtcNow;
        
        // Calculate word count
        entry.CalculateWordCount();
        
        _context.JournalEntries.Add(entry);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return (await GetByIdAsync(entry.Id))!;
    }
    
    public async Task<JournalEntry> UpdateAsync(JournalEntry entry)
    {
        // Update timestamp
        entry.UpdatedAt = DateTime.UtcNow;
        
        // Recalculate word count
        entry.CalculateWordCount();
        
        _context.JournalEntries.Update(entry);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return (await GetByIdAsync(entry.Id))!;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var entry = await _context.JournalEntries.FindAsync(id);
        if (entry == null) return false;
        
        _context.JournalEntries.Remove(entry);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> EntryExistsAsync(DateOnly date)
    {
        return await _context.JournalEntries.AnyAsync(e => e.EntryDate == date);
    }
    
    #endregion
    
    #region Query Operations
    
    public async Task<PagedResult<JournalEntry>> GetEntriesAsync(EntryFilter filter)
    {
        var query = _context.JournalEntries
            .Include(e => e.PrimaryMood)
            .Include(e => e.SecondaryMood1)
            .Include(e => e.SecondaryMood2)
            .Include(e => e.EntryTags)
                .ThenInclude(et => et.Tag)
            .AsQueryable();
        
        // Apply search text filter (title or content)
        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            var searchLower = filter.SearchText.ToLower();
            query = query.Where(e => 
                e.Title.ToLower().Contains(searchLower) || 
                e.Content.ToLower().Contains(searchLower));
        }
        
        // Apply date range filter
        if (filter.StartDate.HasValue)
        {
            query = query.Where(e => e.EntryDate >= filter.StartDate.Value);
        }
        if (filter.EndDate.HasValue)
        {
            query = query.Where(e => e.EntryDate <= filter.EndDate.Value);
        }
        
        // Apply mood filter
        if (filter.MoodIds?.Any() == true)
        {
            query = query.Where(e => 
                filter.MoodIds.Contains(e.PrimaryMoodId) ||
                (e.SecondaryMood1Id.HasValue && filter.MoodIds.Contains(e.SecondaryMood1Id.Value)) ||
                (e.SecondaryMood2Id.HasValue && filter.MoodIds.Contains(e.SecondaryMood2Id.Value)));
        }
        
        // Apply tag filter
        if (filter.TagIds?.Any() == true)
        {
            query = query.Where(e => 
                e.EntryTags.Any(et => filter.TagIds.Contains(et.TagId)));
        }
        
        // Apply category filter
        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            query = query.Where(e => e.Category == filter.Category);
        }
        
        // Get total count before pagination
        var totalCount = await query.CountAsync();
        
        // Apply ordering (most recent first) and pagination
        var items = await query
            .OrderByDescending(e => e.EntryDate)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
        
        return new PagedResult<JournalEntry>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }
    
    public async Task<List<JournalEntry>> GetEntriesInRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        return await _context.JournalEntries
            .Include(e => e.PrimaryMood)
            .Include(e => e.SecondaryMood1)
            .Include(e => e.SecondaryMood2)
            .Include(e => e.EntryTags)
                .ThenInclude(et => et.Tag)
            .Where(e => e.EntryDate >= startDate && e.EntryDate <= endDate)
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync();
    }
    
    public async Task<List<DateOnly>> GetDatesWithEntriesAsync(int year, int month)
    {
        var startDate = new DateOnly(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        
        return await _context.JournalEntries
            .Where(e => e.EntryDate >= startDate && e.EntryDate <= endDate)
            .Select(e => e.EntryDate)
            .ToListAsync();
    }
    
    public async Task<List<DateOnly>> GetAllEntryDatesAsync()
    {
        return await _context.JournalEntries
            .Select(e => e.EntryDate)
            .OrderBy(d => d)
            .ToListAsync();
    }
    
    #endregion
    
    #region Tag Operations
    
    public async Task<List<Tag>> GetAllTagsAsync()
    {
        return await _context.Tags
            .OrderBy(t => t.IsPrebuilt ? 0 : 1) // Pre-built first
            .ThenBy(t => t.Name)
            .ToListAsync();
    }
    
    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        tag.IsPrebuilt = false;
        tag.CreatedAt = DateTime.UtcNow;
        
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        return tag;
    }
    
    public async Task<bool> DeleteTagAsync(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null || tag.IsPrebuilt) return false;
        
        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task UpdateEntryTagsAsync(int entryId, List<int> tagIds)
    {
        // Remove existing tags
        var existingTags = await _context.EntryTags
            .Where(et => et.JournalEntryId == entryId)
            .ToListAsync();
        _context.EntryTags.RemoveRange(existingTags);
        
        // Add new tags
        foreach (var tagId in tagIds.Distinct())
        {
            _context.EntryTags.Add(new EntryTag
            {
                JournalEntryId = entryId,
                TagId = tagId
            });
        }
        
        await _context.SaveChangesAsync();
    }
    
    #endregion
    
    #region Mood Operations
    
    public async Task<List<Mood>> GetAllMoodsAsync()
    {
        return await _context.Moods
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();
    }
    
    public async Task<List<Mood>> GetMoodsByCategoryAsync(MoodCategory category)
    {
        return await _context.Moods
            .Where(m => m.Category == category)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();
    }
    
    #endregion
    
    #region Category Operations
    
    public async Task<List<string>> GetAllCategoriesAsync()
    {
        return await _context.JournalEntries
            .Where(e => !string.IsNullOrEmpty(e.Category))
            .Select(e => e.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }
    
    #endregion
}
