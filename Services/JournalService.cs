using JournalManagementSystem.Entities;

namespace JournalManagementSystem.Services;

public interface IJournalService
{
    List<Journal> GetAllJournals();
    Journal? GetJournalById(Guid id);
    Journal? GetJournalByDate(DateTime date);
    bool HasJournalForDate(DateTime date);
    Journal CreateJournal(Journal model);
    void UpdateJournal(Journal journal);
    void DeleteJournal(Guid id);
    int GetCurrentStreak();
    int GetLongestStreak();
    List<DateTime> GetMissedDays(DateTime startDate, DateTime endDate);
}

public class JournalService : IJournalService
{
    private readonly List<Journal> _journals = new();

    public List<Journal> GetAllJournals()
    {
        return _journals.OrderByDescending(j => j.CreatedAt).ToList();
    }

    public Journal? GetJournalById(Guid id)
    {
        return _journals.FirstOrDefault(j => j.Id == id);
    }

    public Journal? GetJournalByDate(DateTime date)
    {
        var dateOnly = date.Date;
        return _journals.FirstOrDefault(j => j.Date.Date == dateOnly);
    }

    public bool HasJournalForDate(DateTime date)
    {
        var dateOnly = date.Date;
        return _journals.Any(j => j.Date.Date == dateOnly);
    }

    public Journal CreateJournal(Journal model)
    {
        var today = DateTime.Today;
        // check if journal already exists for today
        var existingJournal = GetJournalByDate(today);
        if (existingJournal != null)
        {
            throw new InvalidOperationException("A journal entry already exists for today. You can only create one journal per day.");
        }
        var journal = new Journal
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            DescriptionHtml = model.DescriptionHtml,
            PrimaryMood = model.PrimaryMood,
            SecondaryMoods = model.SecondaryMoods,
            Tags = model.Tags,
            Date = today,
            CreatedAt = DateTime.Now
        };

        _journals.Add(journal);
        return journal;
    }

    public void UpdateJournal(Journal journal)
    {
        var existingJournal = _journals.FirstOrDefault(j => j.Id == journal.Id);
        if (existingJournal != null)
        {
            existingJournal.Title = journal.Title;
            existingJournal.DescriptionHtml = journal.DescriptionHtml;
            existingJournal.PrimaryMood = journal.PrimaryMood;
            existingJournal.SecondaryMoods = journal.SecondaryMoods;
            existingJournal.Tags = journal.Tags;
            existingJournal.UpdatedAt = DateTime.Now;
        }
    }

    public void DeleteJournal(Guid id)
    {
        var journal = _journals.FirstOrDefault(j => j.Id == id);
        if (journal != null)
        {
            _journals.Remove(journal);
        }
    }

    public int GetCurrentStreak()
    {
        if (!_journals.Any())
            return 0;

        var orderedJournals = _journals
            .OrderByDescending(j => j.Date)
            .Select(j => j.Date.Date)
            .Distinct()
            .ToList();

        var today = DateTime.Today;
        var streak = 0;

        // check if there's a journal for today or yesterday
        var currentDate = orderedJournals.Contains(today) ? today : today.AddDays(-1);

        if (!orderedJournals.Contains(currentDate))
            return 0;

        while (orderedJournals.Contains(currentDate))
        {
            streak++;
            currentDate = currentDate.AddDays(-1);
        }

        return streak;
    }

    public int GetLongestStreak()
    {
        if (!_journals.Any())
            return 0;

        var journalDates = _journals
            .Select(j => j.Date.Date)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        int longestStreak = 1;
        int currentStreak = 1;

        for (int i = 1; i < journalDates.Count; i++)
        {
            if ((journalDates[i] - journalDates[i - 1]).TotalDays == 1)
            {
                currentStreak++;
                longestStreak = Math.Max(longestStreak, currentStreak);
            }
            else
            {
                currentStreak = 1;
            }
        }

        return longestStreak;
    }

    public List<DateTime> GetMissedDays(DateTime startDate, DateTime endDate)
    {
        var journalDates = _journals
            .Where(j => j.Date.Date >= startDate.Date && j.Date.Date <= endDate.Date)
            .Select(j => j.Date.Date)
            .ToHashSet();

        var missedDays = new List<DateTime>();
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            if (!journalDates.Contains(date))
            {
                missedDays.Add(date);
            }
        }

        return missedDays;
    }
}