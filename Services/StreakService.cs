using JournalApp.Models;
using JournalApp.Repositories;

namespace JournalApp.Services;

/// <summary>
/// Service for calculating journaling streaks, missed days, and consistency metrics.
/// </summary>
public class StreakService
{
    private readonly IJournalRepository _repository;
    
    public StreakService(IJournalRepository repository)
    {
        _repository = repository;
    }
    
    /// <summary>
    /// Calculates all streak-related data.
    /// </summary>
    public async Task<StreakData> GetStreakDataAsync()
    {
        var allDates = await _repository.GetAllEntryDatesAsync();
        
        if (!allDates.Any())
        {
            return new StreakData
            {
                CurrentStreak = 0,
                LongestStreak = 0,
                MissedDays = new List<DateOnly>()
            };
        }
        
        var today = DateOnly.FromDateTime(DateTime.Today);
        var sortedDates = allDates.OrderByDescending(d => d).ToList();
        
        // Calculate current streak
        int currentStreak = CalculateCurrentStreak(sortedDates, today);
        
        // Calculate longest streak
        int longestStreak = CalculateLongestStreak(allDates.OrderBy(d => d).ToList());
        
        // Calculate missed days (last 30 days by default)
        var missedDays = CalculateMissedDays(allDates, today.AddDays(-30), today);
        
        return new StreakData
        {
            CurrentStreak = currentStreak,
            LongestStreak = longestStreak,
            MissedDays = missedDays,
            LastEntryDate = sortedDates.FirstOrDefault()
        };
    }
    
    /// <summary>
    /// Calculates the current consecutive journaling streak.
    /// Streak counts from today backwards, allowing today to not have an entry yet.
    /// </summary>
    private int CalculateCurrentStreak(List<DateOnly> descendingDates, DateOnly today)
    {
        if (!descendingDates.Any()) return 0;
        
        var streak = 0;
        var checkDate = today;
        
        // If today doesn't have an entry yet, start checking from yesterday
        // but only if we're still within the same day (haven't missed yet)
        var lastEntryDate = descendingDates.First();
        
        // If last entry is not today or yesterday, streak is broken
        var daysSinceLastEntry = today.DayNumber - lastEntryDate.DayNumber;
        if (daysSinceLastEntry > 1)
        {
            return 0;
        }
        
        // Start counting from the last entry date
        checkDate = lastEntryDate;
        
        foreach (var date in descendingDates)
        {
            if (date == checkDate)
            {
                streak++;
                checkDate = checkDate.AddDays(-1);
            }
            else if (date < checkDate)
            {
                // Gap found, streak ends here
                break;
            }
        }
        
        return streak;
    }
    
    /// <summary>
    /// Calculates the longest streak ever achieved.
    /// </summary>
    private int CalculateLongestStreak(List<DateOnly> ascendingDates)
    {
        if (!ascendingDates.Any()) return 0;
        
        int longestStreak = 1;
        int currentStreak = 1;
        
        for (int i = 1; i < ascendingDates.Count; i++)
        {
            var daysDiff = ascendingDates[i].DayNumber - ascendingDates[i - 1].DayNumber;
            
            if (daysDiff == 1)
            {
                // Consecutive day
                currentStreak++;
                longestStreak = Math.Max(longestStreak, currentStreak);
            }
            else if (daysDiff > 1)
            {
                // Gap in days, reset current streak
                currentStreak = 1;
            }
            // daysDiff == 0 means duplicate date (shouldn't happen), ignore
        }
        
        return longestStreak;
    }
    
    /// <summary>
    /// Finds days without entries within a date range.
    /// </summary>
    private List<DateOnly> CalculateMissedDays(List<DateOnly> allDates, DateOnly startDate, DateOnly endDate)
    {
        var dateSet = new HashSet<DateOnly>(allDates);
        var missedDays = new List<DateOnly>();
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            // Don't count future dates or today (might still write)
            if (date >= today) continue;
            
            if (!dateSet.Contains(date))
            {
                missedDays.Add(date);
            }
        }
        
        return missedDays;
    }
    
    /// <summary>
    /// Gets missed days within a specific date range.
    /// </summary>
    public async Task<List<DateOnly>> GetMissedDaysInRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        var allDates = await _repository.GetAllEntryDatesAsync();
        return CalculateMissedDays(allDates, startDate, endDate);
    }
    
    /// <summary>
    /// Gets the consistency percentage for a date range.
    /// </summary>
    public async Task<double> GetConsistencyPercentageAsync(DateOnly startDate, DateOnly endDate)
    {
        var allDates = await _repository.GetAllEntryDatesAsync();
        var datesInRange = allDates.Where(d => d >= startDate && d <= endDate).ToList();
        
        var totalDays = endDate.DayNumber - startDate.DayNumber + 1;
        
        // Don't count future dates
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (endDate > today)
        {
            totalDays = Math.Max(1, today.DayNumber - startDate.DayNumber + 1);
        }
        
        return totalDays > 0 ? (double)datesInRange.Count / totalDays * 100 : 0;
    }
}
