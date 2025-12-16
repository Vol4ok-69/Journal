namespace JournalApi.DTOs.Reports;

public class MonthlyReportDTO
{
    public string Group { get; set; } = null!;
    public DateTime Month { get; set; }
    public List<StudentMonthlyReportDTO> Students { get; set; } = new();
}