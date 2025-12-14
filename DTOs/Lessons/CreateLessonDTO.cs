namespace JournalApi.DTOs.Lessons;

public class CreateLessonDTO
{
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public DateTime Date { get; set; }
    public int GroupId { get; set; }
    public int Number { get; set; } // Номер пары в день (1, 2, 3, ...)
    public int LessonTypeId { get; set; }
    public string? Topic { get; set; } // Тема урока (может быть в модели Lesson)
}