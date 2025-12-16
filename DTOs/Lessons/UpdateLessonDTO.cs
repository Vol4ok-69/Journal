namespace JournalApi.DTOs.Lessons;

public class UpdateLessonDTO
{
    public int SubjectId { get; set; }
    public DateTime Date { get; set; }
    public int GroupId { get; set; }
    public int Number { get; set; }
    public int LessonTypeId { get; set; }
    public string? Topic { get; set; }
    public int TeacherId { get; set; }
}