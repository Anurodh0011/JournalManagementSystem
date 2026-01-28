using JournalManagementSystem.Data;
using JournalManagementSystem.Model;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace JournalManagementSystem.Services;
public class AnalyticService : IAnalyticService
{

    private readonly AppDbContext _context;

    public AnalyticService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<AnalyticDisplayModel> GetAnalyticsAsync(int Id)
    {
        var journals = await _context.Journals
            .Where(j => j.UserId == Id)
            .OrderBy(j => j.CreatedAt)
            .ToListAsync();

        var result = new AnalyticDisplayModel();

        if (!journals.Any())
            return result;

        // mood distribution
        result.MoodDistribution = journals
            .GroupBy(j => j.PrimaryMood)
            .ToDictionary(g => g.Key, g => g.Count());

        // tag distribution
        result.TagDistribution = journals
            .SelectMany(j => j.Tags)
            .GroupBy(t => t)
            .ToDictionary(g => g.Key, g => g.Count());

        // word count trend
        result.WordCountTrend = journals
            .Select(j => new WordCountTrend
            {
                Date = j.CreatedAt,
                WordCount = j.WordCount
            })
            .ToList();

        return result;
    }

    public async Task<DashboardSummaryModel> GetDashboardSummaryAsync(int Id)
    {
        var journals = await _context.Journals
            .Where(j => j.UserId == Id)
            .ToListAsync();

        if (!journals.Any())
            return new DashboardSummaryModel();

        var mostFrequentMood = journals
            .GroupBy(j => j.PrimaryMood)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        var mostUsedTag = journals
            .SelectMany(j => j.Tags)
            .GroupBy(t => t)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        return new DashboardSummaryModel
        {
            TotalEntries = journals.Count,
            MostFrequentMood = mostFrequentMood,
            MostUsedTag = mostUsedTag
        };
    }
    public StreakResultModel CalculateStreaks(List<DateTime> dates)
    {
        if (dates == null || !dates.Any())
            return new StreakResultModel();

        dates = dates
            .Select(d => d.Date)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        return new StreakResultModel
        {
            CurrentStreak = CalculateCurrentStreak(dates),
            LongestStreak = CalculateLongestStreak(dates),
            MissedDays = CalculateMissedDays(dates)
        };
    }

    private int CalculateCurrentStreak(List<DateTime> dates)
    {
        var set = dates.ToHashSet();
        var today = DateTime.Today;

        if (!set.Contains(today))
            return 0;

        int streak = 0;
        while (set.Contains(today.AddDays(-streak)))
            streak++;

        return streak;
    }

    private int CalculateLongestStreak(List<DateTime> dates)
    {
        int longest = 0;
        int current = 1;

        for (int i = 1; i < dates.Count; i++)
        {
            if ((dates[i] - dates[i - 1]).Days == 1)
                current++;
            else
            {
                longest = Math.Max(longest, current);
                current = 1;
            }
        }

        return Math.Max(longest, current);
    }

    private int CalculateMissedDays(List<DateTime> dates)
    {
        var firstDate = dates.Min();
        var totalDays = (DateTime.Today - firstDate).Days + 1;

        return totalDays - dates.Count;
    }

    public async Task<byte[]> GenerateJournalPdfAsync(
     int userId,
     DateTime fromDate,
     DateTime toDate)
    {
        var journals = await _context.Journals
            .Where(j =>
                j.UserId == userId &&
                j.CreatedAt >= fromDate &&
                j.CreatedAt <= toDate)
            .OrderBy(j => j.CreatedAt)
            .ToListAsync();

        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(35);
                page.PageColor("#FFFFFF");
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(header =>
                {
                    header.Item().Text("Journal Report")
                        .FontSize(20)
                        .SemiBold()
                        .AlignCenter();

                    header.Item().Text(
                        $"Period: {fromDate:dd MMM yyyy} - {toDate:dd MMM yyyy}")
                        .FontSize(11)
                        .AlignCenter()
                        .FontColor("#757575");

                    header.Item().PaddingTop(10)
                        .LineHorizontal(1)
                        .LineColor("#E0E0E0");
                });

                page.Content().PaddingTop(15).Column(col =>
                {
                    if (!journals.Any())
                    {
                        col.Item().AlignCenter().PaddingTop(50)
                            .Text("No journal entries found for the selected period.")
                            .Italic()
                            .FontColor("#757575");
                        return;
                    }

                    foreach (var j in journals)
                    {
                        col.Item().PaddingBottom(15).Border(1)
                            .BorderColor("#E0E0E0")
                            .Padding(12)
                            .Column(card =>
                            {
                                card.Item().Text(j.CreatedAt.ToString("dddd, dd MMM yyyy"))
                                    .FontSize(12)
                                    .SemiBold()
                                    .FontColor("#1E88E5");

                                card.Item().PaddingTop(5)
                                    .Text(j.Title)
                                    .FontSize(14)
                                    .SemiBold();

                                card.Item().PaddingTop(6).Row(row =>
                                {
                                    row.RelativeItem().Text($"Mood: {j.PrimaryMood}")
                                        .FontSize(10)
                                        .FontColor("#757575");

                                    row.RelativeItem().AlignRight()
                                        .Text($"Words: {j.WordCount}")
                                        .FontSize(10)
                                        .FontColor("#757575");
                                });

                                card.Item().PaddingVertical(6)
                                    .LineHorizontal(0.5f)
                                    .LineColor("#E0E0E0");

                                card.Item().Text(j.Description)
                                    .FontSize(11)
                                    .LineHeight(1.4f);
                            });
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generated on ");
                    text.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm"))
                        .SemiBold();
                });
            });
        });

        return document.GeneratePdf();
    }

}
