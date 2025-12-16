namespace JournalApi.DTOs.Lessons;

public class LessonDTO
{
    public int Id { get; set; }
    public string Subject { get; set; } = null!;
    public string Teacher { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Group { get; set; } = null!;
    public int Number { get; set; }
    public string LessonType { get; set; } = null!;
    public string? Topic { get; set; }
    public int TeacherId { get; set; }
}