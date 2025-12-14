namespace JournalApi.DTOs.Grades;

public class CreateGradeDTO
{
    public int StudentId { get; set; }
    public int LessonId { get; set; }
    public int GradeId { get; set; }
    public string? Description { get; set; }
}