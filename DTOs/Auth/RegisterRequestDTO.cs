namespace JournalApi.DTOs.Auth;

public class RegisterRequestDTO
{
    public string Surname { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Patronymic { get; set; }
    public DateOnly Birthday { get; set; }
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Role { get; set; }
    public int? GroupId { get; set; }
    public int? PostId { get; set; }
    public decimal? Salary { get; set; }
}