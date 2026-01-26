namespace JournalManagementSystem.Model;

public class AnalyticDisplayModel
{
    public Dictionary<string, int> MoodDistribution { get; set; } = new();
    public Dictionary<string, int> TagDistribution { get; set; } = new();

    public List<WordCountTrend> WordCountTrend { get; set; } = new();
}

public class WordCountTrend
{
    public DateTime Date { get; set; }
    public int WordCount { get; set; } 
}
