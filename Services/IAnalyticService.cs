using JournalManagementSystem.Model;

namespace JournalManagementSystem.Services;

public interface IAnalyticService
{
    Task<AnalyticDisplayModel> GetAnalyticsAsync(int Id);

    Task<DashboardSummaryModel> GetDashboardSummaryAsync(int Id);

    StreakResultModel CalculateStreaks(List<DateTime> dates);

    Task<byte[]> GenerateJournalPdfAsync(
    int userId,
    DateTime fromDate,
    DateTime toDate);
}
