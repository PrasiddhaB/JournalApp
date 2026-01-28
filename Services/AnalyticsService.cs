using JournalApp.Models;
using JournalApp.Repositories;

namespace JournalApp.Services;

/// <summary>
/// Service for calculating analytics data: mood distribution, tag usage, word count trends.
/// All analytics are date-range filterable.
/// </summary>
public class AnalyticsService
{
    private readonly IJournalRepository _repository;
    private readonly StreakService _streakService;
    
    public AnalyticsService(IJournalRepository repository, StreakService streakService)
    {
        _repository = repository;
        _streakService = streakService;
    }
    
    /// <summary>
    /// Gets complete analytics data for the dashboard.
    /// </summary>
    /// <param name="startDate">Start of date range (optional, defaults to all time).</param>
    /// <param name="endDate">End of date range (optional, defaults to today).</param>
    public async Task<AnalyticsData> GetAnalyticsAsync(DateOnly? startDate = null, DateOnly? endDate = null)
    {
        // Set defaults
        var today = DateOnly.FromDateTime(DateTime.Today);
        var start = startDate ?? new DateOnly(2020, 1, 1); // Far past date
        var end = endDate ?? today;
        
        // Get all entries in range
        var entries = await _repository.GetEntriesInRangeAsync(start, end);
        
        if (!entries.Any())
        {
            return new AnalyticsData
            {
                TotalEntries = 0,
                CurrentStreak = 0,
                LongestStreak = 0
            };
        }
        
        var analytics = new AnalyticsData
        {
            TotalEntries = entries.Count,
            TotalDaysWithEntries = entries.Select(e => e.EntryDate).Distinct().Count()
        };
        
        // Calculate mood distribution
        CalculateMoodDistribution(entries, analytics);
        
        // Calculate most frequent mood
        CalculateMostFrequentMood(entries, analytics);
        
        // Get streak data
        var streakData = await _streakService.GetStreakDataAsync();
        analytics.CurrentStreak = streakData.CurrentStreak;
        analytics.LongestStreak = streakData.LongestStreak;
        analytics.MissedDays = await _streakService.GetMissedDaysInRangeAsync(start, end);
        
        // Calculate tag statistics
        await CalculateTagStatistics(entries, analytics);
        
        // Calculate word count trends
        CalculateWordCountTrends(entries, analytics);
        
        return analytics;
    }
    
    /// <summary>
    /// Calculates mood distribution by category (Positive/Neutral/Negative).
    /// </summary>
    private void CalculateMoodDistribution(List<JournalEntry> entries, AnalyticsData analytics)
    {
        var total = entries.Count;
        if (total == 0) return;
        
        foreach (var entry in entries)
        {
            if (entry.PrimaryMood == null) continue;
            
            switch (entry.PrimaryMood.Category)
            {
                case MoodCategory.Positive:
                    analytics.PositiveMoodCount++;
                    break;
                case MoodCategory.Neutral:
                    analytics.NeutralMoodCount++;
                    break;
                case MoodCategory.Negative:
                    analytics.NegativeMoodCount++;
                    break;
            }
        }
        
        analytics.PositiveMoodPercentage = Math.Round((double)analytics.PositiveMoodCount / total * 100, 1);
        analytics.NeutralMoodPercentage = Math.Round((double)analytics.NeutralMoodCount / total * 100, 1);
        analytics.NegativeMoodPercentage = Math.Round((double)analytics.NegativeMoodCount / total * 100, 1);
    }
    
