namespace JournalManagementSystem.Entities;

public class Journal
{
    public int JournalId { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PrimaryMood { get; set; } = string.Empty;
    public List<string> SecondaryMoods { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public int WordCount { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}