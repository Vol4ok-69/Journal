namespace JournalApi.DTOs.Auth;

public class TokenResponseDTO
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? Message { get; set; }
}