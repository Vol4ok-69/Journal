namespace JournalApi.DTOs.Grades;

public class CreateGradeDTO
{
    public int StudentId { get; set; }
    public int LessonId { get; set; }
    public int GradeId { get; set; } // ID из таблицы Grades
    public string? Description { get; set; } // Комментарий к оценке
}