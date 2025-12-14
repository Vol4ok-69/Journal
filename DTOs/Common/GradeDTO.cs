namespace JournalApi.DTOs.Common;

public class GradeDTO
{
    public int Id { get; set; }
    public string Grade { get; set; } = null!; // e.g., "5", "4", "3", "2", "Зачет", "Незачет"
}