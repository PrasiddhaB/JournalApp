using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using JournalApp.Data;
using JournalApp.Models;
using JournalApp.Repositories;

using PdfColors = QuestPDF.Helpers.Colors;

namespace JournalApp.Services;

public class ExportPdfService
{
    private readonly IJournalRepository _repository;

    public ExportPdfService(IJournalRepository repository)
    {
        _repository = repository;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<string> ExportToPdfAsync(DateOnly startDate, DateOnly endDate, string? outputPath = null)
    {
        var entries = await _repository.GetEntriesInRangeAsync(startDate, endDate);

        var fileName = $"Journal_{startDate:yyyy-MM-dd}_to_{endDate:yyyy-MM-dd}.pdf";
        var filePath = outputPath ?? Path.Combine(DbPathHelper.GetExportsFolder(), fileName);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(QuestPDF.Helpers.PageSizes.A4);
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(11));

                // Header
                page.Header()
                    .PaddingBottom(20)
                    .Column(column =>
                    {
                        column.Item()
                            .Text("Personal Journal")
                            .FontSize(24)
                            .Bold()
                            .FontColor(PdfColors.Blue.Darken2);

                        column.Item()
                            .Text($"Exported on {DateTime.Now:MMMM d, yyyy}")
                            .FontSize(10)
                            .FontColor(PdfColors.Grey.Medium);
                    });

                // Content
                page.Content()
                    .PaddingVertical(10)
                    .Column(column =>
                    {
                        column.Spacing(10);

                        column.Item()
                            .Text($"Journal Entries: {startDate:MMMM d, yyyy} - {endDate:MMMM d, yyyy}")
                            .FontSize(14)
                            .SemiBold();

                        column.Item()
                            .Text($"Total Entries: {entries.Count}")
                            .FontSize(10)
                            .FontColor(PdfColors.Grey.Medium);

                        column.Item().PaddingTop(10);

                        if (!entries.Any())
                        {
                            column.Item()
                                .Text("No journal entries found in this date range.")
                                .Italic()
                                .FontColor(PdfColors.Grey.Medium);
                        }
                        else
                        {
                            foreach (var entry in entries.OrderBy(e => e.EntryDate))
                            {
                                column.Item()
                                    .Border(1)
                                    .BorderColor(PdfColors.Grey.Lighten2)
                                    .Padding(15)
                                    .Column(entryColumn =>
                                    {
                                        entryColumn.Spacing(5);

                                        // Date
                                        entryColumn.Item()
                                            .Text(entry.EntryDate.ToString("dddd, MMMM d, yyyy"))
                                            .FontSize(10)
                                            .FontColor(PdfColors.Grey.Darken1);

                                        // Title
                                        entryColumn.Item()
                                            .Text(entry.Title)
                                            .FontSize(16)
                                            .Bold()
                                            .FontColor(PdfColors.Blue.Darken1);

                                        // Mood
                                        if (entry.PrimaryMood != null)
                                        {
                                            entryColumn.Item()
                                                .Text($"Mood: {entry.PrimaryMood.Name}")
                                                .FontSize(10)
                                                .FontColor(GetMoodColor(entry.PrimaryMood.Category));
                                        }

                                        // Tags
                                        if (entry.EntryTags.Any())
                                        {
                                            var tagNames = entry.EntryTags
                                                .Where(et => et.Tag != null)
                                                .Select(et => et.Tag!.Name);

                                            entryColumn.Item()
                                                .Text($"Tags: {string.Join(", ", tagNames)}")
                                                .FontSize(9)
                                                .FontColor(PdfColors.Grey.Darken1);
                                        }

                                        // Category
                                        if (!string.IsNullOrEmpty(entry.Category))
                                        {
                                            entryColumn.Item()
                                                .Text($"Category: {entry.Category}")
                                                .FontSize(9)
                                                .Italic()
                                                .FontColor(PdfColors.Grey.Medium);
                                        }

                                        // Content
                                        entryColumn.Item()
                                            .PaddingTop(8)
                                            .Text(entry.Content)
                                            .FontSize(11)
                                            .LineHeight(1.5f);

                                        // Word count
                                        entryColumn.Item()
                                            .PaddingTop(8)
                                            .Text($"{entry.WordCount} words • Created: {entry.CreatedAt:g}")
                                            .FontSize(8)
                                            .FontColor(PdfColors.Grey.Medium);
                                    });
                            }
                        }
                    });

                // Footer
                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
            });
        });

        document.GeneratePdf(filePath);

        return filePath;
    }

    private string GetMoodColor(MoodCategory category)
    {
        return category switch
        {
            MoodCategory.Positive => PdfColors.Green.Darken1,
            MoodCategory.Neutral => PdfColors.Blue.Medium,
            MoodCategory.Negative => PdfColors.Red.Darken1,
            _ => PdfColors.Grey.Medium
        };
    }

    public void OpenExportsFolder()
    {
        var folder = DbPathHelper.GetExportsFolder();
        System.Diagnostics.Process.Start("explorer.exe", folder);
    }
}