namespace JournalApi.DTOs.Reports;

public class ExportReportDTO
{
    public string Type { get; set; } = null!; // e.g., "excel", "pdf"
    public byte[] Content { get; set; } = null!; // Байты файла
    public string FileName { get; set; } = null!;
}