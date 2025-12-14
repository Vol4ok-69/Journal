namespace JournalApi.DTOs.Reports;

public class MonthlyReportDTO
{
    public string Group { get; set; } = null!;
    public DateTime Month { get; set; }
    public List<StudentMonthlyReportDTO> Students { get; set; } = new();
}

public class StudentMonthlyReportDTO
{
    public string Surname { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Patronymic { get; set; } = null!;
    public Dictionary<string, List<string>> SubjectGrades { get; set; } = new(); // Key: SubjectName, Value: List of grades
    public Dictionary<string, double> SubjectAverage { get; set; } = new(); // Key: SubjectName, Value: Average grade
    public int TotalAbsences { get; set; }
    public int TotalLateness { get; set; }
}