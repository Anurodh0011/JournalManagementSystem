using JournalManagementSystem.Data;
using JournalManagementSystem.Entities;
using JournalManagementSystem.Model;
using JournalManagementSystem.Common;
using Microsoft.EntityFrameworkCore;

namespace JournalManagementSystem.Services;

public class JournalService : IJournalService
{
    private readonly AppDbContext _context;

    public JournalService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<Journal>> AddOrUpdateJournalAsync(int userId, JournalViewModel model)
    {
        try
        {
            // Prevent future date
            if (model.CreatedAt > DateTime.Today)
                return ServiceResult<Journal>.FailureResult("Cannot create journal for future dates");

            // Check if journal exists for this user & date
            var journal = await _context.Journals
                .FirstOrDefaultAsync(j => j.UserId == userId && j.CreatedAt.Date == model.CreatedAt.Date);

            if (journal == null)
            {
                // Create new
                journal = new Journal
                {
                    UserId = userId,
                    Title = model.Title,
                    Description = model.Description,
                    CreatedAt = model.CreatedAt.Date,
                    PrimaryMood = model.PrimaryMood,
                    SecondaryMoods = model.SecondaryMoods,
                    Tags = model.Tags,
                    WordCount = CountWords(model.Description),
                    UpdatedAt = DateTime.Now
                };

                _context.Journals.Add(journal);
            }
            else
            {
                // Update existing
                journal.Title = model.Title;
                journal.Description = model.Description;
                journal.PrimaryMood = model.PrimaryMood;
                journal.SecondaryMoods = model.SecondaryMoods;
                journal.Tags = model.Tags;
                journal.WordCount = CountWords(model.Description);
                journal.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return ServiceResult<Journal>.SuccessResult(journal);
        }
        catch (Exception ex)
        {
            return ServiceResult<Journal>.FailureResult($"Error saving journal: {ex.Message}");
        }
    }

    public async Task<Journal?> GetJournalByDateAsync(int userId, DateTime date)
    {
        return await _context.Journals
            .FirstOrDefaultAsync(j => j.UserId == userId && j.CreatedAt.Date == date.Date);
    }

    private int CountWords(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return 0;

        var words = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Length;
    }

    public async Task<(List<JournalDisplayModel> Journals, int TotalCount)> GetAllJournalsByUserAsync(
    int userId, int page = 1, int pageSize = 10)
    {
        // Only fetch journals of this user
        var query = _context.Journals
            .Where(j => j.UserId == userId)
            .OrderByDescending(j => j.CreatedAt);

        // Get total count for pagination
        int totalCount = await query.CountAsync();

        // Apply pagination
        var journals = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => new JournalDisplayModel
            {
                JournalId = j.JournalId,
                CreatedAt = j.CreatedAt,
                Title = j.Title,
                Description = j.Description,
                PrimaryMood = j.PrimaryMood,
                SecondaryMoods = j.SecondaryMoods,
                Tags = j.Tags,
                WordCount = j.WordCount
            })
            .ToListAsync();

        return (journals, totalCount);
    }

    public async Task<bool> DeleteJournalAsync(int userId, int journalId)
    {
        var journal = await _context.Journals
            .FirstOrDefaultAsync(j => j.JournalId == journalId && j.UserId == userId);

        if (journal == null)
            return false;

        _context.Journals.Remove(journal);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> HasJournalForTodayAsync(int userId)
    {
        return await _context.Journals.AnyAsync(j =>
            j.UserId == userId &&
            j.CreatedAt.Date == DateTime.Today);
    }
}