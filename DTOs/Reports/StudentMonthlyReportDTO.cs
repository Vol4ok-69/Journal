namespace JournalApi.DTOs.Reports;

public class StudentMonthlyReportDTO
{
    public string Surname { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Patronymic { get; set; } = null!;
    public Dictionary<string, List<string>> SubjectGrades { get; set; } = new();
    public Dictionary<string, double> SubjectAverage { get; set; } = new();
    public int TotalAbsences { get; set; }
    public int TotalLateness { get; set; }
}