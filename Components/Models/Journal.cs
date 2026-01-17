namespace JournalManagementSystem.Models;

public class Journal
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string DescriptionHtml { get; set; } = string.Empty;
    public string PrimaryMood { get; set; } = string.Empty;
    public List<string> SecondaryMoods { get; set; } = new();
    public HashSet<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}