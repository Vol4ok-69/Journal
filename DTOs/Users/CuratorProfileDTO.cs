namespace JournalApi.DTOs.Users;

public class CuratorProfileDTO
{
    public int Id { get; set; }
    public string Surname { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Patronymic { get; set; }
    public DateOnly Birthday { get; set; }
    public string Login { get; set; } = null!;
    public string? Phone { get; set; }
    public string Post { get; set; } = null!;
    public decimal Salary { get; set; }
    public string GroupCode { get; set; } = null!;
}