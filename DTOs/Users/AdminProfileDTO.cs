namespace JournalApi.DTOs.Users;

public class AdminProfileDTO
{
    public int Id { get; set; }
    public string Surname { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Patronymic { get; set; }
    public DateOnly Birthday { get; set; }
    public string Login { get; set; } = null!;
    public string? Phone { get; set; }
    public string Post { get; set; } = null!;
}