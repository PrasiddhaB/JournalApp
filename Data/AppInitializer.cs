using Microsoft.EntityFrameworkCore;

namespace JournalApp.Data;

/// <summary>
/// Handles database initialization, migrations, and seeding.
/// Called once at application startup.
/// </summary>
public static class AppInitializer
{
    /// <summary>
    /// Initializes the database, applies migrations, and seeds data if needed.
    /// </summary>
    /// <param name="context">The database context to initialize.</param>
    public static async Task InitializeAsync(JournalDbContext context)
    {
        // Ensure the database is created
        await context.Database.EnsureCreatedAsync();
        
        // Seed moods if table is empty
        if (!await context.Moods.AnyAsync())
        {
            var moods = SeedData.GetMoods();
            await context.Moods.AddRangeAsync(moods);
            await context.SaveChangesAsync();
        }
        
        // Seed pre-built tags if table is empty
        if (!await context.Tags.AnyAsync())
        {
            var tags = SeedData.GetPrebuiltTags();
            await context.Tags.AddRangeAsync(tags);
            await context.SaveChangesAsync();
        }
        
        // Ensure AppSettings record exists (seeded via HasData, but double-check)
        if (!await context.AppSettings.AnyAsync())
        {
            context.AppSettings.Add(new Models.AppSettings
            {
                Id = 1,
                IsPasswordEnabled = false,
                Theme = "light",
                SessionTimeoutMinutes = 30,
                UpdatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }
    }
    
    /// <summary>
    /// Resets the database (for development/testing only).
    /// </summary>
    /// <param name="context">The database context.</param>
    public static async Task ResetDatabaseAsync(JournalDbContext context)
    {
        await context.Database.EnsureDeletedAsync();
        await InitializeAsync(context);
    }
}
