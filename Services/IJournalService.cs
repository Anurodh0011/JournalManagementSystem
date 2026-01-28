using JournalManagementSystem.Common;
using JournalManagementSystem.Entities;
using JournalManagementSystem.Model;

namespace JournalManagementSystem.Services;

public interface IJournalService
{
    Task<ServiceResult<Journal>> AddOrUpdateJournalAsync(int userId, JournalViewModel model);
    Task<Journal?> GetJournalByDateAsync(int userId, DateTime date);
    Task<(List<JournalDisplayModel> Journals, int TotalCount)> GetAllJournalsByUserAsync(
        int userId, int page = 1, int pageSize = 10);
    Task<bool> DeleteJournalAsync(int userId, int journalId);
    Task<bool> HasJournalForTodayAsync(int userId);

    Task<(List<JournalDisplayModel>, int)> SearchJournalsAsync(
        int userId,
        string title,
        string mood,
        string tag,
        DateTime? fromDate,
        DateTime? toDate,
        int page,
        int pageSize);
}
