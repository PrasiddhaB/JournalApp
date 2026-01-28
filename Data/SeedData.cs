using JournalApp.Models;

namespace JournalApp.Data;

/// <summary>
/// Provides seed data for moods and pre-built tags.
/// Called on first application run to populate the database.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Returns all predefined moods with their categories and emojis.
    /// </summary>
    public static List<Mood> GetMoods()
    {
        return new List<Mood>
        {
            // Positive Moods
            new() { Id = 1, Name = "Happy", Category = MoodCategory.Positive, Emoji = "😊", DisplayOrder = 1 },
            new() { Id = 2, Name = "Excited", Category = MoodCategory.Positive, Emoji = "🤩", DisplayOrder = 2 },
            new() { Id = 3, Name = "Relaxed", Category = MoodCategory.Positive, Emoji = "😌", DisplayOrder = 3 },
            new() { Id = 4, Name = "Grateful", Category = MoodCategory.Positive, Emoji = "🙏", DisplayOrder = 4 },
            new() { Id = 5, Name = "Confident", Category = MoodCategory.Positive, Emoji = "💪", DisplayOrder = 5 },
            
            // Neutral Moods
            new() { Id = 6, Name = "Calm", Category = MoodCategory.Neutral, Emoji = "😐", DisplayOrder = 6 },
            new() { Id = 7, Name = "Thoughtful", Category = MoodCategory.Neutral, Emoji = "🤔", DisplayOrder = 7 },
            new() { Id = 8, Name = "Curious", Category = MoodCategory.Neutral, Emoji = "🧐", DisplayOrder = 8 },
            new() { Id = 9, Name = "Nostalgic", Category = MoodCategory.Neutral, Emoji = "💭", DisplayOrder = 9 },
            new() { Id = 10, Name = "Bored", Category = MoodCategory.Neutral, Emoji = "😑", DisplayOrder = 10 },
            
            // Negative Moods
            new() { Id = 11, Name = "Sad", Category = MoodCategory.Negative, Emoji = "😢", DisplayOrder = 11 },
            new() { Id = 12, Name = "Angry", Category = MoodCategory.Negative, Emoji = "😠", DisplayOrder = 12 },
            new() { Id = 13, Name = "Stressed", Category = MoodCategory.Negative, Emoji = "😰", DisplayOrder = 13 },
            new() { Id = 14, Name = "Lonely", Category = MoodCategory.Negative, Emoji = "😔", DisplayOrder = 14 },
            new() { Id = 15, Name = "Anxious", Category = MoodCategory.Negative, Emoji = "😟", DisplayOrder = 15 }
        };
    }
    
    /// <summary>
    /// Returns all pre-built tags as specified in requirements.
    /// </summary>
    public static List<Tag> GetPrebuiltTags()
    {
        var now = DateTime.UtcNow;
        var tags = new List<string>
        {
            // Work & Career
            "Work", "Career", "Studies", "Projects", "Planning",
            
            // Relationships
            "Family", "Friends", "Relationships", "Parenting",
            
            // Health & Wellness
            "Health", "Fitness", "Exercise", "Self-care", "Meditation", "Yoga",
            
            // Personal Development
            "Personal Growth", "Reflection", "Reading", "Writing",
            
            // Lifestyle
            "Hobbies", "Travel", "Nature", "Music", "Cooking", "Shopping",
            
            // Life Events
            "Birthday", "Holiday", "Vacation", "Celebration",
            
            // Other
            "Finance", "Spirituality"
        };
        
        return tags.Select((name, index) => new Tag
        {
            Id = index + 1,
            Name = name,
            IsPrebuilt = true,
            CreatedAt = now,
            Color = GetTagColor(name)
        }).ToList();
    }
    
    /// <summary>
    /// Returns a color code for a tag based on its category.
    /// </summary>
    private static string GetTagColor(string tagName)
    {
        return tagName switch
        {
            // Work & Career - Blue tones
            "Work" or "Career" or "Studies" or "Projects" or "Planning" => "#3B82F6",
            
            // Relationships - Pink/Red tones
            "Family" or "Friends" or "Relationships" or "Parenting" => "#EC4899",
            
            // Health & Wellness - Green tones
            "Health" or "Fitness" or "Exercise" or "Self-care" or "Meditation" or "Yoga" => "#10B981",
            
            // Personal Development - Purple tones
            "Personal Growth" or "Reflection" or "Reading" or "Writing" => "#8B5CF6",
            
            // Lifestyle - Orange tones
            "Hobbies" or "Travel" or "Nature" or "Music" or "Cooking" or "Shopping" => "#F59E0B",
            
            // Life Events - Yellow tones
            "Birthday" or "Holiday" or "Vacation" or "Celebration" => "#EAB308",
            
            // Other - Gray tones
            _ => "#6B7280"
        };
    }
}
