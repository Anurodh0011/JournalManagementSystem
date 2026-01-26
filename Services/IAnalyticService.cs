using JournalManagementSystem.Model;

namespace JournalManagementSystem.Services;

public interface IAnalyticService
{
    //Task<(List<JournalDisplayModel> Journals, int TotalCount)> SearchJournalsAsync(
    //        int userId,
    //        string searchText,
    //        DateTime? fromDate,
    //        DateTime? toDate,
    //        int page,
    //        int pageSize);
    Task<AnalyticDisplayModel> GetAnalyticsAsync(int Id);

    Task<DashboardSummaryModel> GetDashboardSummaryAsync(int Id);

    StreakResultModel CalculateStreaks(List<DateTime> dates);

    Task<byte[]> GenerateJournalPdfAsync(
    int userId,
    DateTime fromDate,
    DateTime toDate);
}
