namespace JournalApp.Models;

/// <summary>
/// Filter parameters for searching and filtering journal entries.
/// </summary>
public class EntryFilter
{
    public string? SearchText { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public List<int>? MoodIds { get; set; }
    public List<int>? TagIds { get; set; }
    public string? Category { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// Paginated result set for timeline view.
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// Analytics data for the dashboard.
/// </summary>
public class AnalyticsData
{
    // Mood Distribution
    public int PositiveMoodCount { get; set; }
    public int NeutralMoodCount { get; set; }
    public int NegativeMoodCount { get; set; }
    public double PositiveMoodPercentage { get; set; }
    public double NeutralMoodPercentage { get; set; }
    public double NegativeMoodPercentage { get; set; }
    
    // Most Frequent Mood
    public string? MostFrequentMood { get; set; }
    public int MostFrequentMoodCount { get; set; }
    
    // Streaks
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public List<DateOnly> MissedDays { get; set; } = new();
    
    // Tag Statistics
    public List<TagUsage> MostUsedTags { get; set; } = new();
    public List<TagUsage> TagBreakdown { get; set; } = new();
    
    // Word Count Trends
    public List<WordCountTrend> WordCountTrends { get; set; } = new();
    public double AverageWordCount { get; set; }
    
    // General Stats
    public int TotalEntries { get; set; }
    public int TotalDaysWithEntries { get; set; }
}

/// <summary>
/// Tag usage statistics for charts.
/// </summary>
public class TagUsage
{
    public int TagId { get; set; }
    public string TagName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

/// <summary>
/// Word count trend data point for charts.
/// </summary>
public class WordCountTrend
{
    public DateOnly Date { get; set; }
    public int WordCount { get; set; }
    public double MovingAverage { get; set; }
}

/// <summary>
/// Streak calculation result.
/// </summary>
public class StreakData
{
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public List<DateOnly> MissedDays { get; set; } = new();
    public DateOnly? LastEntryDate { get; set; }
}
