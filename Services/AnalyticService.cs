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

        // Mood distribution
        result.MoodDistribution = journals
            .GroupBy(j => j.PrimaryMood)
            .ToDictionary(g => g.Key, g => g.Count());

        // Tag distribution
        result.TagDistribution = journals
            .SelectMany(j => j.Tags)
            .GroupBy(t => t)
            .ToDictionary(g => g.Key, g => g.Count());

        // Word count trend
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

    // ================= CURRENT STREAK =================
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

    // ================= LONGEST STREAK =================
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

    // ================= MISSED DAYS =================
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
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Text($"Journal Report ({fromDate:dd MMM yyyy} - {toDate:dd MMM yyyy})")
                    .SemiBold().FontSize(16).AlignCenter();

                page.Content().Column(col =>
                {
                    foreach (var j in journals)
                    {
                        col.Item().PaddingBottom(10).BorderBottom(1).Column(c =>
                        {
                            c.Item().Text(j.CreatedAt.ToString("dd MMM yyyy"))
                                .SemiBold().FontSize(12);

                            c.Item().Text(j.Title).SemiBold();
                            c.Item().Text($"Mood: {j.PrimaryMood}");
                            c.Item().Text($"Words: {j.WordCount}");
                            c.Item().Text(j.Description);
                        });
                    }
                });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated on ");
                        x.Span(DateTime.Now.ToString("dd MMM yyyy HH:mm"));
                    });
            });
        });

        return document.GeneratePdf();
    }
}
