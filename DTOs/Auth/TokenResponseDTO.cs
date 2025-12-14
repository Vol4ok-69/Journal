namespace JournalApi.DTOs.Auth;

public class TokenResponseDTO
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}