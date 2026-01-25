namespace JournalManagementSystem.Model;

using System.ComponentModel.DataAnnotations;

public class JournalViewModel
{
    public int JournalId { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage = "Title can be max 100 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Primary Mood is required")]
    public string PrimaryMood { get; set; } = string.Empty;

    public List<String> SecondaryMoods { get; set; } = new();

    public List<String> Tags { get; set; } = new();

    [Required(ErrorMessage = "Created at is required")]
    public DateTime CreatedAt { get; set; }
}