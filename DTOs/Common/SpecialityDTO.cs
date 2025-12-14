namespace JournalApi.DTOs.Common;

public class LessonTypeDTO
{
    public int Id { get; set; }
    public string Value { get; set; } = null!; // e.g., "Лекция", "Практика", "Лабораторная работа"
}