namespace JournalApi.DTOs.Grades;

public class MonthlyGradeDTO
{
    public string StudentSurname { get; set; } = null!;
    public string StudentName { get; set; } = null!;
    public string StudentPatronymic { get; set; } = null!;
    public Dictionary<string, List<string>> SubjectGrades { get; set; } = new(); // Key: SubjectName, Value: List of grades for the month
    public Dictionary<string, double> SubjectAverage { get; set; } = new(); // Key: SubjectName, Value: Average grade
}