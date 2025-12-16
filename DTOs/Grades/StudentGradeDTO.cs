namespace JournalApi.DTOs.Grades;

public class StudentGradeDTO
{
    public int StudentId { get; set; }
    public string StudentSurname { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public string StudentPatronymic { get; set; } = null!;
    public int LessonId { get; set; }
    public string Subject { get; set; } = null!;
    public string Grade { get; set; } = null!;
    public string? Description { get; set; }
    public List<string> Grades { get; set; } = [];
}