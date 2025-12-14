namespace JournalApi.DTOs.Attendance;

public class AttendanceDTO
{
    public int StudentId { get; set; }
    public int LessonId { get; set; }
    public bool IsPresent { get; set; }
    public bool IsLate { get; set; }
    public string? Comment { get; set; }
}