    /// <summary>
    /// Finds the most frequently recorded mood.
    /// </summary>
    private void CalculateMostFrequentMood(List<JournalEntry> entries, AnalyticsData analytics)
    {
        var moodCounts = entries
            .Where(e => e.PrimaryMood != null)
            .GroupBy(e => e.PrimaryMood!.Name)
            .Select(g => new { MoodName = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();
        
        if (moodCounts != null)
        {
            analytics.MostFrequentMood = moodCounts.MoodName;
            analytics.MostFrequentMoodCount = moodCounts.Count;
        }
    }
    
    /// <summary>
    /// Calculates tag usage statistics.
    /// </summary>
    private async Task CalculateTagStatistics(List<JournalEntry> entries, AnalyticsData analytics)
    {
        var allTags = await _repository.GetAllTagsAsync();
        var tagUsage = new Dictionary<int, int>();
        
        foreach (var entry in entries)
        {
            foreach (var entryTag in entry.EntryTags)
            {
                if (!tagUsage.ContainsKey(entryTag.TagId))
                {
                    tagUsage[entryTag.TagId] = 0;
                }
                tagUsage[entryTag.TagId]++;
            }
        }
        
        var totalTagUsages = tagUsage.Values.Sum();
        
        // Most used tags (top 10)
        analytics.MostUsedTags = tagUsage
            .OrderByDescending(kvp => kvp.Value)
            .Take(10)
            .Select(kvp =>
            {
                var tag = allTags.FirstOrDefault(t => t.Id == kvp.Key);
                return new TagUsage
                {
                    TagId = kvp.Key,
                    TagName = tag?.Name ?? "Unknown",
                    Count = kvp.Value,
                    Percentage = totalTagUsages > 0 
                        ? Math.Round((double)kvp.Value / totalTagUsages * 100, 1) 
                        : 0
                };
            })
            .ToList();
        
        // Tag breakdown (all tags with percentage)
        analytics.TagBreakdown = tagUsage
            .Select(kvp =>
            {
                var tag = allTags.FirstOrDefault(t => t.Id == kvp.Key);
                return new TagUsage
                {
                    TagId = kvp.Key,
                    TagName = tag?.Name ?? "Unknown",
                    Count = kvp.Value,
                    Percentage = entries.Count > 0 
                        ? Math.Round((double)kvp.Value / entries.Count * 100, 1) 
                        : 0
                };
            })
            .OrderByDescending(t => t.Count)
            .ToList();
    }
    
    /// <summary>
    /// Calculates word count trends over time.
    /// </summary>
    private void CalculateWordCountTrends(List<JournalEntry> entries, AnalyticsData analytics)
    {
        if (!entries.Any())
        {
            analytics.AverageWordCount = 0;
            return;
        }
        
        // Order by date
        var orderedEntries = entries.OrderBy(e => e.EntryDate).ToList();
        
        // Calculate simple moving average (7-day window)
        const int windowSize = 7;
        var trends = new List<WordCountTrend>();
        
        for (int i = 0; i < orderedEntries.Count; i++)
        {
            var entry = orderedEntries[i];
            
            // Calculate moving average
            var windowStart = Math.Max(0, i - windowSize + 1);
            var windowEntries = orderedEntries.Skip(windowStart).Take(i - windowStart + 1);
            var movingAvg = windowEntries.Average(e => e.WordCount);
            
            trends.Add(new WordCountTrend
            {
                Date = entry.EntryDate,
                WordCount = entry.WordCount,
                MovingAverage = Math.Round(movingAvg, 1)
            });
        }
        
        analytics.WordCountTrends = trends;
        analytics.AverageWordCount = Math.Round(entries.Average(e => e.WordCount), 1);
    }
    
    /// <summary>
    /// Gets mood distribution specifically for pie/bar charts.
    /// </summary>
    public async Task<Dictionary<string, double>> GetMoodDistributionChartDataAsync(DateOnly? startDate = null, DateOnly? endDate = null)
    {
        var analytics = await GetAnalyticsAsync(startDate, endDate);
        
        return new Dictionary<string, double>
        {
            ["Positive"] = analytics.PositiveMoodPercentage,
            ["Neutral"] = analytics.NeutralMoodPercentage,
            ["Negative"] = analytics.NegativeMoodPercentage
        };
    }
    
    /// <summary>
    /// Gets word count data formatted for charts.
    /// </summary>
    public async Task<List<object>> GetWordCountChartDataAsync(DateOnly? startDate = null, DateOnly? endDate = null)
    {
        var analytics = await GetAnalyticsAsync(startDate, endDate);
        
        return analytics.WordCountTrends
            .Select(t => new
            {
                date = t.Date.ToString("MMM dd"),
                wordCount = t.WordCount,
                average = t.MovingAverage
            } as object)
            .ToList();
    }
}
