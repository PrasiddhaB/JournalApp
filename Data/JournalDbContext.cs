using Microsoft.EntityFrameworkCore;
using JournalApp.Models;

namespace JournalApp.Data;

/// <summary>
/// Entity Framework Core database context for the journal application.
/// Configures SQLite storage with appropriate constraints and indexes.
/// </summary>
public class JournalDbContext : DbContext
{
    private readonly string _dbPath;
    
    public JournalDbContext()
    {
        _dbPath = DbPathHelper.GetDatabasePath();
    }
    
    public JournalDbContext(DbContextOptions<JournalDbContext> options) : base(options)
    {
        _dbPath = DbPathHelper.GetDatabasePath();
    }
    
    // DbSets for each entity
    public DbSet<JournalEntry> JournalEntries { get; set; } = null!;
    public DbSet<Mood> Moods { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<EntryTag> EntryTags { get; set; } = null!;
    public DbSet<AppSettings> AppSettings { get; set; } = null!;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // JournalEntry configuration
        modelBuilder.Entity<JournalEntry>(entity =>
        {
            // CRITICAL: Unique index on EntryDate ensures one entry per day
            entity.HasIndex(e => e.EntryDate)
                  .IsUnique()
                  .HasDatabaseName("IX_JournalEntries_EntryDate");
            
            // Index for date range queries
            entity.HasIndex(e => new { e.EntryDate, e.PrimaryMoodId })
                  .HasDatabaseName("IX_JournalEntries_Date_Mood");
            
            // Full-text search index on Title and Content
            entity.HasIndex(e => e.Title)
                  .HasDatabaseName("IX_JournalEntries_Title");
            
            // Configure mood relationships
            entity.HasOne(e => e.PrimaryMood)
                  .WithMany()
                  .HasForeignKey(e => e.PrimaryMoodId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.SecondaryMood1)
                  .WithMany()
                  .HasForeignKey(e => e.SecondaryMood1Id)
                  .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(e => e.SecondaryMood2)
                  .WithMany()
                  .HasForeignKey(e => e.SecondaryMood2Id)
                  .OnDelete(DeleteBehavior.SetNull);
            
            // DateOnly conversion for SQLite
            entity.Property(e => e.EntryDate)
                  .HasConversion(
                      v => v.ToDateTime(TimeOnly.MinValue),
                      v => DateOnly.FromDateTime(v));
        });
        
        // Mood configuration
        modelBuilder.Entity<Mood>(entity =>
        {
            entity.HasIndex(e => e.Name)
                  .IsUnique()
                  .HasDatabaseName("IX_Moods_Name");
        });
        
        // Tag configuration
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasIndex(e => e.Name)
                  .IsUnique()
                  .HasDatabaseName("IX_Tags_Name");
        });
        
        // EntryTag (join table) configuration
        modelBuilder.Entity<EntryTag>(entity =>
        {
            // Composite unique index to prevent duplicate tags on same entry
            entity.HasIndex(e => new { e.JournalEntryId, e.TagId })
                  .IsUnique()
                  .HasDatabaseName("IX_EntryTags_Entry_Tag");
            
            entity.HasOne(et => et.JournalEntry)
                  .WithMany(e => e.EntryTags)
                  .HasForeignKey(et => et.JournalEntryId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(et => et.Tag)
                  .WithMany(t => t.EntryTags)
                  .HasForeignKey(et => et.TagId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // AppSettings configuration (singleton table)
        modelBuilder.Entity<AppSettings>(entity =>
        {
            entity.HasData(new AppSettings
            {
                Id = 1,
                IsPasswordEnabled = false,
                Theme = "light",
                SessionTimeoutMinutes = 30,
                UpdatedAt = DateTime.UtcNow
            });
        });
    }
}
