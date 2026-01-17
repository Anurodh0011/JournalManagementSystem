using JournalManagementSystem.Models;

namespace JournalManagementSystem.Services;

public interface IJournalService
{
    List<Journal> GetAllJournals();
    Journal? GetJournalById(Guid id);
    Journal CreateJournal(Journal model);
    void UpdateJournal(Journal journal);
    void DeleteJournal(Guid id);
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

    public Journal CreateJournal(Journal model)
    {
        var journal = new Journal
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            DescriptionHtml = model.DescriptionHtml,
            PrimaryMood = model.PrimaryMood,
            SecondaryMoods = model.SecondaryMoods,
            Tags = model.Tags,
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
}