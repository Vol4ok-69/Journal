namespace JournalApi.DTOs.Grades;

public class GradeInfoDTO
{
    public int Id { get; set; }
    public string StudentSurname { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public string StudentPatronymic { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Grade { get; set; } = null!;
    public DateTime Date { get; set; }
    public string? Description { get; set; }
}