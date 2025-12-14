namespace JournalApi.DTOs.Common;

public class SubjectDTO
{
    public int Id { get; set; }
    public string Value { get; set; } = null!;
    public string? Description { get; set; }
}