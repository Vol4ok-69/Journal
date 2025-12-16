namespace JournalApi.DTOs.Attendance;

public class AttendanceDTO
{
    public int StudentId { get; set; }
    public string StudentSurname { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public string StudentPatronymic { get; set; } = null!;
    public int LessonId { get; set; }
    public bool IsPresent { get; set; }
    public bool IsLate { get; set; }
    public string? Comment { get; set; }
}