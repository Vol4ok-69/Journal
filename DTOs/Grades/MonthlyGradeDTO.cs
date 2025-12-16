namespace JournalApi.DTOs.Grades;

public class MonthlyGradeDTO
{
    public int SubjectId { get; set; }
    public DateTime Month { get; set; }
    public List<StudentGradeDTO> StudentGrades { get; set; } = new();
}