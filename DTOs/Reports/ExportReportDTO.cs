namespace JournalApi.DTOs.Reports;

public class ExportReportDTO
{
    public string Type { get; set; } = null!;
    public byte[] Content { get; set; } = null!;
    public string FileName { get; set; } = null!;
}