namespace JournalApi.DTOs.Attendance;

public class MarkAttendanceDTO
{
    public int StudentId { get; set; }
    public bool IsPresent { get; set; }
    public bool IsLate { get; set; }
    public string? Comment { get; set; }
}