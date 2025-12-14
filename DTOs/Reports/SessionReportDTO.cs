namespace JournalApi.DTOs.Reports;

public class SessionReportDTO
{
    public string Group { get; set; } = null!;
    public int SemesterNumber { get; set; }
    public int Year { get; set; }
    public List<StudentSessionReportDTO> Students { get; set; } = new();
}

public class StudentSessionReportDTO
{
    public string Surname { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Patronymic { get; set; } = null!;
    public Dictionary<string, string> FinalGrades { get; set; } = new();
    public bool IsAcademicProbation { get; set; }
}