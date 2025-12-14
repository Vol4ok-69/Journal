namespace JournalApi.DTOs.Auth;

public class LoginRequestDTO
{
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
}