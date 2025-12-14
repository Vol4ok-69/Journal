namespace JournalApi.DTOs.Attendance;

public class AttendanceDTO
{
    public int StudentId { get; set; }
    public int LessonId { get; set; }
    public bool IsPresent { get; set; } // true - присутствовал, false - отсутствовал (НБ)
    public bool IsLate { get; set; }   // true - опоздал (НБ/О)
    public string? Comment { get; set; } // Возможен комментарий
}